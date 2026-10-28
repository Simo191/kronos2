# Déploiement Kronos — 27/09/2026

## Contenu du paquet

| Dossier | Contenu |
|---|---|
| `1_base-de-donnees` | `00_verification_avant.sql`, `01_migration_AddSuiviCommandes.sql`, `02_verification_apres.sql`, `99_retour_arriere_AddSuiviCommandes.sql` |
| `2_backend` | API .NET 8 publiée en Release (sans `appsettings.Development.json`, sans `wwwroot` local) |
| `3_frontend` | Angular en production (API : `http://x72sdeigfze:8035/api`) + `web.config` IIS du serveur |

Nouveautés : page **Suivi des commandes**, nouveaux champs du contrat (n° commande, CTR, tranche ferme/option,
logiciel, commentaire, répartition par année) et correctif **Planning** (le glisser-déposer ne supprime plus les serveurs).

> ⚠️ Ne **jamais** lancer `dotnet ef database update` sur la prod : les anciennes migrations écraseraient les mots de
> passe des comptes. Utiliser uniquement les scripts SQL de ce dossier.

## Étapes

### 1. Sauvegardes (sur le serveur)
- Base :
  `pg_dump -h localhost -U postgres -d gestion_prestataires -F c -f gestion_prestataires_avant_20260927.backup`
- Copier le dossier de l'API et le dossier du site Angular (ex. renommer en `*_bak_20260927`).

### 2. Base de données (pgAdmin → Query Tool sur `gestion_prestataires`, ou psql)
1. `00_verification_avant.sql` → la 1re ligne doit être `20260611233857_AddEntreprises` et le compteur `0`.
   Sinon : **s'arrêter**.
2. `01_migration_AddSuiviCommandes.sql` → doit se terminer sans erreur (`COMMIT`).
3. `02_verification_apres.sql` → `1`, 5 colonnes, `1`, tous les contrats en `Ferme`, mêmes compteurs qu'à l'étape 1.

### 3. Backend (API, port 8035)
1. Déposer un fichier `app_offline.htm` dans le dossier de l'API (ou arrêter le pool IIS) : libère les DLL.
2. Copier le contenu de `2_backend` dans le dossier de l'API en remplaçant les fichiers,
   **sauf `appsettings.json` et `web.config` : garder ceux du serveur** (aucun changement de configuration dans cette version).
3. **Ne rien supprimer** dans `wwwroot` (documents des contrats, photos) ni `logs`.
4. Supprimer `app_offline.htm` (ou redémarrer le pool).
5. Test : `http://x72sdeigfze:8035/api/applicationsplanning` doit répondre **401** (API en ligne, authentification requise).

### 4. Frontend (site Angular, port 8036)
1. Supprimer les anciens fichiers du site **sauf `web.config`** (ancien `main-GTT7SZIH.js`, etc.).
2. Copier le contenu de `3_frontend`.

### 5. Vérifications fonctionnelles (Ctrl+F5 dans le navigateur)
- Connexion, tableau de bord, Contrats (ouvrir un contrat : nouveaux champs + répartition par année).
- Menu **Suivi commandes** (Admin/Manager) : vues Liste et Cartes.
- Planning : déplacer une application → ses serveurs sont toujours là.
- Jours travaillés / génération depuis le Shift : inchangés.

## Retour arrière
1. Remettre les dossiers `*_bak_20260927` de l'API et du site.
2. Base : exécuter `99_retour_arriere_AddSuiviCommandes.sql`
   (supprime uniquement les nouveaux champs et leurs données) — ou restaurer la sauvegarde `pg_dump`.
