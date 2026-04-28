import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/services/api-error.service';
import { MemberContextService } from '../../core/services/member-context.service';
import { MembersApiService } from '../../core/services/members-api.service';
import { MemberPageComponent } from './member-page.component';

describe('MemberPageComponent', () => {
  let fixture: ComponentFixture<MemberPageComponent>;
  let membersApi: jasmine.SpyObj<MembersApiService>;
  let memberContext: jasmine.SpyObj<MemberContextService>;

  beforeEach(async () => {
    membersApi = jasmine.createSpyObj<MembersApiService>('MembersApiService', ['getMembers', 'createMember']);
    memberContext = jasmine.createSpyObj<MemberContextService>('MemberContextService', [
      'selectedMember',
      'setSelectedMember'
    ]);
    memberContext.selectedMember.and.returnValue(null);

    membersApi.getMembers.and.returnValues(
      of([{ id: 1, fullName: 'Existing Member' }]),
      of([
        { id: 1, fullName: 'Existing Member' },
        { id: 2, fullName: 'New Member' }
      ])
    );
    membersApi.createMember.and.returnValue(of({ id: 2, fullName: 'New Member' }));

    await TestBed.configureTestingModule({
      imports: [MemberPageComponent],
      providers: [
        { provide: MembersApiService, useValue: membersApi },
        { provide: MemberContextService, useValue: memberContext },
        ApiErrorService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MemberPageComponent);
    fixture.detectChanges();
  });

  it('should not submit quick registration when full name is empty', () => {
    const form = fixture.nativeElement.querySelector('form') as HTMLFormElement;
    form.dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    expect(membersApi.createMember).not.toHaveBeenCalled();
  });

  it('should register and auto-select a newly created member', () => {
    fixture.componentInstance.fullName.setValue('New Member');
    fixture.componentInstance.createMember();

    expect(membersApi.createMember).toHaveBeenCalledWith({ fullName: 'New Member' });
    expect(memberContext.setSelectedMember).toHaveBeenCalledWith({ id: 2, fullName: 'New Member' });
  });

  it('should submit quick registration without reloading the page', () => {
    fixture.componentInstance.fullName.setValue('New Member');
    fixture.detectChanges();

    const form = fixture.nativeElement.querySelector('form') as HTMLFormElement;
    form.dispatchEvent(new Event('submit', { cancelable: true }));
    fixture.detectChanges();

    expect(membersApi.createMember).toHaveBeenCalledWith({ fullName: 'New Member' });
  });
});
