# Application de Gestion des Prestataires

Application professionnelle avec **Clean Architecture** et **CQRS Pattern** pour gérer les prestataires, leurs contrats et leurs jours de travail.

## Architecture

### Backend
- **Clean Architecture** avec séparation des couches (Domain, Application, Infrastructure, API)
- **CQRS Pattern** avec MediatR
- **C# .NET 8** - API REST
- **Entity Framework Core** - ORM
- **FluentValidation** - Validation des commandes

### Couches

#### 1. Domain Layer
- Entités métier (Prestataire, Contrat, TypeContrat, JourTravail)
- Interfaces des repositories
- Logique métier pure, sans dépendances

#### 2. Application Layer
- **Commands** (Create, Update, Delete) avec handlers
- **Queries** (GetAll, GetById) avec handlers
- **MediatR** pour le pattern CQRS
- Aucune dépendance vers l'infrastructure

#### 3. Infrastructure Layer
- Implémentation des repositories
- DbContext Entity Framework
- Configuration de la base de données PostgreSQL/Supabase

#### 4. API Layer
- Contrôleurs minimalistes qui délèguent à MediatR
- Configuration CORS
- Swagger/OpenAPI

### Frontend
- **Angular 21** avec TypeScript
- **Authentification Supabase** (email/password)
- **Guards** pour protéger les routes
- **Services** avec HttpClient et RxJS
- **Modèles** TypeScript
- **Composants** Login, Register, Dashboard

### Base de données
- **PostgreSQL** (Supabase)
- **Supabase Auth** - Authentification intégrée
- Triggers pour calculs automatiques
- Row Level Security (RLS)

## Configuration

### 1. Base de données Supabase

La base de données est déjà configurée avec les tables suivantes :
- `prestataires`
- `types_contrat`
- `contrats`
- `jours_travail`

### 2. Configuration Backend (C# .NET)

1. Installer .NET 8 SDK
2. Configurer la chaîne de connexion dans `backend/GestionPrestataires.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=VOTRE_HOST_SUPABASE;Database=postgres;Username=postgres;Password=VOTRE_MOT_DE_PASSE"
  }
}
```

3. Lancer l'API:
```bash
cd backend/GestionPrestataires.API
dotnet restore
dotnet run
```

L'API sera disponible sur http://localhost:5000

### 3. Configuration Frontend (Angular)

1. Installer Node.js et npm
2. Configurer Supabase dans `src/app/services/supabase.service.ts`:
```typescript
this.supabase = createClient(
  'YOUR_SUPABASE_URL',        // URL de votre projet Supabase
  'YOUR_SUPABASE_ANON_KEY'    // Clé anon de votre projet
);
```

3. Installer les dépendances:
```bash
cd frontend/gestion-prestataires
npm install
```

4. Lancer l'application:
```bash
npm start
```

L'application sera disponible sur http://localhost:4200

### 4. Routes de l'application

- `/login` - Page de connexion
- `/register` - Page d'inscription
- `/dashboard` - Tableau de bord (protégé par authGuard)

## Fonctionnalités

### 1. Gestion des Prestataires
- Créer, modifier, supprimer des prestataires
- Informations: Nom, Prénom, Email, Téléphone, TJM

### 2. Types de Contrat
- Gérer les types de contrat (Freelance, CDI, CDD, Portage, ESN)

### 3. Gestion des Contrats
- Créer des contrats pour les prestataires
- Calcul automatique du nombre de jours ouvrés
- Calcul automatique du montant total
- Liaison avec prestataire et type de contrat

### 4. Jours de Travail
- Enregistrer les journées travaillées
- Filtres par prestataire et par mois
- Statistiques (total jours, montant total)
- Commentaires et projets optionnels

### 5. Tableau de Bord
- Vue calendrier annuelle
- Visualisation des jours travaillés par prestataire
- Statistiques globales
- Navigation entre les années

## API Endpoints

### Prestataires
- GET /api/prestataires - Liste tous les prestataires
- GET /api/prestataires/{id} - Détails d'un prestataire
- POST /api/prestataires - Créer un prestataire
- PUT /api/prestataires/{id} - Modifier un prestataire
- DELETE /api/prestataires/{id} - Supprimer un prestataire

### Types de Contrat
- GET /api/typescontrat - Liste tous les types
- POST /api/typescontrat - Créer un type
- PUT /api/typescontrat/{id} - Modifier un type
- DELETE /api/typescontrat/{id} - Supprimer un type

### Contrats
- GET /api/contrats - Liste tous les contrats
- POST /api/contrats - Créer un contrat
- PUT /api/contrats/{id} - Modifier un contrat
- DELETE /api/contrats/{id} - Supprimer un contrat

