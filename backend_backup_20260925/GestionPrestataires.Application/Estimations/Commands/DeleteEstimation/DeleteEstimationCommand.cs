using MediatR;

namespace GestionPrestataires.Application.Estimations.Commands.DeleteEstimation;

/// <summary>
/// Command pour supprimer une estimation
/// </summary>
public class DeleteEstimationCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public DeleteEstimationCommand(Guid id)
    {
        Id = id;
    }
}