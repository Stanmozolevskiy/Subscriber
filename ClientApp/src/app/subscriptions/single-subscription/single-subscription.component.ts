import { DataService } from './../../data.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Data } from 'src/app/models/data';
 

@Component({
  selector: 'app-single-subscription',
  templateUrl: './single-subscription.component.html',
  styleUrls: ['./single-subscription.component.css']
})
export class SingleSubscriptionComponent implements OnInit {
  id!:string;
  subscription!:string;
  message!: string;
  data!:Data[];  
  loading: boolean = true;

  constructor(private route: ActivatedRoute, private dataService: DataService ) { }

  ngOnInit(): void {
    this.getData();
  }

  public getData(){
    this.route.paramMap.subscribe((query:any)=>{ 
      this.id = query.params.id; 
      this.subscription = query.params.subscription;
    })

    this.dataService.getSubscriptionData(`${this.id}/${this.subscription}`).subscribe(
      res => res.length == 43 ?  this.handleError()
      : this.data = res);
      this.loading = false;
    }
    
  private handleError(){
    this.message = "Please wait one minute for data to populate";
    if(this.loading == false) this.loading = true;
    setTimeout(()=>  this.handleData(), 3000 );
    }
    
    private handleData(){
      this.dataService.getSubscriptionData(`${this.id}/${this.subscription}`).subscribe(
                                                                res => res.length == 43 ?  
                                                                this.handleError():
                                                                this.data = res);
      this.loading = false;
      }
  }
