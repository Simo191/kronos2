using MediatR;

namespace GestionPrestataires.Application.Prestataires.Commands.DeletePrestataire;

public record DeletePrestataireCommand(Guid Id) : IRequest;
