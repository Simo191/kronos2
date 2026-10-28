using GestionPrestataires.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.TypeShifts.Commands
{
    public class CreateTypeShiftCommandHandler : IRequestHandler<CreateTypeShiftCommand, Guid>
    {
        private readonly ITypeShiftRepository _context;

        public CreateTypeShiftCommandHandler(ITypeShiftRepository context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateTypeShiftCommand command, CancellationToken cancellationToken)
        {
            var typeShift = new TypeShift
            {
                Id = Guid.NewGuid(),
                Label = command.Label,
                ShortLabel = command.ShortLabel,
                Code = command.Code,
                Color = command.Color,
                BgColor = command.BgColor,
                BorderColor = command.BorderColor,
                TextColor = command.TextColor,
                Order = command.Order,
                Category = command.Category, //   NOUVEAU
                IsActive = command.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.AddAsync(typeShift);
 
            return typeShift.Id;
        }
    }
}
