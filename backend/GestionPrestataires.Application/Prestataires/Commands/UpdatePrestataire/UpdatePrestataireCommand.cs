using MediatR;

namespace GestionPrestataires.Application.Prestataires.Commands.UpdatePrestataire;

public record UpdatePrestataireCommand(
    Guid Id,
    string Nom,
    string Prenom,
    string Email,
    string? Telephone,
    decimal Tjm,
   bool Responsable,
   int Order,
    string? ImageUrl,
    Guid? EntrepriseId = null   //   Entreprise / SSII de rattachement (facultatif)
) : IRequest;
