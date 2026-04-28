import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Member } from '../../core/models';
import { ApiErrorService } from '../../core/services/api-error.service';
import { MemberContextService } from '../../core/services/member-context.service';
import { MembersApiService } from '../../core/services/members-api.service';

@Component({
  selector: 'app-member-page',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './member-page.component.html',
  styleUrl: './member-page.component.scss'
})
export class MemberPageComponent implements OnInit {
  readonly members = signal<Member[]>([]);
  readonly isLoading = signal(false);
  readonly isCreateInFlight = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  readonly fullName = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(120)]
  });

  constructor(
    private readonly membersApi: MembersApiService,
    private readonly memberContext: MemberContextService,
    private readonly apiError: ApiErrorService
  ) {}

  ngOnInit(): void {
    this.loadMembers();
  }

  isSelected(member: Member): boolean {
    return this.memberContext.selectedMember()?.id === member.id;
  }

  selectMember(member: Member): void {
    this.memberContext.setSelectedMember(member);
    this.successMessage.set(`Selected ${member.fullName}.`);
  }

  onSubmit(event: Event): void {
    event.preventDefault();
    this.createMember();
  }

  createMember(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.fullName.markAsTouched();
    this.fullName.updateValueAndValidity();

    const fullName = this.fullName.value.trim();
    if (!fullName || this.fullName.invalid) {
      return;
    }

    this.isCreateInFlight.set(true);
    this.membersApi.createMember({ fullName }).subscribe({
      next: (member) => {
        this.memberContext.setSelectedMember(member);
        this.successMessage.set(`Registered ${member.fullName} and selected as active member.`);
        this.fullName.reset('');
        this.isCreateInFlight.set(false);
        this.loadMembers();
      },
      error: (error: unknown) => {
        this.errorMessage.set(this.apiError.map(error));
        this.isCreateInFlight.set(false);
      }
    });
  }

  private loadMembers(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.membersApi.getMembers().subscribe({
      next: (members) => {
        this.members.set(members);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.errorMessage.set(this.apiError.map(error));
        this.isLoading.set(false);
      }
    });
  }
}
