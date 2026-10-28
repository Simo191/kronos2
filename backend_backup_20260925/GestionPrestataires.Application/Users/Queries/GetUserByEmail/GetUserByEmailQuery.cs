using GestionPrestataires.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Users.Queries.GetUserByEmail;
    public record GetUserByEmailQuery(string Email) : IRequest<User?>;
