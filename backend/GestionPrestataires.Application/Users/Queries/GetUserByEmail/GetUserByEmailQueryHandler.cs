using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Users.Queries.GetUserByEmail
{

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, User?>
    {
        private readonly IUserRepository _repository;

        public GetUserByEmailQueryHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByEmailAsync(request.Email);
        }
    }
}
