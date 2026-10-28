namespace GestionPrestataires.Application.DTOs;

public class EntrepriseDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateEntrepriseDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
}

public class UpdateEntrepriseDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
