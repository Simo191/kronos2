using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Application.DTOs;

public class MoisVerrouillDto
{
    public Guid Id { get; set; }
    public string Mois { get; set; } = string.Empty; // Format: "2025-12"
    public Guid VerrouilleParUserId { get; set; }
    public string? VerrouilleParUserName { get; set; } // Nom complet de l'admin/manager
    public DateTime DateVerrouillage { get; set; }
    public string? Commentaire { get; set; }
}
