using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Role>
{
    private readonly IRoleRepository _repository;

    public CreateRoleCommandHandler(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Role> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = request.Nom,
            DisplayName = request.Nom,
            Description = request.Description
        };

        await _repository.AddAsync(role);
        return role;
    }
}
