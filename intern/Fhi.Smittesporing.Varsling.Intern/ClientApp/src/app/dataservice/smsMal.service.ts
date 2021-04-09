import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { SmsMal, SmsFlettefelt, SmsHendelse, SmsTestmelding, SmsFletteinnstillinger } from '../model/smsMal';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class SmsMalService {
    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    }

    hentStandardSmsMal(): Observable<SmsMal> {
        const url = `${this.baseUrl}api/SmsVarselMal`;
        return this.http.get<SmsMal>(url);
    }

    hentFlettefelter(): Observable<SmsFlettefelt[]> {
        const url = `${this.baseUrl}api/SmsVarselMal/Flettefelter`;
        return this.http.get<SmsFlettefelt[]>(url);
    }

    lagreStandardSmsMal(mal: SmsMal): Observable<any> {
        const url = `${this.baseUrl}api/SmsVarselMal`;
        return this.http.put(url, mal);
    }

    sendTestmeldingStandardmal(testmelding: SmsTestmelding): Observable<any> {
        const url = `${this.baseUrl}api/SmsTestmeldinger`;
        return this.http.post(url, testmelding);
    }

    hentTestmeldingshendelser(smsRef: string): Observable<SmsHendelse[]> {
        const url = `${this.baseUrl}api/SmsTestmeldinger/${smsRef}/Hendelser`;
        return this.http.get<SmsHendelse[]>(url);
    }

    hentFletteinnstillinger(): Observable<SmsFletteinnstillinger> {
        const url = `${this.baseUrl}api/SmsVarselMal/Fletteinnstillinger`;
        return this.http.get<SmsFletteinnstillinger>(url);
    }

    oppdaterFletteinnstillinger(nyeInnstillinger: SmsFletteinnstillinger): Observable<any> {
        const url = `${this.baseUrl}api/SmsVarselMal/Fletteinnstillinger`;
        return this.http.put(url, nyeInnstillinger);
    }
    
    oppdaterSmsInfoForKommune(kommuneId: number, infotekst: string): Observable<any> {
        const url = `${this.baseUrl}api/Kommune/${kommuneId}/SmsFletteinfo`;
        return this.http.put(url, {
            smsFletteinfo: infotekst
        });
    }
}
