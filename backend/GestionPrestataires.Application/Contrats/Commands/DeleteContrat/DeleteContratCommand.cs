using MediatR;

namespace GestionPrestataires.Application.Contrats.Commands.DeleteContrat;

public record DeleteContratCommand(Guid Id) : IRequest;
