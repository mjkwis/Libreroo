import { Injectable, signal } from '@angular/core';
import { Member, SelectedMember } from '../models';

const STORAGE_KEY = 'libreroo.selected-member';

@Injectable({ providedIn: 'root' })
export class MemberContextService {
  private readonly selectedMemberSignal = signal<SelectedMember | null>(this.readFromStorage());

  selectedMember(): SelectedMember | null {
    return this.selectedMemberSignal();
  }

  setSelectedMember(member: Member): void {
    const selectedMember = { id: member.id, fullName: member.fullName };
    this.selectedMemberSignal.set(selectedMember);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(selectedMember));
  }

  clear(): void {
    this.selectedMemberSignal.set(null);
    localStorage.removeItem(STORAGE_KEY);
  }

  private readFromStorage(): SelectedMember | null {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (!stored) {
      return null;
    }

    try {
      const parsed = JSON.parse(stored);
      if (this.isSelectedMember(parsed)) {
        return parsed;
      }
    } catch {
      return null;
    }

    return null;
  }

  private isSelectedMember(value: unknown): value is SelectedMember {
    return (
      typeof value === 'object' &&
      value !== null &&
      'id' in value &&
      typeof value.id === 'number' &&
      'fullName' in value &&
      typeof value.fullName === 'string'
    );
  }
}
