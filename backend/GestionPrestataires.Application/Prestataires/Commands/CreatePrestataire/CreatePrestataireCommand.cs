using MediatR;

namespace GestionPrestataires.Application.Prestataires.Commands.CreatePrestataire;

public record CreatePrestataireCommand(
    string Nom,
    string Prenom,
    string Email,
    string? Telephone,
    decimal Tjm,
    bool Responsable,
    int Order,
    string? ImageUrl,   // ✅ NOUVEAU
    Guid? EntrepriseId = null   //   Entreprise / SSII de rattachement (facultatif)

) : IRequest<Guid>;
