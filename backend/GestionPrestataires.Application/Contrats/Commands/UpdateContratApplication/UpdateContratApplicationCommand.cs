using MediatR;

namespace GestionPrestataires.Application.Contrats.Commands.UpdateContratApplication;

//   Change uniquement le logiciel d'un contrat (null = revenir au logiciel déduit du planning)
public record UpdateContratApplicationCommand(Guid Id, Guid? ApplicationId) : IRequest<bool>;
