using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Prestataires.Commands.CreatePrestataire;

public class CreatePrestataireCommandHandler : IRequestHandler<CreatePrestataireCommand, Guid>
{
    private readonly IPrestataireRepository _repository;

    public CreatePrestataireCommandHandler(IPrestataireRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreatePrestataireCommand request, CancellationToken cancellationToken)
    {
        var prestataire = new Prestataire
        {
            Id = Guid.NewGuid(),
            Nom = request.Nom,
            Prenom = request.Prenom,
            Email = request.Email,
            Telephone = request.Telephone,
            Tjm = request.Tjm,
            ImageUrl = request.ImageUrl,       // ✅ NOUVEAU
            Responsable = request.Responsable,
            Order = request.Order,
            EntrepriseId = request.EntrepriseId,   //   Lien entreprise (facultatif)
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(prestataire);
        return prestataire.Id;
    }
}