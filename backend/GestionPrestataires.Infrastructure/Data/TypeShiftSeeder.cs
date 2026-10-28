namespace GestionPrestataires.Infrastructure.Data
{
    public static class TypeShiftSeeder
    {
        public static void SeedTypeShifts(AppDbContext context)
        {
            if (context.TypeShifts.Any())
            {
                return; // Déjà des données
            }

            var typeShifts = new List<TypeShift>
            {
                // ============ TRAVAIL ============
                new TypeShift
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Label = "Sur Site",
                    ShortLabel = "Sur Site",
                    Code = "SUR_SITE",
                    Color = "#4ade80",
                    BgColor = "bg-green-200",
                    BorderColor = "border-green-400",
                    TextColor = "text-green-900",
                    Order = 1,
                    Category = ShiftCategory.Travail, //   Travail
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TypeShift
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Label = "Télétravail",
                    ShortLabel = "Télétrav.",
                    Code = "TELETRAVAIL",
                    Color = "#facc15",
                    BgColor = "bg-yellow-200",
                    BorderColor = "border-yellow-400",
                    TextColor = "text-yellow-900",
                    Order = 2,
                    Category = ShiftCategory.Travail, //   Travail
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TypeShift
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Label = "Formation",
                    ShortLabel = "Formatio.",
                    Code = "FORMATION",
                    Color = "#60a5fa",
                    BgColor = "bg-blue-200",
                    BorderColor = "border-blue-400",
                    TextColor = "text-blue-900",
                    Order = 3,
                    Category = ShiftCategory.Travail, //   Travail
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TypeShift
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Label = "Déplacement",
                    ShortLabel = "Déplacmt.",
                    Code = "DEPLACEMENT",
                    Color = "#c084fc",
                    BgColor = "bg-purple-200",
                    BorderColor = "border-purple-400",
                    TextColor = "text-purple-900",
                    Order = 4,
                    Category = ShiftCategory.Travail, //   Travail
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // ============ ABSENCE ============
                new TypeShift
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Label = "Absence",
                    ShortLabel = "Absence",
                    Code = "ABSENCE",
                    Color = "#9ca3af",
                    BgColor = "bg-gray-200",
                    BorderColor = "border-gray-400",
                    TextColor = "text-gray-900",
                    Order = 5,
                    Category = ShiftCategory.Absence, //   Absence
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // ============ CONGÉ ============
                new TypeShift
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Label = "Congé",
                    ShortLabel = "Congé",
                    Code = "CONGE",
                    Color = "#f87171",
                    BgColor = "bg-red-200",
                    BorderColor = "border-red-400",
                    TextColor = "text-red-900",
                    Order = 6,
                    Category = ShiftCategory.Conge, //   Congé
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TypeShift
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Label = "École",
                    ShortLabel = "École",
                    Code = "ECOLE",
                    Color = "#fb923c",
                    BgColor = "bg-orange-200",
                    BorderColor = "border-orange-400",
                    TextColor = "text-orange-900",
                    Order = 7,
                    Category = ShiftCategory.Conge, //   Congé
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TypeShift
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    Label = "Séminaire",
                    ShortLabel = "Séminair.",
                    Code = "SEMINAIRE",
                    Color = "#047857",
                    BgColor = "bg-emerald-700",
                    BorderColor = "border-emerald-800",
                    TextColor = "text-white",
                    Order = 8,
                    Category = ShiftCategory.Travail, //   Travail
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.TypeShifts.AddRange(typeShifts);
            context.SaveChanges();

            Console.WriteLine("  8 TypeShifts créés avec catégories (4 Travail, 1 Absence, 3 Congé)");
        }
    }
}
