import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ShiftService } from '../../services/shift.service';
import { TypeShift, ShiftCategory } from '../../models/Shift.model';

@Component({
  selector: 'app-type-shift-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './type-shift.component.html',
  styleUrls: ['./type-shift.component.scss']
})
export class TypeShiftComponent implements OnInit {
  typeShifts: TypeShift[] = [];
  showModal = false;
  modalMode: 'create' | 'edit' = 'create';
  selectedTypeShift: TypeShift | null = null;
  
  // Vue mode
  viewMode: 'list' | 'cards' = 'cards';

  // Formulaire
  formLabel = '';
  formShortLabel = '';
  formCode = '';
  formOrder = 1;
  formColor = '#000000';
  formBgColor = '';
  formBorderColor = '';
  formTextColor = '';
  formCategory = ShiftCategory.Travail; // ✅ NOUVEAU
  
  // Enum pour le template
  ShiftCategory = ShiftCategory; // ✅ NOUVEAU
  
  // Pour le sélecteur de couleur
  selectedColorPreset: any = null;
  
  predefinedColors = [
    { name: 'Vert', color: '#4ade80', bg: 'bg-green-200', border: 'border-green-400', text: 'text-green-900' },
    { name: 'Jaune', color: '#facc15', bg: 'bg-yellow-200', border: 'border-yellow-400', text: 'text-yellow-900' },
    { name: 'Bleu', color: '#60a5fa', bg: 'bg-blue-200', border: 'border-blue-400', text: 'text-blue-900' },
    { name: 'Rouge', color: '#f87171', bg: 'bg-red-200', border: 'border-red-400', text: 'text-red-900' },
    { name: 'Gris', color: '#9ca3af', bg: 'bg-gray-200', border: 'border-gray-400', text: 'text-gray-900' },
    { name: 'Violet', color: '#c084fc', bg: 'bg-purple-200', border: 'border-purple-400', text: 'text-purple-900' },
    { name: 'Orange', color: '#fb923c', bg: 'bg-orange-200', border: 'border-orange-400', text: 'text-orange-900' },
    { name: 'Émeraude', color: '#047857', bg: 'bg-emerald-700', border: 'border-emerald-800', text: 'text-white' },
    { name: 'Rose', color: '#f472b6', bg: 'bg-pink-200', border: 'border-pink-400', text: 'text-pink-900' },
    { name: 'Indigo', color: '#818cf8', bg: 'bg-indigo-200', border: 'border-indigo-400', text: 'text-indigo-900' }
  ];

  constructor(private shiftService: ShiftService ,private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadTypeShifts();
  }

  loadTypeShifts(): void {
    this.shiftService.getAllTypeShifts(false).subscribe({
      next: (types) => {
        this.typeShifts = types.sort((a, b) => a.order - b.order);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Erreur chargement types:', err);
        this.cdr.detectChanges();
      }
    });
  }

  openCreateModal(): void {
    this.modalMode = 'create';
    this.resetForm();
    this.showModal = true;
  }

  openEditModal(typeShift: TypeShift): void {
    this.modalMode = 'edit';
    this.selectedTypeShift = typeShift;
    
    this.formLabel = typeShift.label;
    this.formShortLabel = typeShift.shortLabel;
    this.formCode = typeShift.code;
    this.formOrder = typeShift.order;
    this.formColor = typeShift.color;
    this.formBgColor = typeShift.bgColor;
    this.formBorderColor = typeShift.borderColor;
    this.formTextColor = typeShift.textColor;
    this.formCategory = typeShift.category; // ✅ NOUVEAU
    
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.selectedTypeShift = null;
    this.resetForm();
  }

  resetForm(): void {
    this.formLabel = '';
    this.formShortLabel = '';
    this.formCode = '';
    this.formOrder = this.typeShifts.length + 1;
    this.formColor = '#000000';
    this.formBgColor = '';
    this.formBorderColor = '';
    this.formTextColor = '';
    this.formCategory = ShiftCategory.Travail; // ✅ NOUVEAU
    this.selectedColorPreset = null;
  }

  saveTypeShift(): void {
    if (!this.validateForm()) {
      alert('Veuillez remplir tous les champs obligatoires');
      return;
    }

    const data = {
      label: this.formLabel,
      shortLabel: this.formShortLabel,
      code: this.formCode,
      order: this.formOrder,
      color: this.formColor,
      bgColor: this.formBgColor,
      borderColor: this.formBorderColor,
      textColor: this.formTextColor,
      category: this.formCategory, // ✅ NOUVEAU
      isActive: true
    };

    if (this.modalMode === 'create') {
      this.shiftService.createTypeShift(data).subscribe({
        next: () => {
          this.closeModal();
          this.loadTypeShifts();
        },
        error: (err) => {
          console.error('Erreur création:', err);
          alert('Erreur lors de la création');
        }
      });
    } else if (this.selectedTypeShift) {
      this.shiftService.updateTypeShift(this.selectedTypeShift.id, data).subscribe({
        next: () => {
          this.closeModal();
          this.loadTypeShifts();
        },
        error: (err) => {
          console.error('Erreur modification:', err);
          alert('Erreur lors de la modification');
        }
      });
    }
  }

