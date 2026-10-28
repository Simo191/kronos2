namespace GestionPrestataires.Application.DTOs;

/// <summary>
///   Catégorie de notification.
///   Les contrats sont décomposés en 2 catégories distinctes : un même contrat
///   peut générer 0, 1 ou 2 notifications (une "date" + une "quota").
/// </summary>
public enum NotificationKind
{
    /// <summary>Contrat dont la date de fin approche ou est dépassée.</summary>
    ContratDate = 0,
    /// <summary>Contrat dont le quota de jours est presque ou totalement épuisé.</summary>
    ContratQuota = 1,
    /// <summary>Compte technique (ADM, ADML, VPN…) qui expire.</summary>
    Credential = 2
}

/// <summary>
///   Niveau d'alerte :
///   - Warning : expire bientôt (orange)
///   - Danger  : déjà expiré (rouge)
/// </summary>
public enum NotificationSeverity
{
    Warning = 0,
    Danger = 1
}

/// <summary>
///   Notification d'expiration (contrat ou compte technique).
///   Calculées à la volée à chaque appel ; pas de stockage.
/// </summary>
public class NotificationDto
{
    /// <summary>Id stable = "contrat-{guid}" ou "cred-{guid}" (pour le tracking côté front).</summary>
    public string Id { get; set; } = string.Empty;
    public NotificationKind Kind { get; set; }
    public NotificationSeverity Severity { get; set; }

    public Guid PrestataireId { get; set; }
    public string PrestataireName { get; set; } = string.Empty;

    /// <summary>Titre court : "Contrat Freelance" ou "Compte ADM — login".</summary>
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    /// <summary>Négatif si déjà expiré (nb jours depuis expiration).</summary>
    public int DaysUntilExpiry { get; set; }
}
