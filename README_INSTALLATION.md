# Guide d'Installation - Gestion des Prestataires

## 1. Base de données PostgreSQL

### Créer la base de données
```bash
psql -U postgres
CREATE DATABASE gestion_prestataires;
\q
```

### Exécuter le schéma
```bash
psql -U postgres -d gestion_prestataires -f database/schema.sql
```

### Ajouter les rôles par défaut
```bash
psql -U postgres -d gestion_prestataires -f database/seed_roles.sql
```

### Créer l'utilisateur admin (mot de passe: Admin123!)
```bash
psql -U postgres -d gestion_prestataires -f database/seed_admin_user.sql
```

## 2. Backend C# .NET

### Ouvrir dans Visual Studio
1. Ouvrir `backend/GestionPrestataires.sln`
2. Clic droit sur solution → Restore NuGet Packages
3. Mettre à jour la connection string dans `GestionPrestataires.API/appsettings.json`
4. Définir `GestionPrestataires.API` comme projet de démarrage
5. Appuyer sur F5

### Ou en ligne de commande
```bash
cd backend
dotnet restore
dotnet build
cd GestionPrestataires.API
dotnet run
```

L'API sera disponible sur : `http://localhost:5000`

## 3. Frontend Angular

```bash
cd frontend/gestion-prestataires
npm install
ng serve
```

L'application sera disponible sur : `http://localhost:4200`

## 4. Connexion

### Utilisateur Admin par défaut
- Email: `admin@gestion.com`
- Mot de passe: `Admin123!`

## Rôles disponibles
- **Admin** - Administrateur système avec tous les droits
- **Manager** - Gestionnaire avec droits de gestion
- **User** - Utilisateur standard

## Fonctionnalités
✅ Authentification JWT
✅ Gestion des Utilisateurs et Rôles
✅ Gestion des Prestataires
✅ Gestion des Contrats
✅ Gestion des Jours de Travail
✅ Clean Architecture + CQRS
✅ Interface Bootstrap 5
