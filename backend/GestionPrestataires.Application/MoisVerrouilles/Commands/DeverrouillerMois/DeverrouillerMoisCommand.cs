using MediatR;

namespace GestionPrestataires.Application.MoisVerrouilles.Commands.DeverrouillerMois;

public record DeverrouillerMoisCommand(Guid Id) : IRequest;