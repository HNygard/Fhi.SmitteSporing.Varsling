
export class Indekspasient {
  indekspasientId: number;
  opprettettidspunkt: Date;
  provedato: Date;
  status: string;
  kommuneId: number;
  varslingsstatus: string;
  kanGodkjennesForVarsling: boolean;
  antallSmittekontakter: number;
}

export interface IndekspasientPersonopplysninger {
  telefonnummer: string;
  fodselsnummer: string;
}

export class IndekspasientFilter {
  kreverGodkjenning?: boolean;
  erRegistert?: boolean;
  erFerdig?: boolean;
  manglerKontaktinfo?: boolean;
  kommuneNr?: string;
  medSmittekontakt?: boolean;
  sideindeks?: number;
  sideantall?: number;
  telefonnummer?: string;
}

export interface Varslingssimulering
{
    indekspasientId: number;
    antallKontakter: number;
    antallKontakterTilVarsling: number;
    antallKontakterUtenVarsling: number;
    detaljer: Varslingssimuleringdetaljer[];
}

export interface Varslingssimuleringdetaljer
{
    smittekontaktId: number;
    kanVarsles: boolean;
    varselIkkeTillatAvRegler: Varslingsregel[];
    varselTillatAvRegler: Varslingsregel[];
}

export interface Varslingsregel
{
    navn: string;
    beskrivelse: string;
}
