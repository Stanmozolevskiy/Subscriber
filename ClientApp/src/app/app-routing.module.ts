import { SingleSubscriptionComponent } from './subscriptions/single-subscription/single-subscription.component';

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SubscribeComponent } from './subscribe/subscribe.component';

const routes: Routes = [
   { path: '', component: SubscribeComponent },
   { path: 'subscriptions/:id/:subscription', component: SingleSubscriptionComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' } )],
  exports: [RouterModule]
})
export class AppRoutingModule { }
