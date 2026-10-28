using MediatR;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Unit>
{
    private readonly IRoleRepository _repository;

    public UpdateRoleCommandHandler(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _repository.GetByIdAsync(request.Id);
        if (role == null)
            throw new Exception("Rôle non trouvé");

        role.Name = request.Nom;
        role.Description = request.Description;
        role.DisplayName = request.Nom;

        await _repository.UpdateAsync(role);
        return Unit.Value;
    }
}
