import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Smittekontakt, SmittekontaktQuery, SmittekontaktPersonopplysninger, SmittekontaktListemodell, SmsVarsel } from '../model/smittekontakt';
import { Observable } from 'rxjs';
import { PagedList } from '../model/pagedList';

@Injectable()
export class SmittekontaktService {
    private baseUrl: string;

    private _personopplysningerForKontakter: {[id: number]: SmittekontaktPersonopplysninger} = {}

    constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    hentSmittekontakterForIndekspasient(indekspasientId: number, sideindeks: number = 0): Observable<PagedList<SmittekontaktListemodell>> {
        const url = `${this.baseUrl}api/smittekontakt`;
        const query: SmittekontaktQuery = {
            indekspasientId: indekspasientId,
            sideindeks: sideindeks
        }
        return this.http.get<PagedList<SmittekontaktListemodell>>(url, {
            params: query as any
        });
    }

    hentSmittekontakt(smittekontaktId: number): Observable<Smittekontakt> {
        const url = `${this.baseUrl}api/smittekontakt/${smittekontaktId}`;
        return this.http.get<Smittekontakt>(url);
    }

    sendVarsel(smittekontaktId: number) {
        const url = `${this.baseUrl}api/smittekontakt/${smittekontaktId}/sendvarsel`;
        return this.http.post(url, null);
    }

    hentVarselInfo(smittekontaktId: number): Observable<SmsVarsel[]> {
        const url = `${this.baseUrl}api/smittekontakt/${smittekontaktId}/smsvarsler`;
        return this.http.get<SmsVarsel[]>(url);
    }

    lagOppsummertKontaktGrafUrl(smittekontaktId: number): string {
      return `${this.baseUrl}api/smittekontakt/${smittekontaktId}/OppsummertKontaktGraf`;
    }

    lagGpsHistPlotUrlUrl(smittekontaktId: number): string {
      return `${this.baseUrl}api/smittekontakt/${smittekontaktId}/GpsHistogram`;
    }

    lagKartForDagHtmlUrl(smittekontaktId: number, smittekontaktDagId: number): string {
        return `${this.baseUrl}api/smittekontakt/${smittekontaktId}/Dager/${smittekontaktDagId}/KartHtml`;
    }

    erKontaktDekryptert(smittekontakt: Smittekontakt): boolean {
      return !!this._personopplysningerForKontakter[smittekontakt.smittekontaktId];
    }
  
    telefonnummerForKontakt(smittekontakt: Smittekontakt): string {
      var personopplysninger = this._personopplysningerForKontakter[smittekontakt.smittekontaktId];
      return personopplysninger ? personopplysninger.telefonnummer : '****';
    }
  
    dekrypterKontakt(smittekontakt: Smittekontakt) {
      this.hentPersonopplysninger(smittekontakt.smittekontaktId).subscribe(data => {
        this._personopplysningerForKontakter[smittekontakt.smittekontaktId] = data;
        setTimeout(() => {
          delete this._personopplysningerForKontakter[smittekontakt.smittekontaktId];
        }, 1000 * 60 * 60 * 8);
      });
    }

    private hentPersonopplysninger(smittekontaktId: number): Observable<SmittekontaktPersonopplysninger> {
        const url = `${this.baseUrl}api/smittekontakt/${smittekontaktId}/personopplysninger`;
        return this.http.get<SmittekontaktPersonopplysninger>(url);
    }
}