  deleteTypeShift(typeShift: TypeShift): void {
    if (confirm(`Êtes-vous sûr de vouloir supprimer "${typeShift.label}" ?`)) {
      this.shiftService.deleteTypeShift(typeShift.id).subscribe({
        next: () => {
          this.loadTypeShifts();
        },
        error: (err) => {
          console.error('Erreur suppression:', err);
          alert('Erreur lors de la suppression');
        }
      });
    }
  }

  moveUp(typeShift: TypeShift): void {
    const index = this.typeShifts.indexOf(typeShift);
    if (index > 0) {
      this.swapOrders(this.typeShifts[index - 1], typeShift);
    }
  }

  moveDown(typeShift: TypeShift): void {
    const index = this.typeShifts.indexOf(typeShift);
    if (index < this.typeShifts.length - 1) {
      this.swapOrders(typeShift, this.typeShifts[index + 1]);
    }
  }

  swapOrders(type1: TypeShift, type2: TypeShift): void {
    const tempOrder = type1.order;
    
    this.shiftService.updateTypeShift(type1.id, { order: type2.order }).subscribe({
      next: () => {
        this.shiftService.updateTypeShift(type2.id, { order: tempOrder }).subscribe({
          next: () => {
            this.loadTypeShifts();
          }
        });
      }
    });
  }

  selectColorPreset(preset: any): void {
    this.selectedColorPreset = preset;
    this.formColor = preset.color;
    this.formBgColor = preset.bg;
    this.formBorderColor = preset.border;
    this.formTextColor = preset.text;
  }

  // ✅ Quand couleur custom change → vider les classes CSS (elles seraient fausses)
  onCustomColorChange(): void {
    this.selectedColorPreset = null;
    this.formBgColor = '';
    this.formBorderColor = '';
    this.formTextColor = '';
  }

  // ✅ Style inline pour l'aperçu quand couleur personnalisée
  getPreviewStyle(): { [key: string]: string } {
    if (this.formBgColor || this.formBorderColor || this.formTextColor) return {};
    if (!this.formColor || this.formColor === '#000000') return {};
    const hex = this.formColor;
    const r = parseInt(hex.slice(1, 3), 16) || 0;
    const g = parseInt(hex.slice(3, 5), 16) || 0;
    const b = parseInt(hex.slice(5, 7), 16) || 0;
    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
    return {
      'background-color': hex,
      'border-color': hex,
      'color': luminance > 0.5 ? '#1a1a1a' : '#ffffff',
      'border-width': '2px',
      'border-style': 'solid'
    };
  }

  generateShortLabel(): void {
    if (this.formLabel && !this.formShortLabel) {
      this.formShortLabel = this.formLabel.substring(0, 6) + '...';
    }
  }

  generateCode(): void {
    if (this.formLabel && !this.formCode) {
      this.formCode = this.formLabel
        .toUpperCase()
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .replace(/[^A-Z0-9]/g, '_');
    }
  }

  validateForm(): boolean {
    return !!(this.formLabel && this.formShortLabel && this.formCode && this.formOrder);
  }
  
  // ✅ NOUVEAU : Retourne le label de la catégorie
  getCategoryLabel(category: ShiftCategory): string {
    switch(category) {
      case ShiftCategory.Travail: return 'Travail';
      case ShiftCategory.Absence: return 'Absence';
      case ShiftCategory.Conge: return 'Congé';
      default: return 'Travail';
    }
  }
  get activeTypeShifts() {
    return (this.typeShifts ?? []).filter(t => t.isActive);
  }
  
  get activeTypeShiftsCount(): number {
    return this.activeTypeShifts.length;
  }

  toggleViewMode(mode: 'list' | 'cards') {
    this.viewMode = mode;
  }

  // ✅ Style inline pour les cards de type existants (même logique)
  getPreviewStyleForType(typeShift: any): { [key: string]: string } {
    if (typeShift.bgColor || typeShift.borderColor || typeShift.textColor) return {};
    if (!typeShift.color) return {};
    const hex = typeShift.color;
    const r = parseInt(hex.slice(1, 3), 16) || 0;
    const g = parseInt(hex.slice(3, 5), 16) || 0;
    const b = parseInt(hex.slice(5, 7), 16) || 0;
    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
    return {
      'background-color': hex,
      'border-color': hex,
      'color': luminance > 0.5 ? '#1a1a1a' : '#ffffff',
      'border-width': '2px',
      'border-style': 'solid'
    };
  }
}