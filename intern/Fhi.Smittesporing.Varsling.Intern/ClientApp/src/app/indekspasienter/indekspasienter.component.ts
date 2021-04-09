import { Component, OnInit } from '@angular/core';
import { IndekspasientService } from '../dataservice/indekspasient.service';
import { LookupService } from "../dataservice/lookup.service";
import { Indekspasient, IndekspasientFilter } from '../model/indekspasient';
import { Kommune } from "../model/kommune";
import { PagedList } from '../model/pagedList';

@Component({
  selector: 'app-indekspasienter',
  templateUrl: './indekspasienter.component.html',
  styleUrls: ['./indekspasienter.component.css']
})
export class IndekspasienterComponent implements OnInit {

  // static for at valg skal huskes mellom navigering
  private static _aktivSide: number = 1;
  private static _valgtKommune: Kommune;
  private static _visErFerdig: boolean = false;
  private static _visMedSmittekontakt: boolean = true;
  private static _aktivtTelefonnummerFilter: string = null;

  indekspasienter: PagedList<Indekspasient>;
  kommuneList: Kommune[];
  smittetilfelleValgteTilGodkjenning = new Array<Indekspasient>();
  laster = false;
  showGodkjenningAlert = false;
  visVelgTilfelleTekst = true;

  telefonnummerInput: string = IndekspasienterComponent._aktivtTelefonnummerFilter || '';

  get visErFerdig(): boolean {
    return IndekspasienterComponent._visErFerdig;
  }
  set visErFerdig(value: boolean) {
    IndekspasienterComponent._visErFerdig = value;
    this.hentIndekspasienter();
  }

  get visMedSmittekontakt(): boolean {
    return IndekspasienterComponent._visMedSmittekontakt;
  }
  set visMedSmittekontakt(value: boolean) {
    IndekspasienterComponent._visMedSmittekontakt = value;
    this.hentIndekspasienter();
  }

  get aktivSide(): number {
    return IndekspasienterComponent._aktivSide;
  }
  set aktivSide(value: number) {
    IndekspasienterComponent._aktivSide = value;
    this.hentIndekspasienter();
  }

  get valgtKommune(): Kommune {
    return IndekspasienterComponent._valgtKommune;
  }
  set valgtKommune(value: Kommune) {
    IndekspasienterComponent._valgtKommune = value;
    IndekspasienterComponent._aktivSide = 1;
    this.hentIndekspasienter();
  }

  get aktivtTelefonnummerFilter(): string {
    return IndekspasienterComponent._aktivtTelefonnummerFilter;
  }
  set aktivtTelefonnummerFilter(value: string) {
    IndekspasienterComponent._aktivtTelefonnummerFilter = value;
    IndekspasienterComponent._aktivSide = 1;
    this.hentIndekspasienter();
  }

  constructor(private indekspasientService: IndekspasientService, private lookupService: LookupService) { }

  ngOnInit(): void {
    this.getKommuneList();
    this.hentIndekspasienter();
  }

  async hentIndekspasienter() {
    this.laster = true;

    const filter: IndekspasientFilter = {
      sideindeks: this.aktivSide - 1
    };

    if (this.valgtKommune) {
      filter.kommuneNr = this.valgtKommune.kommuneNr
    }

    if (this.visErFerdig !== null) {
      filter.erFerdig = this.visErFerdig;
    }

    if (this.visMedSmittekontakt !== null) {
      filter.medSmittekontakt = this.visMedSmittekontakt;
    }

    if (this.aktivtTelefonnummerFilter) {
      filter.telefonnummer = this.aktivtTelefonnummerFilter;
    }

    const smittetilfelleList = await this.indekspasientService.hentListe(filter).toPromise();
    this.laster = false;

    this.indekspasienter = smittetilfelleList;
  }

  getKommuneList(): void {
    this.lookupService.getKommuneListe().subscribe(data => {
      this.kommuneList = data;
    });
  }

  velgKommune(kommune: Kommune) {
    this.valgtKommune = kommune;
  }

  kanDekrypteres(indekspasient: Indekspasient): boolean {
    return this.indekspasientService.kanDekrypteres(indekspasient);
  }

  hentKommuneFor(indekspasient: Indekspasient): string {
    var kommune = this.kommuneList && this.kommuneList.find(k => k.kommuneId == indekspasient.kommuneId);
    return kommune ? kommune.navn : '-';
  }

  telefonnummerFor(indekspasient: Indekspasient): string {
    return this.indekspasientService.telefonnummerFor(indekspasient);
  }

  fodselsnummerFor(indekspasient: Indekspasient): string {
    return this.indekspasientService.fodselsnummerFor(indekspasient);
  }

  dekrypter(indekspasient: Indekspasient) {
    this.indekspasientService.dekrypter(indekspasient);
  }

  fjernTlfFilter() {
    this.aktivtTelefonnummerFilter = null;
    this.telefonnummerInput = '';
  }

}
