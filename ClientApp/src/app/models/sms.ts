
export class SMS {
    Recipients!: Recipient[];
    Body!:string;
}

export class Recipient {
    constructor(number: string) {
        this.PhoneNumber = number;
    }
    PhoneNumber!: string;
}

export class TwilioAccount {
    Status!: string;
    SID!: string;
    Token!: string;
    Name!: string;
}