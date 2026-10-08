using GameNet.Domain.Agents;
using GameNet.Domain.Identity;
using GameNet.Domain.Stations;

namespace GameNet.Application.Foundation;

public interface IGameNetStore
{
    Task<IReadOnlyList<Operator>> ListOperatorsAsync(CancellationToken cancellationToken);
    Task<Operator?> FindOperatorByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<Operator?> GetOperatorAsync(Guid operatorId, CancellationToken cancellationToken);
    Task AddOperatorAsync(Operator operatorAccount, CancellationToken cancellationToken);

    Task<OperatorSession?> FindOperatorSessionByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken);

    Task AddOperatorSessionAsync(
        OperatorSession session,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Station>> ListStationsAsync(CancellationToken cancellationToken);
    Task<bool> StationNumberExistsAsync(int number, CancellationToken cancellationToken);
    Task<Station?> GetStationAsync(Guid stationId, CancellationToken cancellationToken);
    Task AddStationAsync(Station station, CancellationToken cancellationToken);

    Task<AgentDevice?> GetAgentAsync(Guid agentId, CancellationToken cancellationToken);
    Task<AgentDevice?> FindAgentByDeviceIdAsync(string deviceId, CancellationToken cancellationToken);
    Task<AgentDevice?> FindAgentByCredentialHashAsync(
        string credentialHash,
        CancellationToken cancellationToken);
    Task AddAgentAsync(AgentDevice agent, CancellationToken cancellationToken);
    Task<IReadOnlyList<AgentDevice>> ListAgentsAsync(CancellationToken cancellationToken);

    Task<bool> BindAgentToStationAsync(
        Guid agentId,
        Guid stationId,
        CancellationToken cancellationToken);

    Task<bool> UnbindAgentFromStationAsync(
        Guid agentId,
        Guid stationId,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
