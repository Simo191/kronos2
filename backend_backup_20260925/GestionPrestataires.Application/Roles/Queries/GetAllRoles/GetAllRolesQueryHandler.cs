using MediatR;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;

namespace GestionPrestataires.Application.Roles.Queries.GetAllRoles;

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<Role>>
{
    private readonly IRoleRepository _repository;

    public GetAllRolesQueryHandler(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Role>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
