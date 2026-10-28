using GestionPrestataires.Domain.Entities;
using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestataires.Infrastructure.Repositories;

public class CagnotteRepository : ICagnotteRepository
{
    private readonly AppDbContext _context;

    public CagnotteRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère toutes les cagnottes avec leurs sources de budget
    /// </summary>
    public async Task<List<Cagnotte>> GetAllAsync()
    {
        return await _context.Cagnottes
            .Include(c => c.SourcesBudget) //   IMPORTANT pour calculer BudgetAnnuel
            .OrderByDescending(c => c.Annee)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère une cagnotte par ID avec ses sources
    /// </summary>
    public async Task<Cagnotte?> GetByIdAsync(Guid id)
    {
        return await _context.Cagnottes
            .Include(c => c.SourcesBudget) //   IMPORTANT
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Récupère une cagnotte par année avec ses sources
    /// </summary>
    public async Task<Cagnotte?> GetByAnneeAsync(int annee)
    {
        return await _context.Cagnottes
            .Include(c => c.SourcesBudget) //   IMPORTANT
            .FirstOrDefaultAsync(c => c.Annee == annee);
    }

    /// <summary>
    /// Ajoute une nouvelle cagnotte
    /// </summary>
    public async Task<Cagnotte> AddAsync(Cagnotte cagnotte)
    {
        cagnotte.DateCreation = DateTime.UtcNow;
        cagnotte.DateModification = DateTime.UtcNow;

        _context.Cagnottes.Add(cagnotte);
        await _context.SaveChangesAsync();

        return cagnotte;
    }

    /// <summary>
    /// Met à jour une cagnotte existante
    /// </summary>
    public async Task UpdateAsync(Cagnotte cagnotte)
    {
        cagnotte.DateModification = DateTime.UtcNow;

        _context.Cagnottes.Update(cagnotte);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Supprime une cagnotte (et ses sources en cascade)
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var cagnotte = await GetByIdAsync(id);
        if (cagnotte != null)
        {
            _context.Cagnottes.Remove(cagnotte);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Calcule le budget consommé pour une année donnée
    /// (Somme des jours de travail × TJM)
    /// </summary>
    //public async Task<decimal> CalculerBudgetConsommeAsync(int annee)
    //{
    //    // Cette requête calcule le budget consommé depuis la table JoursTravail
    //    var budgetConsomme = await _context.JoursTravail
    //        .Where(j => j.Mois.StartsWith(annee.ToString()))
    //        .SumAsync(j => (decimal)j.NombreJours * j.Tjm);

    //    return budgetConsomme;
    //}

    public async Task<decimal> CalculerBudgetConsommeAsync(int Annee)
    {
        var debutAnnee = new DateTime(Annee, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var finAnnee = new DateTime(Annee, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        var totalConsommed = await _context.JoursTravail
             .Where(j => j.DateTravail >= debutAnnee && j.DateTravail <= finAnnee).ToListAsync();


        //var totalConsomme = await _context.JoursTravail
        //    .Where(j => j.Mois == mois)
        //    .SumAsync(j => j.NombreJours * j.Tjm);
        var totalConsomme = await _context.JoursTravail
            .Where(j => j.DateTravail >= debutAnnee && j.DateTravail <= finAnnee)
            .SumAsync(j => j.NombreJours * j.Tjm);

        return totalConsomme;
    }
}