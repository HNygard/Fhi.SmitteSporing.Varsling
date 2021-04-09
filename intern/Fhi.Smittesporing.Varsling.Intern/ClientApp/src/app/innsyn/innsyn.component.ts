import { Component, OnInit, Inject } from "@angular/core";
import { InnsynService } from '../dataservice/innsyn.service';
import { InnsynFilter, InnsynLogg, InnsynSmittekontakt, InnsynIndekspasient, InnsynSmsVarsel, InnsynSimulaDatabruk, InnsynSimulaGpsData } from '../model/innsyn';
import { PagedList } from "../model/pagedList";

@Component({
  selector: 'app-innsyn',
  templateUrl: './innsyn.component.html',
  styleUrls: ['./innsyn.component.css']
})

export class InnsynComponent implements OnInit {

  private _aktivSideLogg: number = 1;
  private _aktivSideSmittekontakter: number = 1;
  private _aktivSideIndeksPasienter: number = 1;
  private _aktivSideSmsVarsler: number = 1;
  private _aktivSideSimulaDatabruk = 1;
  private _aktivSideSimulaGpsData = 1;

  failedLogg: boolean = false;
  failedSmittekontakt: boolean = false;
  failedIndekspasient: boolean = false;
  failedSmsvarsel: boolean = false;
  failedSimuladatabruk: boolean = false;
  failedSimulagpsdata: boolean = false;

  activeTab: number = 1;
  telefonnummer: string;
  fodselsnummer: string;
  logg: PagedList<InnsynLogg>;
  simuladatabruk: PagedList<InnsynSimulaDatabruk>;
  simulagpsdata: PagedList<InnsynSimulaGpsData>;
  smittekontakter: PagedList<InnsynSmittekontakt>;
  indekspasienter: PagedList<InnsynIndekspasient>;
  smsvarsler: PagedList<InnsynSmsVarsel>;
  laster = false;

  async search() {

    this.showAlert = false;

    const filter = this.getFilter(0);

    this.innsynService.loggSøk(filter)
      .toPromise()
      .then(() => {
        this._aktivSideLogg = 1;
        this._aktivSideIndeksPasienter = 1;
        this._aktivSideSmittekontakter = 1;
        this._aktivSideSmsVarsler = 1;
        this._aktivSideSimulaDatabruk = 1;
        this._aktivSideSimulaGpsData = 1;
        this.hentLogg();
        this.hentSmittekontakter();
        this.hentInnsynIndekspasienter();
        this.hentInnsynSmsVarsler();
        this.hentInnsynSimulaDatabruk();
        this.hentInnsynSimulaGpsData();
      })
      .catch(() => {
        this.showAlert = true;
        window.scrollTo(0, 0);
      });
  }

  async eksporter() {    
    window.open(`${this.baseUrl}api/innsyn/eksport/?telefonnummer=${encodeURIComponent(this.telefonnummer)}&fodselsnummer=${this.fodselsnummer}`, '_blank');
  }

  getFilter(sideindeks: number) {
    const filter: InnsynFilter = {
      sideindeks: sideindeks - 1,
      fodselsnummer: this.fodselsnummer,
      telefonnummer: this.telefonnummer
    };

    return filter;
  }

  async hentLogg() {
    this.laster = true;
    try {
      const innsyn = await this.innsynService.hentLogg(this.getFilter(this._aktivSideLogg)).toPromise();
      this.logg = innsyn;
    }
    catch{
      this.failedLogg = true;
    }
    this.laster = false;
  }

  async hentSmittekontakter() {
    this.laster = true;
    try {
      const smittekontakter = await this.innsynService.hentSmittekontakt(this.getFilter(this._aktivSideSmittekontakter)).toPromise();
      this.smittekontakter = smittekontakter;
    }
    catch{
      this.failedSmittekontakt = true;
    }
    this.laster = false;
  }

  async hentInnsynIndekspasienter() {
    this.laster = true;
    try {
      const innsynIndekspasienter = await this.innsynService.hentIndekspasient(this.getFilter(this._aktivSideIndeksPasienter)).toPromise();
      this.indekspasienter = innsynIndekspasienter;
    }
    catch{
      this.failedIndekspasient = true;
    }
    this.laster = false;
  }

  async hentInnsynSmsVarsler() {
    this.laster = true;
    try {
      const smsvarsler = await this.innsynService.hentSmsVarsel(this.getFilter(this._aktivSideSmsVarsler)).toPromise();
      this.smsvarsler = smsvarsler;
    }
    catch{
      this.failedSmsvarsel = true;
    }

    this.laster = false;
  }

  async hentInnsynSimulaDatabruk() {
    this.laster = true;
    try {
      const simuladatabruk = await this.innsynService.hentSimulaDatabruk(this.getFilter(this._aktivSideSimulaDatabruk)).toPromise();
      this.simuladatabruk = simuladatabruk;
    }
    catch{
      this.failedSimuladatabruk = true;
    }
    this.laster = false;
  }

  async hentInnsynSimulaGpsData() {
    this.laster = true;
    try {
      const simulagpsdata = await this.innsynService.hentSimulaGpsData(this.getFilter(this._aktivSideSimulaGpsData)).toPromise();
      this.simulagpsdata = simulagpsdata;
    }
    catch{
      this.failedSimulagpsdata = true;
    }
    this.laster = false;
  }

  get aktivSideLogg(): number {
    return this._aktivSideLogg;
  }
  set aktivSideLogg(value: number) {
    this._aktivSideLogg = value;
    this.hentLogg();
  }
  get aktivSideSmittekontakter(): number {
    return this._aktivSideSmittekontakter;
  }
  set aktivSideSmittekontakter(value: number) {
    this._aktivSideSmittekontakter = value;
    this.hentSmittekontakter();
  }
  get aktivSideIndeksPasienter(): number {
    return this._aktivSideIndeksPasienter;
  }
  set aktivSideIndeksPasienter(value: number) {
    this._aktivSideIndeksPasienter = value;
    this.hentInnsynIndekspasienter();
  }
  get aktivSideSmsVarsler(): number {
    return this._aktivSideIndeksPasienter;
  }
  set aktivSideSmsVarsler(value: number) {
    this._aktivSideSmsVarsler = value;
    this.hentInnsynSmsVarsler();
  }
  get aktivSideSimulaDatabruk(): number {
    return this._aktivSideSimulaDatabruk;
  }
  set aktivSideSimulaDatabruk(value: number) {
    this._aktivSideSimulaDatabruk = value;
    this.hentInnsynSimulaDatabruk();
  }
  get aktivSideSimulaGpsData(): number {
    return this._aktivSideSimulaGpsData;
  }
  set aktivSideSimulaGpsData(value: number) {
    this._aktivSideSimulaGpsData = value;
    this.hentInnsynSimulaGpsData();
  }
  get failedInnhold(): boolean {
    return this.failedIndekspasient || this.failedSmittekontakt || this.failedSmsvarsel;
  }
  set failedInnhold(failed: boolean) {
    this.failedIndekspasient = failed;
    this.failedSmittekontakt = failed;
    this.failedSmsvarsel = failed;
  }
  get showAlert(): boolean {
    return this.failedInnhold || this.failedLogg || this.failedSimuladatabruk || this.failedSimulagpsdata;
  }
  set showAlert(failed: boolean) {
    this.failedInnhold = failed;
    this.failedLogg = failed;
    this.failedSimuladatabruk = failed;
    this.failedSimulagpsdata = failed;
  }

  constructor(private innsynService: InnsynService, @Inject('BASE_URL') private baseUrl: string) {
  }

  ngOnInit(): void {
    }
}
