import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InnsynFilter, InnsynLogg, InnsynSmittekontakt, InnsynIndekspasient, InnsynSmsVarsel, InnsynSimulaDatabruk, InnsynSimulaGpsData } from '../model/innsyn';
import { PagedList } from '../model/pagedList';
import { CustomHttpParamEncoder } from '../utils/customHttpParamEncoder';

@Injectable()
export class InnsynService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  hentLogg(innsynFilter: InnsynFilter = null): Observable<PagedList<InnsynLogg>> {
    const url = `${this.baseUrl}api/innsyn/logg`;
    return this.http.get<PagedList<InnsynLogg>>(url, {
      params: innsynFilter as any || {}
      //params: new HttpParams({ encoder: new CustomHttpParamEncoder(), fromObject: innsynFilter as any || {} }) 
    });
  }

  loggSøk(innsynFilter: InnsynFilter) {
    const url = `${this.baseUrl}api/innsyn/logg`;
    return this.http.post(url, innsynFilter);// new HttpParams({ encoder: new CustomHttpParamEncoder(), fromObject: innsynFilter as any || {} }));
  }

  hentSmittekontakt(innsynFilter: InnsynFilter = null): Observable<PagedList<InnsynSmittekontakt>> {
    const url = `${this.baseUrl}api/innsyn/smittekontakter`;
    return this.http.get<PagedList<InnsynSmittekontakt>>(url, {
      params: innsynFilter as any || {}
      //params: new HttpParams({ encoder: new CustomHttpParamEncoder(), fromObject: innsynFilter as any || {} }) 
    });
  }

  hentIndekspasient(innsynFilter: InnsynFilter = null): Observable<PagedList<InnsynIndekspasient>> {
    const url = `${this.baseUrl}api/innsyn/indekspasienter`;
    return this.http.get<PagedList<InnsynIndekspasient>>(url, {
      params: innsynFilter as any || {}
      //params: new HttpParams({ encoder: new CustomHttpParamEncoder(), fromObject: innsynFilter as any || {} }) 
    });
  }

  hentSmsVarsel(innsynFilter: InnsynFilter = null): Observable<PagedList<InnsynSmsVarsel>> {
    const url = `${this.baseUrl}api/innsyn/smsvarsler`;
    return this.http.get<PagedList<InnsynSmsVarsel>>(url, {
      params: innsynFilter as any || {}
      //params: new HttpParams({ encoder: new CustomHttpParamEncoder(), fromObject: innsynFilter as any || {} }) 
    });
  }
  hentSimulaDatabruk(innsynFilter: InnsynFilter = null): Observable<PagedList<InnsynSimulaDatabruk>> {
    const url = `${this.baseUrl}api/innsyn/simuladatabruk`;
    return this.http.get<PagedList<InnsynSimulaDatabruk>>(url, {
      params: innsynFilter as any || {}
      //params: new HttpParams({ encoder: new CustomHttpParamEncoder(), fromObject: innsynFilter as any || {} }) 
    });
  }
  hentSimulaGpsData(innsynFilter: InnsynFilter = null): Observable<PagedList<InnsynSimulaGpsData>> {
    const url = `${this.baseUrl}api/innsyn/simulagpsdata`;
    return this.http.get<PagedList<InnsynSimulaGpsData>>(url, {
      params: innsynFilter as any || {}
      //params: new HttpParams({ encoder: new CustomHttpParamEncoder(), fromObject: innsynFilter as any || {} }) 
    });
  }
}
