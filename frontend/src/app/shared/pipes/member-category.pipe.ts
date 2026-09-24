import { Pipe, PipeTransform } from '@angular/core';
import { MemberCategory } from '../../core/models/enums';

const LABELS: Record<MemberCategory, string> = {
  [MemberCategory.Adulto]: 'Adulto',
  [MemberCategory.Juvenil]: 'Juvenil',
  [MemberCategory.Nino]: 'Niño'
};

@Pipe({ name: 'memberCategory', standalone: false })
export class MemberCategoryPipe implements PipeTransform {
  transform(value: MemberCategory): string {
    return LABELS[value] ?? String(value);
  }
}