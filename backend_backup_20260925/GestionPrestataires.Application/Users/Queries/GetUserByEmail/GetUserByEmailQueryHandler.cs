using GestionPrestataires.Application.Users.Queries.GetUserById;
using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
