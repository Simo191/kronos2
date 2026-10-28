import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { PrestatairesComponent } from './components/prestataires/prestataires.component';
import { ContratsComponent } from './components/contrats/contrats.component';
import { JoursTravailComponent } from './components/jours-travail/jours-travail.component';
import { TypesContratComponent } from './components/types-contrat/types-contrat.component';
import { CagnottesComponent } from './components/cagnottes/cagnottes.component';
import { UsersComponent } from './components/users/users.component';
import { RolesComponent } from './components/roles/roles.component';
import { ShiftPlanningComponent } from './components/shift-planning/shift-planning.component';
import { TypeShiftComponent } from './components/type-shift/type-shift.component';
 import { authGuard } from './guards/auth.guard';
import { permissionGuard } from './guards/permission.guard';
import { Permission } from './services/permission.service';
import { UnauthorizedComponent } from './components/Unauthorized/Unauthorized.component';
import { ProfilComponent } from './components/profil/profil.component';
import { GestionVerrouillageSComponent } from './components/gestion-verrouillages.component/gestion-verrouillages.component';
import { EstimationsComponent } from './components/estimations.component/estimations.component';
import { EstimationDetailComponent } from './components/estimation-detail.component/estimation-detail.component';
import { EstimationResultatComponent } from './components/estimation-resultat.component/estimation-resultat.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  
  // ✅ PAGE UNAUTHORIZED (accessible sans permission)
  { 
    path: 'unauthorized', 
    component: UnauthorizedComponent 
  },
  
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    runGuardsAndResolvers: 'always',
    children: [
      // ✅ DASHBOARD : Tous les rôles (pas de permission requise)
      { 
        path: 'dashboard', 
        component: DashboardComponent, 
        runGuardsAndResolvers: 'always' 
      },
      
      // ✅ PRESTATAIRES : Admin + Manager uniquement
      { 
        path: 'prestataires', 
        component: PrestatairesComponent,
        canActivate: [permissionGuard],
        data: { permission: Permission.VIEW_PRESTATAIRES },
        runGuardsAndResolvers: 'always' 
      },
      
      // ✅ CONTRATS : Tous (mais filtrés selon rôle)
      { 
        path: 'contrats', 
        component: ContratsComponent,
        canActivate: [permissionGuard],
        data: { permission: Permission.VIEW_CONTRATS },
        runGuardsAndResolvers: 'always' 
      },
      
      // ✅ JOURS TRAVAIL : Tous (mais filtrés selon rôle)
      { 
        path: 'jours-travail', 
        component: JoursTravailComponent,
        canActivate: [permissionGuard],
        data: { permission: Permission.VIEW_JOURS_TRAVAIL },
        runGuardsAndResolvers: 'always' 
      },
      // ✅ TYPES PLANNING : Admin uniquement
      { 
        path: 'gestion-verrouillages', 
        component: GestionVerrouillageSComponent,
        canActivate: [permissionGuard],
        data: { permission: Permission.VIEW_TYPE_PLANNING },
        runGuardsAndResolvers: 'always' 
      },
      // ✅ PLANNING : Tous (mais actions selon rôle)
      { 
        path: 'shift-planning', 
        component: ShiftPlanningComponent,
        canActivate: [permissionGuard],
        data: { permission: Permission.VIEW_PLANNING },
        runGuardsAndResolvers: 'always' 
      },
      
      // ✅ TYPES CONTRAT : Admin uniquement
      { 
        path: 'types-contrat', 
        component: TypesContratComponent,
        canActivate: [permissionGuard],
        data: { permission: Permission.VIEW_TYPE_CONTRAT },
        runGuardsAndResolvers: 'always' 
      },
      
      // ✅ TYPES PLANNING : Admin uniquement
      { 
        path: 'type-shift', 
        component: TypeShiftComponent,
        canActivate: [permissionGuard],
        data: { permission: Permission.VIEW_TYPE_PLANNING },
        runGuardsAndResolvers: 'always' 
      },
      
      // ✅ CAGNOTTES : Admin uniquement
      { 
        path: 'cagnottes', 
        component: CagnottesComponent,
        canActivate: [permissionGuard],
        data: { permission: Permission.VIEW_CAGNOTTES },
        runGuardsAndResolvers: 'always' 
      },
      
      // ✅ CAGNOTTES : Admin uniquement
      {
        path: 'estimations',
        children: [
          {
            path: '',
            component: EstimationsComponent  // Liste/Cartes
          },
          {
            path: 'nouveau',
            component: EstimationDetailComponent  // Création
          },
          {
            path: ':id/detail',
            component: EstimationDetailComponent  // Modification
          },
          {
            path: ':id/resultat',
            component: EstimationResultatComponent  // Dashboard
          }
        ]
      },

      // ✅ UTILISATEURS : Admin + Manager
      { 
        path: 'users', 
        component: UsersComponent,
        canActivate: [permissionGuard],
        data: { permission: Permission.VIEW_USERS },
        runGuardsAndResolvers: 'always' 
      },
      
      // ✅ RÔLES : Admin uniquement
      { 
        path: 'roles', 
        component: RolesComponent,
        canActivate: [permissionGuard],
        data: { permission: Permission.VIEW_ROLES },
        runGuardsAndResolvers: 'always' 
      },
      // ✅ RÔLES : Admin uniquement
      { 
        path: 'profil', 
        component: ProfilComponent,
        runGuardsAndResolvers: 'always' 
      },

      { 
        path: 'profil', 
        component: ProfilComponent,
        runGuardsAndResolvers: 'always' 
      },
    ]
  },
  { path: '**', redirectTo: '/login' }
];