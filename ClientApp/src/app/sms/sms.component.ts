import { SMS, Recipient, TwilioAccount } from './../models/sms';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpProgressEvent, HttpRequest,HttpEventType, HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FileRestrictions } from "@progress/kendo-angular-upload";
import { concat, Observable, of } from 'rxjs';
import { DataService } from '../data.service';

@Component({
  selector: 'app-sms',
  templateUrl: './sms.component.html',
  styleUrls: ['./sms.component.css']
})

export class SmsComponent implements OnInit, HttpInterceptor {
  intercept(req: HttpRequest<any>,next: HttpHandler): Observable<HttpEvent<any>> 
  {
    if (req.url === "sms/saveAttachment") {
      const events: Observable<HttpEvent<any>>[] = [0, 30, 60, 100].map((x) =>
        of(<HttpProgressEvent>
          {
          type: HttpEventType.UploadProgress,
          loaded: x,
          total: 100,
          }));

      const success:any = of(new HttpResponse({ status: 200 }));
       events.push(success);
      
       return concat(...events);

    }
    if (req.url === 'sms/rempveAttachment') {
      return of(new HttpResponse({ status: 200 }));
    }
    return next.handle(req);
  };

  smsSent!: boolean;
  smsForm!: FormGroup;
  uploadSaveUrl = "sms/saveAttachment";
  uploadRemoveUrl = "sms/rempveAttachment";
  sms!: SMS;
  twilioAccounts!: TwilioAccount[];
  loading: boolean = true;
  modalLoading: boolean = true;
  isModalSuspendOpen: boolean = false;
  isModalResumeOpen: boolean = false;


  constructor(private dataService: DataService, private fb: FormBuilder) { } 
  ngOnInit(): void {
    this.updateGrid();
    this.smsForm = this.fb.group({
      Body: ['',[Validators.required]],
      Recipients: ["", [Validators.required]]
  });
  }

  onSubmit(){
    let sms = {...new SMS(), ...this.smsForm.value};
    sms.Recipients = [new Recipient(this.smsForm.value.Recipients)];

    this.dataService.sendSms(sms).subscribe();
    this.smsForm.reset();
    this.smsForm.disable();
    this.smsSent = true;
  }

  myRestrictions: FileRestrictions = {
    maxFileSize: 4194304,
    allowedExtensions: [".jpg", ".png", ".jpeg", ".gif", ".pdf", ".vcard", ".csv"],
  };
closeModel() {
    if (this.isModalSuspendOpen) {
        this.isModalSuspendOpen = false;
        this.modalLoading = false;
    }
    else {
        this.isModalResumeOpen = false;
        this.modalLoading = false;
    }
}
openModelSuspend(event: any, SID: string) {
  event.preventDefault();
  this.SID = SID;
  this.isModalSuspendOpen = true;
}
openModelResume(event: any, SID: string) {
  event.preventDefault();
  this.SID = SID;
  this.isModalResumeOpen = true;
}

private updateGrid() {
    this.modalLoading = true;
    this.dataService.getSummary().subscribe(response => {
        this.twilioAccounts = response;
        this.loading = false;
        this.closeModel();
    })
}

private SID!: string;
}