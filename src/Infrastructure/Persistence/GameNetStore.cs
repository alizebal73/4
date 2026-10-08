using GameNet.Application.Foundation;
using GameNet.Domain.Agents;
using GameNet.Domain.Identity;
using GameNet.Domain.Stations;
using Microsoft.EntityFrameworkCore;

namespace GameNet.Infrastructure.Persistence;

public sealed class GameNetStore(GameNetDbContext db) : IGameNetStore
{
    public async Task<IReadOnlyList<Operator>> ListOperatorsAsync(
        CancellationToken cancellationToken) =>
        await db.Operators.OrderBy(x => x.UserName).ToListAsync(cancellationToken);

    public Task<Operator?> FindOperatorByUserNameAsync(
        string userName,
        CancellationToken cancellationToken) =>
        db.Operators.SingleOrDefaultAsync(
            x => x.UserName == userName,
            cancellationToken);

    public Task<Operator?> GetOperatorAsync(
        Guid operatorId,
        CancellationToken cancellationToken) =>
        db.Operators.SingleOrDefaultAsync(
            x => x.Id == operatorId,
            cancellationToken);

    public Task AddOperatorAsync(
        Operator operatorAccount,
        CancellationToken cancellationToken) =>
        db.Operators.AddAsync(operatorAccount, cancellationToken).AsTask();

    public Task<OperatorSession?> FindOperatorSessionByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken) =>
        db.OperatorSessions.SingleOrDefaultAsync(
            x => x.TokenHash == tokenHash,
            cancellationToken);

    public Task AddOperatorSessionAsync(
        OperatorSession session,
        CancellationToken cancellationToken) =>
        db.OperatorSessions.AddAsync(session, cancellationToken).AsTask();

    public async Task<IReadOnlyList<Station>> ListStationsAsync(
        CancellationToken cancellationToken) =>
        await db.Stations
            .OrderBy(x => x.Number)
            .ToListAsync(cancellationToken);

    public Task<bool> StationNumberExistsAsync(
        int number,
        CancellationToken cancellationToken) =>
        db.Stations.AnyAsync(x => x.Number == number, cancellationToken);

    public Task<Station?> GetStationAsync(
        Guid stationId,
        CancellationToken cancellationToken) =>
        db.Stations.SingleOrDefaultAsync(
            x => x.Id == stationId,
            cancellationToken);

    public Task AddStationAsync(
        Station station,
        CancellationToken cancellationToken) =>
        db.Stations.AddAsync(station, cancellationToken).AsTask();

    public Task<AgentDevice?> GetAgentAsync(
        Guid agentId,
        CancellationToken cancellationToken) =>
        db.AgentDevices.SingleOrDefaultAsync(
            x => x.Id == agentId,
            cancellationToken);

    public Task<AgentDevice?> FindAgentByDeviceIdAsync(
        string deviceId,
        CancellationToken cancellationToken) =>
        db.AgentDevices.SingleOrDefaultAsync(
            x => x.DeviceId == deviceId,
            cancellationToken);

    public Task<AgentDevice?> FindAgentByCredentialHashAsync(
        string credentialHash,
        CancellationToken cancellationToken) =>
        db.AgentDevices.SingleOrDefaultAsync(
            x => x.CredentialHash == credentialHash,
            cancellationToken);

    public Task AddAgentAsync(
        AgentDevice agent,
        CancellationToken cancellationToken) =>
        db.AgentDevices.AddAsync(agent, cancellationToken).AsTask();

    public async Task<IReadOnlyList<AgentDevice>> ListAgentsAsync(
        CancellationToken cancellationToken) =>
        await db.AgentDevices
            .OrderBy(x => x.DeviceId)
            .ToListAsync(cancellationToken);

    public async Task<bool> BindAgentToStationAsync(
        Guid agentId,
        Guid stationId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable,
            cancellationToken);

        var agent = await db.AgentDevices.SingleOrDefaultAsync(
            x => x.Id == agentId,
            cancellationToken);
        var station = await db.Stations.SingleOrDefaultAsync(
            x => x.Id == stationId,
            cancellationToken);

        if (agent is null || station is null)
        {
            return false;
        }

        if (agent.StationId is not null && agent.StationId != stationId)
        {
            return false;
        }

        if (station.Lifecycle is Domain.Stations.StationLifecycle.Disabled)
        {
            return false;
        }

        var occupiedByAnother = await db.AgentDevices
            .AnyAsync(
                x => x.StationId == stationId && x.Id != agentId,
                cancellationToken);

        if (occupiedByAnother)
        {
            return false;
        }

        try
        {
            agent.BindStation(station.Id);

            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception) when (transaction.GetDbTransaction().Connection is not null)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
    }

    public async Task<bool> UnbindAgentFromStationAsync(
        Guid agentId,
        Guid stationId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var agent = await db.AgentDevices.SingleOrDefaultAsync(
            x => x.Id == agentId,
            cancellationToken);
        var station = await db.Stations.SingleOrDefaultAsync(
            x => x.Id == stationId,
            cancellationToken);

        if (agent is null || station is null)
        {
            return false;
        }

        agent.UnbindStation(station.Id);

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        db.SaveChangesAsync(cancellationToken);
}
