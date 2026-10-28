namespace GestionPrestataires.Domain.Entities;

/// <summary>
///   Compte technique d'un prestataire (ADM, ADML, VPN, Bastion…).
///   Le mot de passe est chiffré (AES-256-GCM) ; il n'est jamais renvoyé
///   en clair sauf via l'endpoint /reveal qui exige le mot de passe
///   utilisateur courant.
/// </summary>
public class PrestataireCredential
{
    public Guid Id { get; set; }
    public Guid PrestataireId { get; set; }

    /// <summary>Type / libellé du compte (ex : "ADM", "ADML", "VPN"). Libre.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Identifiant du compte (login).</summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>Mot de passe chiffré (AES-256-GCM, payload [nonce|tag|cipher] en base64).</summary>
    public string EncryptedPassword { get; set; } = string.Empty;

    /// <summary>Date d'expiration (facultative).</summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>Note facultative (URL d'accès, contexte…).</summary>
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Prestataire? Prestataire { get; set; }
}
