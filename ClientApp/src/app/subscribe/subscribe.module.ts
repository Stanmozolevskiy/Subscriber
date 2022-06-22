import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from "@angular/platform-browser";
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { IconsModule } from '@progress/kendo-angular-icons';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { IndicatorsModule } from '@progress/kendo-angular-indicators';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LabelModule } from '@progress/kendo-angular-label';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { UploadsModule } from '@progress/kendo-angular-upload';
import {GridModule,ExcelModule} from "@progress/kendo-angular-grid";

import { SubscribeComponent } from './subscribe.component';
import { AppRoutingModule } from '../app-routing.module';



@NgModule({
  declarations: [
    SubscribeComponent
  ],
  imports: [
    CommonModule,
    InputsModule,
    LabelModule,
    FormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    ButtonsModule,
    IconsModule,
    IndicatorsModule,
    DialogsModule,
    LayoutModule,
    ReactiveFormsModule,
    DropDownsModule,
    UploadsModule,
    GridModule,
    ExcelModule,
    AppRoutingModule,
  ]
})
export class SubscribeModule { }
