import { TestBed } from '@angular/core/testing';
import { MemberContextService } from './member-context.service';

describe('MemberContextService', () => {
  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [MemberContextService]
    });
  });

  it('should persist selected member identity to local storage', () => {
    const service = TestBed.inject(MemberContextService);
    service.setSelectedMember({ id: 9, fullName: 'Grace Hopper' });

    expect(service.selectedMember()).toEqual({ id: 9, fullName: 'Grace Hopper' });
    expect(localStorage.getItem('libreroo.selected-member')).toBe('{"id":9,"fullName":"Grace Hopper"}');
  });

  it('should restore selected member from local storage', () => {
    localStorage.setItem('libreroo.selected-member', '{"id":3,"fullName":"Katherine Johnson"}');
    const service = TestBed.inject(MemberContextService);

    expect(service.selectedMember()).toEqual({ id: 3, fullName: 'Katherine Johnson' });
  });
});
