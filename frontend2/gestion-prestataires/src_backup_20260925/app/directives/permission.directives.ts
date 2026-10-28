import { 
    Directive, 
    Input, 
    TemplateRef, 
    ViewContainerRef, 
    OnInit 
  } from '@angular/core';
  import { PermissionService, Permission, Role } from '../services/permission.service';
  
  /**
   * Directive pour afficher/masquer des éléments selon les permissions
   * 
   * Usage:
   * <div *hasPermission="'VIEW_PRESTATAIRES'">Contenu visible si permission</div>
   * <div *hasPermission="['VIEW_CONTRATS', 'MANAGE_CONTRATS']">Multiple permissions (OR)</div>
   * <div *hasPermission="['VIEW_CONTRATS', 'MANAGE_CONTRATS']; requireAll: true">Multiple (AND)</div>
   * <div *hasRole="'Admin'">Visible seulement pour Admin</div>
   */
  @Directive({
    selector: '[hasPermission]',
    standalone: true
  })
  export class HasPermissionDirective implements OnInit {
    @Input() hasPermission: Permission | Permission[] = [];
    @Input() requireAll = false; // Si true, toutes les permissions requises (AND), sinon (OR)
  
    constructor(
      private templateRef: TemplateRef<any>,
      private viewContainer: ViewContainerRef,
      private permissionService: PermissionService
    ) {}
  
    ngOnInit() {
      this.updateView();
    }
  
    private updateView() {
      const hasPermission = this.checkPermissions();
      
      if (hasPermission) {
        this.viewContainer.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainer.clear();
      }
    }
  
    private checkPermissions(): boolean {
      if (Array.isArray(this.hasPermission)) {
        return this.requireAll
          ? this.permissionService.hasAllPermissions(...this.hasPermission)
          : this.permissionService.hasAnyPermission(...this.hasPermission);
      } else {
        return this.permissionService.hasPermission(this.hasPermission);
      }
    }
  }
  
  /**
   * Directive pour afficher/masquer selon le rôle
   * 
   * Usage:
   * <div *hasRole="'Admin'">Visible seulement pour Admin</div>
   * <div *hasRole="['Admin', 'Manager']">Visible pour Admin OU Manager</div>
   */
  @Directive({
    selector: '[hasRole]',
    standalone: true
  })
  export class HasRoleDirective implements OnInit {
    @Input() hasRole: Role | Role[] = [];
  
    constructor(
      private templateRef: TemplateRef<any>,
      private viewContainer: ViewContainerRef,
      private permissionService: PermissionService
    ) {}
  
    ngOnInit() {
      this.updateView();
    }
  
    private updateView() {
      const hasRole = this.checkRoles();
      
      if (hasRole) {
        this.viewContainer.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainer.clear();
      }
    }
  
    private checkRoles(): boolean {
      if (Array.isArray(this.hasRole)) {
        return this.hasRole.some(role => this.permissionService.hasRole(role));
      } else {
        return this.permissionService.hasRole(this.hasRole);
      }
    }
  }
  
  /**
   * Directive inverse - masque si permission présente
   * 
   * Usage:
   * <div *lacksPermission="'VIEW_TJM'">Masquer TJM si pas permission</div>
   */
  @Directive({
    selector: '[lacksPermission]',
    standalone: true
  })
  export class LacksPermissionDirective implements OnInit {
    @Input() lacksPermission: Permission | Permission[] = [];
  
    constructor(
      private templateRef: TemplateRef<any>,
      private viewContainer: ViewContainerRef,
      private permissionService: PermissionService
    ) {}
  
    ngOnInit() {
      this.updateView();
    }
  
    private updateView() {
      const hasPermission = this.checkPermissions();
      
      // Inverse : affiche si PAS de permission
      if (!hasPermission) {
        this.viewContainer.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainer.clear();
      }
    }
  
    private checkPermissions(): boolean {
      if (Array.isArray(this.lacksPermission)) {
        return this.permissionService.hasAnyPermission(...this.lacksPermission);
      } else {
        return this.permissionService.hasPermission(this.lacksPermission);
      }
    }
  }