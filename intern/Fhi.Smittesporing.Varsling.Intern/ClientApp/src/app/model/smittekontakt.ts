import { SmsHendelse } from "./smsMal";

export interface SmittekontaktListemodell {
  smittekontaktId: number;
  indekspasientId: number;
  varsletTidspunkt: string;

  risikokategori: string;
  interessepunkter: {[poi:string]: number};
  antallKontakter: string;
  sisteKontaktDato: string;

  bluetoothAkkumulertVarighet: number;
  gpsAkkumulertVarighet: number;
}

export interface Smittekontakt {
  smittekontaktId: number;
  indekspasientId: number;

  varsletTidspunkt: string;

  pipelineVersjon: string;
  enhetsinfo: string[];

  harGpsHistogram: boolean;
  harKontaktDiagram: boolean;

  risikokategori: string;
  interessepunkter: {[poi:string]: number};
  antallKontakter: string;
  antallDagerMedKontakt: number; // nullable

  bluetoothAkkumulertVarighet: number;
  bluetoothAkkumulertRisikoscore: number;
  bluetoothVeldigNarVarighet: number;
  bluetoothRelativtNarVarighet: number;
  bluetoothNarVarighet: number;
  bluetoothAntallDagerMedKontakt: number; // nullable

  gpsAkkumulertVarighet: number;
  gpsAkkumulertRisikoscore: number;
  gpsAntallDagerMedKontakt: number; // nullable

  detaljer: SmittekontaktDetaljer[];
}

export interface SmittekontaktDetaljer {
  smittekontaktDetaljerId: number;
  dato: string;

  harKart: boolean;

  interessepunkter: {[poi:string]: number};

  gpsAkkumulertVarighet: number;
  gpsAkkumulertRisiko: number;
  gpsMedianavstand: number;

  bluetoothAkkumulertVarighet: number;
  bluetoothAkkumulertRisiko: number;
  bluetoothMedianavstand: number;
  bluetoothVeldigNarVarighet: number;
  bluetoothRelativtNarVarighet: number;
  bluetoothNarVarighet: number;
}

export interface SmittekontaktPersonopplysninger {
  telefonnummer: string;
}

export interface SmittekontaktQuery {
  indekspasientId?: number;
  sideindeks?: number;
  sideantall?: number;
}

export interface SmsVarsel {
  status: string;
  created: string;
  referanse: string;
  sistOppdatert: string;
  SisteEksterneHendelsestidspunkt: string;
  oppdateringer: SmsHendelse[];
}
