import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Indekspasient, IndekspasientFilter, IndekspasientPersonopplysninger, Varslingssimulering } from "../model/indekspasient";
import { AntallIndekspasienterRapport, RapportFilter } from "../model/indekspasienterRapport";
import { PagedList } from '../model/pagedList';

@Injectable()
export class IndekspasientService {
  private baseUrl: string;

  private _personopplysninger: {[id: number]: IndekspasientPersonopplysninger} = {};

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  hentListe(indekspasientFilter: IndekspasientFilter = null): Observable<PagedList<Indekspasient>> {
    const url = `${this.baseUrl}api/indekspasient`;
    return this.http.get<PagedList<Indekspasient>>(url, {
      params: indekspasientFilter as any || {}
    });
  }

  hentForId(indekspasientId: number): Observable<Indekspasient> {
    const url = `${this.baseUrl}api/indekspasient/${indekspasientId}`;
    return this.http.get<Indekspasient>(url);
  }

  markerFerdig(indekspasientId: number): Observable<any> {
    const url = `${this.baseUrl}api/indekspasient/${indekspasientId}/SettVarslingFerdig`;
    return this.http.post(url, null);
  }

  getAntallSmittetilfelleRapportForKommune(kommuneNr: string): Observable<AntallIndekspasienterRapport> {
    const filter: RapportFilter = {
      kommuneNr: kommuneNr
    };
    const url = `${this.baseUrl}api/indekspasient/rapport/`;
    return this.http.get<AntallIndekspasienterRapport>(url, {
      params: filter as any
    });
  }

  getAntallSmittetilfelleRapport(): Observable<AntallIndekspasienterRapport> {
    const url = `${this.baseUrl}api/indekspasient/rapport`;
    return this.http.get<AntallIndekspasienterRapport>(url);
  }

  godkjennVarsling(indekspasientId: number): Observable<any> {
    const url = `${this.baseUrl}api/indekspasient/${indekspasientId}/GodkjennVarsling`;
    return this.http.post(url, {});
  }

  godkjennVarslingBatch(indekspasientIder: number[]): Observable<any> {
    const url = `${this.baseUrl}api/indekspasient/GodkjennVarslingBatch`;
    return this.http.post(url, indekspasientIder);
  }

  registrerTelefonnummer(indekspasientId: number, telefonnummer: string): Observable<any> {
    const url = `${this.baseUrl}api/indekspasient/${indekspasientId}/RegistrerTelefon/?telefonnummer=${telefonnummer}`;
    delete this._personopplysninger[indekspasientId];
    return this.http.post(url, {});
  }

  registrerTelefonnummerIkkeFunnet(indekspasientId: number, ): Observable<any> {
    const url = `${this.baseUrl}api/indekspasient/${indekspasientId}/RegistrerTelefon/?ikkeManueltFunnetKontaktInfo=true`;
    return this.http.post(url, {});
  }

  hentVarslingssimulering(indekspasientId: number): Observable<Varslingssimulering> {
    const url = `${this.baseUrl}api/indekspasient/${indekspasientId}/varslingssimulering`;
    return this.http.get<Varslingssimulering>(url);
  }

  kanDekrypteres(indekspasient: Indekspasient): boolean {
    return !this._personopplysninger[indekspasient.indekspasientId] &&
      (indekspasient.status == 'SmitteKontakt' || indekspasient.status == 'IkkeSmitteKontakt' || indekspasient.status == 'KontaktInfoMangler');
  }

  telefonnummerFor(indekspasient: Indekspasient): string {
    var personopplysninger = this._personopplysninger[indekspasient.indekspasientId];
    return personopplysninger ? (personopplysninger.telefonnummer || 'ukjent') : '****';
  }

  fodselsnummerFor(indekspasient: Indekspasient): string {
    var personopplysninger = this._personopplysninger[indekspasient.indekspasientId];
    return personopplysninger
      ? (personopplysninger.fodselsnummer || 'ukjent')
      : this.kanDekrypteres(indekspasient) ?  '****' : '-';
  }

  dekrypter(indekspasient: Indekspasient) {
    this.hentPersonopplysningerForId(indekspasient.indekspasientId).subscribe(data => {
      this._personopplysninger[indekspasient.indekspasientId] = data;
      setTimeout(() => {
        delete this._personopplysninger[indekspasient.indekspasientId];
      }, 1000 * 60 * 60 * 8);
    })
  }

  private hentPersonopplysningerForId(indekspasientId: number): Observable<IndekspasientPersonopplysninger> {
    const url = `${this.baseUrl}api/indekspasient/${indekspasientId}/personopplysninger`;
    return this.http.get<IndekspasientPersonopplysninger>(url);
  }

}
