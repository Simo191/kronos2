import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-logo-prestapro',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="logo-prestapro" [class.logo-mini]="mini">
      <img
        class="logo-img"
        [class.logo-img-mini]="mini"
        [src]="src"
        alt="KRONOS Logo"
      />
    </div>

 
  `,
  styles: [`
    .logo-prestapro{
      display:flex;
      align-items:center;
      justify-content:center;
      width:64px;
      height:64px;
    }
    .logo-mini{ width:42px; height:74px; margin-top:-4px !important;}

    .logo-img{
      width:264px;
      height:264px;
      object-fit:contain;
      display:block;
    }
    .logo-img-mini{ width:90px; height:90px; margin-top:-4px !important; 
      /* filter: brightness(0) invert(1); */
    }

    .logo-text{
      margin-top:40px;
      font-weight:700;
      font-size:14px;
      letter-spacing:1px;
      text-transform:uppercase;
      text-align:center;
    }
    .logo-text-mini{ font-size:12px; }
  `]
})
export class LogoPrestaPro {
  @Input() mini = false;
  @Input() showText = true;
  @Input() src = '/Kronos1.png';
}
