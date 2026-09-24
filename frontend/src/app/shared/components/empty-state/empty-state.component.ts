import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: false,
  templateUrl: './empty-state.component.html',
  styleUrl: './empty-state.component.scss'
})
export class EmptyStateComponent {
  @Input() title = 'No hay credenciales emitidas';
  @Input() subtitle = 'Comenzá dando de alta la primera credencial de socio.';
}
