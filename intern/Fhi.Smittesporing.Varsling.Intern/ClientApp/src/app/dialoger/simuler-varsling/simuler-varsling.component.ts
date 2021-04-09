import { Component, OnInit } from '@angular/core';
import { IndekspasientService } from 'src/app/dataservice/indekspasient.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SmittekontaktService } from 'src/app/dataservice/smittekontakt.service';
import { PagedList } from 'src/app/model/pagedList';
import { SmittekontaktListemodell } from 'src/app/model/smittekontakt';
import { Varslingssimulering, Varslingssimuleringdetaljer } from 'src/app/model/indekspasient';

@Component({
  selector: 'app-simuler-varsling',
  templateUrl: './simuler-varsling.component.html',
  styleUrls: ['./simuler-varsling.component.css']
})
export class SimulerVarslingComponent implements OnInit {
  private _indekspasientId: number;
  private _aktivSide: number = 1;
  private _lasterSmittekontakt: boolean = true;
  private _lasterSimulering: boolean = true;

  get indekspasientId(): number {
    return this._indekspasientId;
  }
  set indekspasientId(value: number) {
    this._indekspasientId = value;
    this.lastForIndekspasient();
  }

  get aktivSide(): number {
    return this._aktivSide;
  }
  set aktivSide(value: number) {
    this._aktivSide = value;
    this.hentSmittekontakter();
  }

  get laster(): boolean {
    return this._lasterSimulering || this._lasterSmittekontakt;
  };

  smittekontakter: PagedList<SmittekontaktListemodell>;
  simulering: Varslingssimulering;
  simuleringDetaljerMap: {[key: number]: Varslingssimuleringdetaljer};

  constructor(public activeModal: NgbActiveModal, private indekspasientService: IndekspasientService, private smittekontaktService: SmittekontaktService) {}

  ngOnInit(): void {
  }

  private lastForIndekspasient() {
    this.lastSimuleringsresultat();
    this.hentSmittekontakter();
  }

  private async lastSimuleringsresultat() {
    this._lasterSimulering = true;
    this.simulering = await this.indekspasientService.hentVarslingssimulering(this._indekspasientId).toPromise();
    this.simuleringDetaljerMap = this.simulering.detaljer.reduce((res, x) => {
      res[x.smittekontaktId] = x
      return res;
    }, {});
    this._lasterSimulering = false;
  }

  private async hentSmittekontakter(): Promise<any> {
    this._lasterSmittekontakt = true;
    this.smittekontakter = await this.smittekontaktService.hentSmittekontakterForIndekspasient(this._indekspasientId, this.aktivSide - 1).toPromise();
    this._lasterSmittekontakt = false;
  }

  async godkjenn() {
    await this.indekspasientService.godkjennVarsling(this._indekspasientId).toPromise();
    this.activeModal.close(true)
  }

}
