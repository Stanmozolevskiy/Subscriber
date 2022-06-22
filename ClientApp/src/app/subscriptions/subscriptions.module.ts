import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';

import { SingleSubscriptionComponent } from './single-subscription/single-subscription.component';

import { AppRoutingModule } from '../app-routing.module';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { IndicatorsModule } from '@progress/kendo-angular-indicators';
import { IconsModule } from '@progress/kendo-angular-icons';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LabelModule } from '@progress/kendo-angular-label';

@NgModule({
  declarations: [
    SingleSubscriptionComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    LabelModule,
    BrowserModule,
    BrowserAnimationsModule,
    ButtonsModule,
    IconsModule,
    IndicatorsModule,
    DialogsModule,
    LayoutModule,

    AppRoutingModule,
  
  ]
})
export class SubscriptionsModule { }
