import { Subscription, Credantials } from '../models/subscribe';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DataService } from '../data.service';
import {Router} from '@angular/router';
import { connectableObservableDescriptor } from 'rxjs/internal/observable/ConnectableObservable';

@Component({
  selector: 'app-subscribe',
  templateUrl: './subscribe.component.html',
  styleUrls: ['./subscribe.component.css']
})
export class SubscribeComponent implements OnInit {
  regestrarionForm!: FormGroup;
  newSubscription!: FormGroup;
  subscriptions!:Subscription[];
  number!:string | null;
  unSubscribeSubject!:string;
  isModalUnsubscribeOpen: boolean = false;
  isModalNewSubscriptionOpen: boolean = false;
  loading: boolean = true;
  
  constructor(private dataService: DataService, private fb: FormBuilder, private route:Router) { }
  ngOnInit(): void {
    this.regestrarionForm = this.fb.group({
      Phone: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(10), Validators.pattern('[0-9]+')]],
    });
    this.newSubscription = this.fb.group({
      Subject: ['', [Validators.required, Validators.minLength(3)]],
    });

    this.number = localStorage.getItem("subscription");
    if(this.number != null && this.number != ""){
      this.updateGrid();
    }
  }

  onRegester(){
    if(this.number != null && this.number != "")
    this.dataService.subscribePhone(this.regestrarionForm.get("Phone")?.value).subscribe();
    
    this.number= `${this.regestrarionForm.get("Phone")?.value}`; 
    localStorage.setItem("subscription", this.number);
    
    this.updateGrid();
  }

  OnRegestrationNumberChange(){
    this.number = null;
    this.regestrarionForm.reset();
    this.regestrarionForm.enable();
    localStorage.removeItem("subscription");
  }

  openModelCreate(event: any){
    event.preventDefault();
    this.isModalNewSubscriptionOpen = true;
  }
  onSubscribe(){
    console.log(`${this.newSubscription.get("Subject")?.value}`); 
    this.dataService.subscribe( new Credantials(this.number, `${this.newSubscription.get("Subject")?.value}`)).subscribe();
    this.closeModel();
    this.updateGrid();
  }

  openModelUnsubscribe(event: any, name: string) {
    event.preventDefault();
    this.unSubscribeSubject = name;
    this.isModalUnsubscribeOpen = true;
  }
  onUnsubscribe(){
    this.dataService.unsubscribe(new Credantials(this.number, this.unSubscribeSubject)).subscribe();
    this.closeModel();
    this.updateGrid();
  }

  closeModel() {
    if (this.isModalUnsubscribeOpen) 
      this.isModalUnsubscribeOpen = false; 
      
    else(this.isModalNewSubscriptionOpen)
      this.isModalNewSubscriptionOpen = false; 
  }

  private updateGrid() 
  {
    //set 1 second delay to get the data
    setTimeout(()=> this.handleData(), 1000);

    
    this.regestrarionForm.reset();
    this.regestrarionForm.disable();
  }

  private handleData(){
    this.dataService.getPhoneSubscription(this.number).subscribe(res => res.length == 43 ? this.subscriptions = []
      : this.subscriptions = res);
      this.loading = false;
  }
}
