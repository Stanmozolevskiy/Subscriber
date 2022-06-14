import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SmsComponent } from './sms.component';
import { BrowserModule } from '@angular/platform-browser';
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

@NgModule({
  declarations: [
    SmsComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    ButtonsModule,
    IconsModule,
    InputsModule,
    IndicatorsModule,
    DialogsModule,
    LayoutModule,
    ReactiveFormsModule,
    FormsModule,
    DropDownsModule,
    LabelModule,
    BrowserAnimationsModule,
    UploadsModule,
    GridModule,
    ExcelModule
  ]
})
export class SmsModule { }