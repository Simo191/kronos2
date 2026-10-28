namespace GestionPrestataires.Application.DTOs;

/// <summary>Vue d'un credential : mot de passe TOUJOURS masqué.</summary>
public class PrestataireCredentialDto
{
    public Guid Id { get; set; }
    public Guid PrestataireId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePrestataireCredentialDto
{
    public string Label { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public string? Note { get; set; }
}

public class UpdatePrestataireCredentialDto
{
    public string Label { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    //   Vide / null => on garde l'ancien mot de passe ; non vide => on chiffre le nouveau
    public string? Password { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Note { get; set; }
}

/// <summary>Body pour reveal : mot de passe utilisateur courant.</summary>
public class RevealCredentialRequestDto
{
    public string UserPassword { get; set; } = string.Empty;
}

/// <summary>Réponse reveal : mot de passe en clair (une seule fois).</summary>
public class RevealCredentialResponseDto
{
    public Guid Id { get; set; }
    public string Password { get; set; } = string.Empty;
}
