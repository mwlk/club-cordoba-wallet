import { Component, Input } from '@angular/core';

// Avatar del socio: muestra la foto cuando la URL es válida y cae a las
// iniciales si no hay URL o la imagen falla al cargar. Se usa en el
// listado y en el carnet de detalle.
@Component({
  selector: 'app-member-avatar',
  standalone: false,
  templateUrl: './member-avatar.component.html',
  styleUrl: './member-avatar.component.scss'
})
export class MemberAvatarComponent {
  @Input({ required: true }) name = '';
  @Input() photo = '';
  @Input() size = 44;

  imageFailed = false;

  get initials(): string {
    return this.name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(part => part[0])
      .join('')
      .toUpperCase();
  }

  get showInitials(): boolean {
    return !this.photo || this.imageFailed;
  }
}