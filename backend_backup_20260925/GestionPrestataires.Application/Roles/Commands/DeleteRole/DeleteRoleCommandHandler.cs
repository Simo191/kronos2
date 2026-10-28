using MediatR;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Roles.Commands.DeleteRole;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Unit>
{
    private readonly IRoleRepository _repository;

    public DeleteRoleCommandHandler(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
