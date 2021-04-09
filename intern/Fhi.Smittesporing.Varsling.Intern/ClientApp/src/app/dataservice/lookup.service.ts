import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Kommune } from '../model/kommune';
import { Observable } from 'rxjs';
import { FunksjonKonfig } from '../model/funksjonKonfig';

@Injectable()
export class LookupService {
  private baseUrl: string;

    constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    getKommuneListe(): Observable<Kommune[]> {
        const url = `${this.baseUrl}api/kommune`;
        return this.http.get<Kommune[]>(url);
    }

    hentFunksjoner(): Observable<FunksjonKonfig> {
        const url = `${this.baseUrl}api/ServerInfo/Funksjoner`;
        return this.http.get<FunksjonKonfig>(url);
    }
}
