using GestionPrestataires.Domain.Entities;

namespace GestionPrestataires.Domain.Repositories;

public interface IApplicationServerRepository
{
    //   Supprime tous les serveurs liés à une application (utilisé lors d'un Update).
    Task DeleteAllAsync(Guid applicationId);

    //   Ajoute en lot une nouvelle liste de serveurs.
    Task AddRangeAsync(IEnumerable<ApplicationServer> servers);
}
