using MediatR;

namespace GestionPrestataires.Application.JoursTravail.Commands.DeleteJourTravail;

public record DeleteJourTravailCommand(Guid Id) : IRequest;
