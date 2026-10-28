using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Cagnottes.Commands.DeleteCagnotte;
public record DeleteCagnotteCommand(Guid Id) : IRequest<Unit>;