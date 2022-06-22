
export class Subscribe {
    Phone!:string;
}

export class Subscription{
    Name!:string;
}

export class Credantials{
    Phone!:string | null;
    Subject!:string;

    public constructor(phone:string | null, subject:string){
        this.Phone = phone; 
        this.Subject = subject;
    }
}

