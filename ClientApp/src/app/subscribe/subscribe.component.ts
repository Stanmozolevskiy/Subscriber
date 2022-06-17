import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DataService } from '../data.service';

@Component({
  selector: 'app-subscribe',
  templateUrl: './subscribe.component.html',
  styleUrls: ['./subscribe.component.css']
})
export class SubscribeComponent implements OnInit {

  newSubscribtinForm!: FormGroup;
  oldSubscribtinForm!: FormGroup;
  
  constructor(private dataService: DataService, private fb: FormBuilder) { }
  ngOnInit(): void {

    this.newSubscribtinForm = this.fb.group({
      Phone: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(10), Validators.pattern('[0-9]+')]],
  });
  this.oldSubscribtinForm = this.fb.group({
    Phone: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(10), Validators.pattern('[0-9]+')]],
});
}


onCheckSubscription(){
  this.dataService.subscribePhone(this.oldSubscribtinForm.get("Phone")?.value).subscribe((res)=> console.log(res));
  this.oldSubscribtinForm.reset();
  this.oldSubscribtinForm.disable();
}

onSubscribe(){
  this.dataService.subscribePhone(this.newSubscribtinForm.get("Phone")?.value).subscribe();
  this.newSubscribtinForm.reset();
  this.newSubscribtinForm.disable();
}

}
