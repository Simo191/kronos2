using GestionPrestataires.Domain.Repositories;
using MediatR;

namespace GestionPrestataires.Application.Prestataires.Commands.UpdatePrestataire;

public class UpdatePrestataireCommandHandler : IRequestHandler<UpdatePrestataireCommand>
{
    private readonly IPrestataireRepository _repository;

    public UpdatePrestataireCommandHandler(IPrestataireRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdatePrestataireCommand request, CancellationToken cancellationToken)
    {
        var prestataire = await _repository.GetByIdAsync(request.Id);
        if (prestataire == null)
            throw new Exception("Prestataire not found");

        prestataire.Nom = request.Nom;
        prestataire.Prenom = request.Prenom;
        prestataire.Email = request.Email;
        prestataire.Telephone = request.Telephone;
        prestataire.Tjm = request.Tjm;
        prestataire.UpdatedAt = DateTime.UtcNow;
        prestataire.Responsable = request.Responsable;
        prestataire.Order = request.Order;
        prestataire.EntrepriseId = request.EntrepriseId;   //   nullable : on accepte la mise à null
        // ✅ NOUVEAU : ne pas écraser l'image si null envoyé
        if (request.ImageUrl != null)
            prestataire.ImageUrl = request.ImageUrl;

        await _repository.UpdateAsync(prestataire);
    }
}