### Jours de Travail
- GET /api/jourstravail?prestataireId=xxx&annee=2025 - Liste avec filtres
- POST /api/jourstravail - Créer un jour de travail
- PUT /api/jourstravail/{id} - Modifier un jour
- DELETE /api/jourstravail/{id} - Supprimer un jour

## Structure du Projet (Clean Architecture)

```
backend/
├── GestionPrestataires.sln
│
├── GestionPrestataires.Domain/              # Couche Domain
│   ├── Entities/
│   │   ├── Prestataire.cs
│   │   ├── TypeContrat.cs
│   │   ├── Contrat.cs
│   │   └── JourTravail.cs
│   └── Repositories/
│       ├── IPrestataireRepository.cs
│       ├── ITypeContratRepository.cs
│       ├── IContratRepository.cs
│       └── IJourTravailRepository.cs
│
├── GestionPrestataires.Application/         # Couche Application (CQRS)
│   ├── Prestataires/
│   │   ├── Commands/
│   │   │   ├── CreatePrestataire/
│   │   │   │   ├── CreatePrestataireCommand.cs
│   │   │   │   └── CreatePrestataireCommandHandler.cs
│   │   │   ├── UpdatePrestataire/
│   │   │   └── DeletePrestataire/
│   │   └── Queries/
│   │       ├── GetAllPrestataires/
│   │       │   ├── GetAllPrestatairesQuery.cs
│   │       │   └── GetAllPrestatairesQueryHandler.cs
│   │       └── GetPrestataireById/
│   └── Contrats/
│       ├── Commands/
│       └── Queries/
│
├── GestionPrestataires.Infrastructure/      # Couche Infrastructure
│   ├── Data/
│   │   └── AppDbContext.cs
│   └── Repositories/
│       ├── PrestataireRepository.cs
│       ├── TypeContratRepository.cs
│       ├── ContratRepository.cs
│       └── JourTravailRepository.cs
│
└── GestionPrestataires.API/                 # Couche Présentation
    ├── Controllers/
    │   ├── PrestatairesController.cs        # Utilise MediatR
    │   ├── TypesContratController.cs
    │   ├── ContratsController.cs
    │   └── JoursTravailController.cs
    ├── Program.cs                           # DI Configuration
    └── appsettings.json

frontend/
└── gestion-prestataires/
    └── src/app/
        ├── models/
        │   ├── prestataire.model.ts
        │   ├── type-contrat.model.ts
        │   ├── contrat.model.ts
        │   └── jour-travail.model.ts
        └── services/
            ├── prestataire.service.ts
            ├── type-contrat.service.ts
            ├── contrat.service.ts
            └── jour-travail.service.ts
```

## Technologies Utilisées

### Backend
- **ASP.NET Core 8.0** - Framework web
- **Entity Framework Core 8.0** - ORM
- **Npgsql** - Driver PostgreSQL
- **MediatR 12.4** - Pattern Mediator pour CQRS
- **FluentValidation 11.10** - Validation
- **Swashbuckle** - Documentation Swagger

### Frontend
- **Angular 21** - Framework frontend
- **TypeScript** - Langage typé
- **RxJS** - Programmation réactive
- **HttpClient** - Client HTTP Angular

### Base de données
- **PostgreSQL** (Supabase)
- Triggers pour calculs automatiques
- Row Level Security (RLS)

## Pattern CQRS

### Commands (Écriture)

Les **Commands** modifient l'état du système :

```csharp
// Créer un prestataire
var command = new CreatePrestataireCommand(
    "Dupont",
    "Jean",
    "jean.dupont@example.com",
    "0123456789",
    450.00m
);
var prestataireId = await _mediator.Send(command);
```

### Queries (Lecture)

Les **Queries** lisent les données sans les modifier :

```csharp
// Récupérer tous les prestataires
var query = new GetAllPrestatairesQuery();
var prestataires = await _mediator.Send(query);
```

## Avantages de l'Architecture

### Clean Architecture
- **Séparation des responsabilités** - Chaque couche a un rôle précis
- **Testabilité** - Facile de tester chaque couche indépendamment
- **Maintenabilité** - Code organisé et facile à maintenir
- **Indépendance** - Le domaine ne dépend d'aucune infrastructure
- **Évolutivité** - Facile d'ajouter de nouvelles fonctionnalités

### CQRS Pattern
- **Séparation lecture/écriture** - Optimisation différente pour chaque besoin
- **Scalabilité** - Peut scaler les lectures et écritures indépendamment
- **Performance** - Requêtes optimisées pour chaque cas d'usage
- **Clarté** - Intentions claires (Command vs Query)
- **Extensibilité** - Facile d'ajouter des handlers et validations
