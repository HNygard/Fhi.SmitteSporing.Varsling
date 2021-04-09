import { Component, OnInit } from '@angular/core';
import { LookupService } from "../dataservice/lookup.service";
import { Kommune } from "../model/kommune";
import { IndekspasientService } from '../dataservice/indekspasient.service';
import { PagedList, mapPagedList } from '../model/pagedList';
import { Indekspasient, IndekspasientFilter } from '../model/indekspasient';

class ManglendeKontaktInfo {
  indekspasient: Indekspasient;
  nyttTelefonnummer: string;
}

@Component({
  selector: 'app-manglendeKontaktInfo',
  templateUrl: './manglendeKontaktInfo.component.html',
})
export class ManglendeKontaktInfoComponent implements OnInit {

  private _aktivSide: number = 1;

  manglerKontaktinfoListe: PagedList<ManglendeKontaktInfo> = null;
  kommuneList: Kommune[];
  valgtKommune: Kommune;
  showRegistreringAlert = false;
  laster: boolean = true;

  get aktivSide(): number {
    return this._aktivSide;
  }
  set aktivSide(value: number) {
    this._aktivSide = value;
    this.getManglendeKontaktInfo();
  }

  constructor(private indekspasientService: IndekspasientService, private lookupService: LookupService) { }

  async ngOnInit() {
    await this.getManglendeKontaktInfo();
    await this.getKommuneList();
    this.laster = false;
  }

  async getManglendeKontaktInfo(): Promise<any> {
    const filter: IndekspasientFilter = {
      manglerKontaktinfo: true,
      sideindeks: this._aktivSide - 1
    };
    if (this.valgtKommune) {
      filter.kommuneNr = this.valgtKommune.kommuneNr;
    }
    const indekspasienter = await this.indekspasientService.hentListe(filter).toPromise();
    this.manglerKontaktinfoListe = mapPagedList(indekspasienter, x => ({
      indekspasient: x, nyttTelefonnummer: ''
    }));
  }

  async getKommuneList() {
    this.kommuneList = await this.lookupService.getKommuneListe().toPromise();
  }

  erDekryptert(manglendeKontaktInfo: ManglendeKontaktInfo): boolean {
    return !this.indekspasientService.kanDekrypteres(manglendeKontaktInfo.indekspasient);
  }

  fodselsnummerFor(manglendeKontaktInfo: ManglendeKontaktInfo): string {
    return this.indekspasientService.fodselsnummerFor(manglendeKontaktInfo.indekspasient);
  }

  hentKommuneFor(manglendeKontaktInfo: ManglendeKontaktInfo): string {
    var kommune =  this.kommuneList && this.kommuneList.find(k => k.kommuneId == manglendeKontaktInfo.indekspasient.kommuneId);
    return kommune ? kommune.navn : '-';
  }

  dekrypter(manglendeKontaktInfo: ManglendeKontaktInfo) {
    return this.indekspasientService.dekrypter(manglendeKontaktInfo.indekspasient);
  }

  async lagre(manglerKontaktinfo: ManglendeKontaktInfo) {
    await this.indekspasientService.registrerTelefonnummer(
        manglerKontaktinfo.indekspasient.indekspasientId,
        manglerKontaktinfo.nyttTelefonnummer
      ).toPromise()
      .then(() => this.getManglendeKontaktInfo())
      .catch(() => {
        this.showRegistreringAlert = true;
        window.scrollTo(0, 0);
      });
  }

  async lagreIkkeFunnet(manglerKontaktinfo: ManglendeKontaktInfo) {
    await this.indekspasientService.registrerTelefonnummerIkkeFunnet(
      manglerKontaktinfo.indekspasient.indekspasientId).toPromise()
      .then(() => this.getManglendeKontaktInfo())
      .catch(() => {
        this.showRegistreringAlert = true;
        window.scrollTo(0, 0);
      });
  }

  async velgKommune(kommune: Kommune): Promise<any> {
    this.valgtKommune = kommune;
    await this.getManglendeKontaktInfo();
  }

}
