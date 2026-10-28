using GestionPrestataires.Application.DTOs;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Shifts.Queries
{
    public class GetShiftsByMonthQueryHandler : IRequestHandler<GetShiftsByMonthQuery, List<ShiftDTO>>
    {
        private readonly IShiftRepository _context;

        public GetShiftsByMonthQueryHandler(IShiftRepository context)
        {
            _context = context;
        }

        public async Task<List<ShiftDTO>> Handle(GetShiftsByMonthQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var startDate = new DateTime(request.Year, request.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var query = await _context.GetShiftsByDateRangeAsync(startDate, endDate);

                if (request.PrestataireId.HasValue)
                {
                    query = query.Where(s => s.PrestataireId == request.PrestataireId.Value);
                }

                var shifts = query
                    .OrderBy(s => s.Date)
                    .ThenBy(s => s.Prestataire.Nom).ToList();

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
