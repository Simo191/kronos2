namespace GestionPrestataires.Application.DTOs;

public class ApplicationPlanningDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Statut { get; set; } = "Planifié";
    public string Priorite { get; set; } = "Moyenne";
    public string Couleur { get; set; } = "#3b82f6";
    public int Avancement { get; set; }
    public int Ordre { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime DateModification { get; set; }
    public List<AssignmentDto> Prestataires { get; set; } = new();
    public List<ApplicationServerDto> Servers { get; set; } = new();
}

public class ApplicationServerDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "DEV";
    public string Identifiant { get; set; } = string.Empty;
    public string? Libelle { get; set; }
    public string? Note { get; set; }
    public int Ordre { get; set; }
}

public class ApplicationServerInputDto
{
    public string Type { get; set; } = "DEV";
    public string Identifiant { get; set; } = string.Empty;
    public string? Libelle { get; set; }
    public string? Note { get; set; }
    public int Ordre { get; set; }
}

public class AssignmentDto
{
    public Guid PrestataireId { get; set; }
    public string NomComplet { get; set; } = string.Empty;
    public string? Role { get; set; }
}

public class CreateApplicationPlanningDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Statut { get; set; } = "Planifié";
    public string Priorite { get; set; } = "Moyenne";
    public string Couleur { get; set; } = "#3b82f6";
    public int Avancement { get; set; }
    public int Ordre { get; set; }
    public List<AssignmentInputDto> Prestataires { get; set; } = new();
    public List<ApplicationServerInputDto> Servers { get; set; } = new();
}

public class UpdateApplicationPlanningDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Statut { get; set; } = "Planifié";
    public string Priorite { get; set; } = "Moyenne";
    public string Couleur { get; set; } = "#3b82f6";
    public int Avancement { get; set; }
    public int Ordre { get; set; }
    public List<AssignmentInputDto> Prestataires { get; set; } = new();
    //   null (champ absent, ex. réordonnancement du planning) = serveurs inchangés ;
    //   liste (même vide) = remplace les serveurs de l'application
    public List<ApplicationServerInputDto>? Servers { get; set; }
}

public class AssignmentInputDto
{
    public Guid PrestataireId { get; set; }
    public string? Role { get; set; }
}