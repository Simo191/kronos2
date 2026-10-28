using GestionPrestataires.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.TypeShifts.Commands
{
    public class UpdateTypeShiftCommandHandler : IRequestHandler<UpdateTypeShiftCommand, Unit>
    {
        private readonly ITypeShiftRepository _context;

        public UpdateTypeShiftCommandHandler(ITypeShiftRepository context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateTypeShiftCommand command, CancellationToken cancellationToken)
        {
            //var typeShift = await _context.GetByIdAsync(request.Id);

            //if (typeShift == null)
            //{
            //    throw new Exception($"TypeShift {request.Id} not found");
            //}

            //typeShift.Label = request.Label;
            //typeShift.ShortLabel = request.ShortLabel;
            //typeShift.Code = request.Code;
            //typeShift.Color = request.Color;
            //typeShift.BgColor = request.BgColor;
            //typeShift.BorderColor = request.BorderColor;
            //typeShift.TextColor = request.TextColor;
            //typeShift.Order = request.Order;
            //typeShift.IsActive = request.IsActive;
            //typeShift.UpdatedAt = DateTime.UtcNow;

            //await _context.UpdateAsync(typeShift);



            var typeShift = await _context.GetByIdAsync(command.Id);

            if (typeShift == null)
            {
                throw new KeyNotFoundException($"TypeShift avec l'ID {command.Id} n'a pas été trouvé.");
            }

            // Vérifier si le nouveau code existe déjà (si changé)
            if (command.Code != null && command.Code != typeShift.Code)
            {
                if (await _context.CodeExistsAsync(command.Code))
                {
                    throw new InvalidOperationException($"Le code '{command.Code}' existe déjà.");
                }
            }

            // Mettre à jour uniquement les propriétés non-null
            if (command.Label != null)
                typeShift.Label = command.Label;

            if (command.ShortLabel != null)
                typeShift.ShortLabel = command.ShortLabel;

            if (command.Code != null)
                typeShift.Code = command.Code;

            if (command.Color != null)
                typeShift.Color = command.Color;

            if (command.BgColor != null)
                typeShift.BgColor = command.BgColor;

            if (command.BorderColor != null)
                typeShift.BorderColor = command.BorderColor;

            if (command.TextColor != null)
                typeShift.TextColor = command.TextColor;

            if (command.Order.HasValue)
                typeShift.Order = command.Order.Value;

            if (command.Category.HasValue) //   NOUVEAU
                typeShift.Category = command.Category.Value;

            if (command.IsActive.HasValue)
                typeShift.IsActive = command.IsActive.Value;

            typeShift.UpdatedAt = DateTime.UtcNow;

            await _context.UpdateAsync(typeShift);
           
            return Unit.Value;
        }
    }
}
