using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.Shifts.Queries
{
    public class GetShiftsByWeekQueryHandler : IRequestHandler<GetShiftsByWeekQuery, List<ShiftDTO>>
    {
        private readonly IShiftRepository _context;

        public GetShiftsByWeekQueryHandler(IShiftRepository context)
        {
            _context = context;
        }

        public async Task<List<ShiftDTO>> Handle(GetShiftsByWeekQuery request, CancellationToken cancellationToken)
        {
            try
            {

            
            var endDate = request.WeekStartDate.AddDays(6);
            var query = await _context.GetShiftsByDateRangeAsync(request.WeekStartDate, endDate);

            

            if (request.PrestataireId.HasValue)
            {
                query = query.Where(s => s.PrestataireId == request.PrestataireId.Value);
            }

            var shifts = query
                .OrderBy(s => s.Date)
                .ThenBy(s => s.Prestataire.Nom)
                .ToList();

            return shifts.Select(s => new ShiftDTO
            {
                Id = s.Id,
                PrestataireId = s.PrestataireId,
                PrestataireNom = s.Prestataire.Nom,
                PrestatairePrenom = s.Prestataire.Prenom,
                Date = s.Date,
                NombreJours = s.NombreJours,
                TypeShiftId = s.TypeShiftId,
                TypeShift = new TypeShiftDTO
                {
                    Id = s.TypeShift.Id,
                    Label = s.TypeShift.Label,
                    ShortLabel = s.TypeShift.ShortLabel,
                    Code = s.TypeShift.Code,
                    Color = s.TypeShift.Color,
                    BgColor = s.TypeShift.BgColor,
                    BorderColor = s.TypeShift.BorderColor,
                    TextColor = s.TypeShift.TextColor,
                    Order = s.TypeShift.Order
                },
                Notes = s.Notes
            }).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
