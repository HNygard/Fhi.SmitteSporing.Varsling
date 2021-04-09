export class InnsynFilter {
  telefonnummer?: string;
  fodselsnummer?: string;
  sideindeks?: number;
  sideantall?: number;
}

export class InnsynLogg {
  created: Date;
  hva: string;
  hvorfor: string;
  hvem: string;
  felt: string;
}

export class InnsynSmittekontakt {

}

export class InnsynIndekspasient {
  opprettettidspunkt: Date;
  provedato: string;
  kommune: string;
  created: Date;
  status: string;
  varslingsstatus: string;
  kanGodkjennesForVarsling: boolean;
}

export class InnsynSmsVarsel {
}

export class InnsynSimulaDatabruk {

}

export class InnsynSimulaGpsData { }
