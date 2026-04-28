import { Routes } from '@angular/router';
import { CatalogPageComponent } from './features/catalog/catalog-page.component';
import { LoansPageComponent } from './features/loans/loans-page.component';
import { MemberPageComponent } from './features/member/member-page.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'catalog' },
  { path: 'catalog', component: CatalogPageComponent },
  { path: 'member', component: MemberPageComponent },
  { path: 'loans', component: LoansPageComponent },
  { path: '**', redirectTo: 'catalog' }
];
