
export interface SmsMal {
    meldingsinnhold: string;
    avsender: string;
}

export interface SmsFlettefelt {
    navn: string;
    placeholder: string;
    beskrivelse: string;
    eksempelverdi: string;
}

export interface SmsHendelse {
    loepenummer: number;
    smsUtsendingReferanse: string;
    gjeldeneStatus: string;
    antallSegmenter: number;
    tidspunkt: string;
    beskrivelse: string;
}

export interface SmsTestmelding {
    telefonnummer: string;
    referanse: string;
    kommuneId?: number;
    risikokategori?: string;
    datoSisteKontakt?: string;
}

export interface SmsFletteinnstillinger {
    kommuneinfoFallback: string;
    lavRisikokategori: string;
    middelsRisikokategori: string;
    hoyRisikokategori: string;
}