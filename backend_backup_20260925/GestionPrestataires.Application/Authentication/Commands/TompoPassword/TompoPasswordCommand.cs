using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Authentication.Commands.TompoPassword;
public record TompoPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest;