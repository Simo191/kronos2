using MediatR;

namespace GestionPrestataires.Application.Contrats.Commands.UpdateContratDocument;

public record UpdateContratDocumentCommand(
    Guid Id,
    string DocumentUrl
) : IRequest;
