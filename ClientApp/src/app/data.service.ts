import {  Credantials, Subscription } from './models/subscribe';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';

import { Data } from './models/data';

@Injectable({providedIn: 'root'})
export class DataService{

  constructor(private http: HttpClient) { }

  subscribePhone(phone: string):Observable<string>  {
    return this.http.post<string>('subscriber/register', phone, {headers: this.headers});
  }

  getPhoneSubscription(phone: string | null):Observable<Subscription[]> {
    return this.http.post<Subscription[]>('subscriber/getSubscription', JSON.stringify(phone), {headers: this.headers});
  }

  getSubscriptionData(subjectr: string):Observable<Data[]> {
    return this.http.post<Data[]>('subscriber/getDataListFromFirebase', JSON.stringify(subjectr), {headers: this.headers});
  }

  subscribe(credantials: Credantials) {
    return this.http.post('subscriber/subscribe', JSON.stringify(credantials), {headers: this.headers});
  }

  unsubscribe(credantials: Credantials) {
    return this.http.post('subscriber/undubscribe', JSON.stringify(credantials), {headers: this.headers});
  }

  private headers: HttpHeaders = new HttpHeaders({ 'Content-Type': 'application/json'});

}