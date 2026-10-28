# Instructions d'Installation - Application Gestion Prestataires

## Architecture

### Backend - C# .NET 8
- **Clean Architecture** avec 4 couches (Domain, Application, Infrastructure, API)
- **CQRS Pattern** avec MediatR
- **Authentification JWT** avec BCrypt
- **Base de données PostgreSQL LOCALE**

### Frontend - Angular 21
- **Bootstrap 5** uniquement (PAS Angular Material)
- **Authentification JWT** (Login/Register)
- **Guards** pour protéger les routes

## Installation

### 1. Base de données PostgreSQL locale

1. Installer PostgreSQL sur votre machine
2. Exécuter le script SQL :

```bash
psql -U postgres -f database/schema.sql
```

Ce script crée :
- La base `gestion_prestataires`
- Les tables : `users`, `roles`, `prestataires`, `types_contrat`, `contrats`, `jours_travail`
- Les rôles par défaut : Admin, Manager, User
- Les types de contrat : Freelance, CDI, CDD, Portage salarial, ESN
- Les triggers automatiques

### 2. Backend C# .NET

1. Installer .NET 8 SDK

2. Modifier la connexion dans `backend/GestionPrestataires.API/appsettings.json` :
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=gestion_prestataires;Username=postgres;Password=VOTRE_PASSWORD"
  }
}
```

3. Lancer l'API :
```bash
cd backend/GestionPrestataires.API
dotnet restore
dotnet run
```

L'API sera sur **http://localhost:5000**

### 3. Frontend Angular

1. Installer Node.js et npm

2. Installer les dépendances :
```bash
cd frontend/gestion-prestataires
npm install
```

3. Lancer l'application :
```bash
npm start
```

L'application sera sur **http://localhost:4200**

## Endpoints API

### Authentification
- `POST /api/authentication/login` - Connexion
- `POST /api/authentication/register` - Inscription

### Prestataires
- `GET /api/prestataires` - Liste
- `GET /api/prestataires/{id}` - Détails
- `POST /api/prestataires` - Créer
- `PUT /api/prestataires/{id}` - Modifier
- `DELETE /api/prestataires/{id}` - Supprimer

### Contrats
- `GET /api/contrats` - Liste
- `POST /api/contrats` - Créer

### Jours de travail
- `GET /api/jourstravail` - Liste avec filtres

### Types de contrat
- `GET /api/typescontrat` - Liste

## Fonctionnalités

### Authentification
- **Inscription** avec email, mot de passe, prénom, nom
- **Connexion** avec JWT token
- **Rôles** : Admin, Manager, User
- **Déconnexion**

### Gestion des prestataires
- CRUD complet
- TJM (Taux Journalier Moyen)

### Gestion des contrats
- Création de contrats
- Calcul automatique du nombre de jours ouvrés
- Calcul automatique du montant total

### Gestion des jours de travail
- Enregistrement des journées
- Filtres par prestataire, année, mois

## Design

- **Bootstrap 5** pour tous les composants
- **Bootstrap Icons** pour les icônes
- Design responsive
- Composants : Login, Register, Dashboard

## Structure

```
backend/
├── GestionPrestataires.Domain/          # Entités + Interfaces
├── GestionPrestataires.Application/     # CQRS Commands/Queries
├── GestionPrestataires.Infrastructure/  # Repositories + DbContext
└── GestionPrestataires.API/             # Controllers + JWT

frontend/gestion-prestataires/
├── src/app/
│   ├── models/                          # User, Login, Register
│   ├── services/                        # AuthService
│   ├── guards/                          # authGuard
│   └── components/                      # Login, Register, Dashboard (Bootstrap)

database/
└── schema.sql                           # Script de création
```

## Important

- ✅ Base de données **LOCALE PostgreSQL** (pas Supabase)
- ✅ Frontend avec **BOOTSTRAP 5** uniquement
- ✅ Authentification **JWT + BCrypt**
- ✅ Clean Architecture + CQRS
- ✅ Tables users et roles complètes

## Premiers pas

1. Créer la base de données avec `schema.sql`
2. Lancer le backend .NET
3. Lancer le frontend Angular
4. S'inscrire sur **http://localhost:4200/register**
5. Se connecter et accéder au dashboard
