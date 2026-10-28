using MediatR;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Roles.Queries.GetRoleById;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Role?>
{
    private readonly IRoleRepository _repository;

    public GetRoleByIdQueryHandler(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Role?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
