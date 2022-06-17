import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SmsComponent } from './sms/sms.component';
import { SubscribeComponent } from './subscribe/subscribe.component';

const routes: Routes = [
   { path: '', component: SubscribeComponent },
  // { path: 'token', component: TokenComponent },
  // { path: 'email', component: EmailComponent },
  // { path: 'pdf', component: PdfComponent },
  { path: 'sms', component: SmsComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' } )],
  exports: [RouterModule]
})
export class AppRoutingModule { }
