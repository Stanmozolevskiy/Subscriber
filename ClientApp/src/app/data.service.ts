import { SMS, TwilioAccount } from './models/sms';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';


import { Observable } from 'rxjs';

@Injectable({providedIn: 'root'})
export class DataService{

  constructor(private http: HttpClient) { }

  sendSms(sms: SMS): Observable<SMS>{
    return this.http.post<SMS>('sms/send', sms, {headers: this.headers});
  }

  getSummary(): Observable<TwilioAccount[]> {
     return this.http.get<TwilioAccount[]>(`sms`, { headers: this.headers });
  }

  private headers: HttpHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

}