namespace GestionPrestataires.Application.Contrats;

//   Proposition automatique de la répartition annuelle d'un contrat :
//   les jours du contrat sont ventilés au prorata des jours ouvrés (lun-ven hors fériés FR)
//   de chaque année civile couverte par la période. Résultat en jours entiers (méthode
//   du plus fort reste) ; une éventuelle partie décimale va sur la dernière année.
public static class RepartitionAnnuelleCalculator
{
    public static List<(int Annee, decimal NombreJours)> Proposer(DateTime dateDebut, DateTime dateFin, decimal nombreJours)
    {
        var debut = dateDebut.Date;
        var fin = dateFin.Date;
        if (fin < debut || nombreJours <= 0) return new();
        //   Garde-fou : période manifestement erronée (date en cours de saisie, etc.)
        if (fin.Year - debut.Year > 20) return new() { (debut.Year, nombreJours) };

        var annees = Enumerable.Range(debut.Year, fin.Year - debut.Year + 1).ToList();
        if (annees.Count == 1) return new() { (annees[0], nombreJours) };

        var ouvres = annees.Select(a =>
        {
            var d = a == debut.Year ? debut : new DateTime(a, 1, 1);
            var f = a == fin.Year ? fin : new DateTime(a, 12, 31);
            return CompterJoursOuvres(d, f);
        }).ToList();

        var totalOuvres = ouvres.Sum();
        if (totalOuvres == 0) return new() { (annees[0], nombreJours) };

        var entiers = Math.Floor(nombreJours);
        var parts = ouvres.Select(o => entiers * o / totalOuvres).ToList();
        var alloues = parts.Select(p => Math.Floor(p)).ToList();

        //   Plus fort reste : distribue les jours entiers restants
        var reste = (int)(entiers - alloues.Sum());
        foreach (var i in parts.Select((p, i) => (Reste: p - Math.Floor(p), Index: i))
                               .OrderByDescending(x => x.Reste).ThenBy(x => x.Index)
                               .Take(reste).Select(x => x.Index))
        {
            alloues[i] += 1;
        }

        alloues[^1] += nombreJours - entiers;

        return annees.Select((a, i) => (a, alloues[i]))
                     .Where(x => x.Item2 > 0)
                     .ToList();
    }

    private static int CompterJoursOuvres(DateTime debut, DateTime fin)
    {
        var feries = new HashSet<DateTime>(JoursFeries(debut.Year).Concat(JoursFeries(fin.Year)));
        var n = 0;
        for (var d = debut; d <= fin; d = d.AddDays(1))
        {
            if (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;
            if (feries.Contains(d)) continue;
            n++;
        }
        return n;
    }

    //   Mêmes jours fériés que le formulaire contrat (frontend)
    private static IEnumerable<DateTime> JoursFeries(int annee)
    {
        var paques = DatePaques(annee);
        return new[]
        {
            new DateTime(annee, 1, 1), new DateTime(annee, 5, 1), new DateTime(annee, 5, 8),
            new DateTime(annee, 7, 14), new DateTime(annee, 8, 15), new DateTime(annee, 11, 1),
            new DateTime(annee, 11, 11), new DateTime(annee, 12, 25),
            paques.AddDays(1), paques.AddDays(39), paques.AddDays(50)
        };
    }

    private static DateTime DatePaques(int annee)
    {
        int a = annee % 19, b = annee / 100, c = annee % 100, d = b / 4, e = b % 4;
        int f = (b + 8) / 25, g = (b - f + 1) / 3, h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4, k = c % 4, l = (32 + 2 * e + 2 * i - h - k) % 7, m = (a + 11 * h + 22 * l) / 451;
        int mois = (h + l - 7 * m + 114) / 31, jour = (h + l - 7 * m + 114) % 31 + 1;
        return new DateTime(annee, mois, jour);
    }
}
