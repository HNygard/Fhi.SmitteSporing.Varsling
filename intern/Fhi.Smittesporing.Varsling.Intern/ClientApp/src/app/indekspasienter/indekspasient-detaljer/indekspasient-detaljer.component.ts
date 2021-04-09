import { Component, OnInit } from '@angular/core';
import { SmittekontaktService } from 'src/app/dataservice/smittekontakt.service';
import { Smittekontakt, SmittekontaktListemodell } from 'src/app/model/smittekontakt';
import { ActivatedRoute } from '@angular/router';
import { PagedList } from 'src/app/model/pagedList';
import { Indekspasient } from 'src/app/model/indekspasient';
import { IndekspasientService } from 'src/app/dataservice/indekspasient.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SimulerVarslingComponent } from 'src/app/dialoger/simuler-varsling/simuler-varsling.component';
import { VarslingsinfoComponent } from 'src/app/dialoger/varslingsinfo/varslingsinfo.component';
import { BekreftComponent } from 'src/app/dialoger/bekreft/bekreft.component';

@Component({
  selector: 'app-indekspasient-detaljer',
  templateUrl: './indekspasient-detaljer.component.html',
  styleUrls: ['./indekspasient-detaljer.component.css']
})
export class IndekspasientDetaljerComponent implements OnInit {
  private _aktivSide: number = 1;
  private _indekspasientId: number;

  indekspasient: Indekspasient;
  smittekontakter: PagedList<SmittekontaktListemodell> = null;
  laster: boolean = false;

  get aktivSide(): number {
    return this._aktivSide;
  }
  set aktivSide(value: number) {
    this._aktivSide = value;
    this.lastSmittekontakterForIndekspasient();
  }

  constructor(
    private smittekontaktService: SmittekontaktService,
    private indekspasientService: IndekspasientService,
    private route: ActivatedRoute,
    private modalService: NgbModal) { }

  ngOnInit(): void {
    this.route.params.subscribe(async p => {
      this._indekspasientId = +p.indekspasientId;
      this.laster = true;
      await this.lastIndekspasient();
      await this.lastSmittekontakterForIndekspasient();
      this.laster = false;
    })
  }

  async lastIndekspasient(): Promise<any> {
    this.indekspasient = await this.indekspasientService.hentForId(this._indekspasientId).toPromise();
  }

  async lastSmittekontakterForIndekspasient(): Promise<any> {
    this.smittekontakter = await this.smittekontaktService.hentSmittekontakterForIndekspasient(this._indekspasientId, this._aktivSide - 1).toPromise();
  }

  async dekrypterIndekspasient(): Promise<any> {
    this.indekspasientService.dekrypter(this.indekspasient);
  }

  kanDekryptereIndekspasient(): boolean {
    return this.indekspasient && this.indekspasientService.kanDekrypteres(this.indekspasient);
  }

  fodselsnummerForPasient(): string {
    return this.indekspasient
      ? this.indekspasientService.fodselsnummerFor(this.indekspasient)
      : '';
  }

  telefonnummerForPasient(): string {
    return this.indekspasient
      ? this.indekspasientService.telefonnummerFor(this.indekspasient)
      : '';
  }

  async markerVarslingFerdig(): Promise<any> {
    await this.indekspasientService.markerFerdig(this._indekspasientId).toPromise();
    await this.lastIndekspasient();
  }

  erFerdig() {
    return this.indekspasient.varslingsstatus == 'Ferdig';
  }

  kanSettesFerdig(): boolean {
    return this.indekspasient.varslingsstatus == 'TilGodkjenning' && this.indekspasient.status != 'Registrert';
  }

  async simulerVarsling() {
    let modalRef = this.modalService.open(SimulerVarslingComponent, {ariaLabelledBy: 'modal-basic-title'});
    modalRef.componentInstance.indekspasientId = this.indekspasient.indekspasientId;
    var bleGodkjent: boolean = await modalRef.result;
    if (bleGodkjent) {
      this.lastIndekspasient();
      this.lastSmittekontakterForIndekspasient();
    }
  }

  async godkjennVarsling(): Promise<any> {
    let modalRef = this.modalService.open(BekreftComponent, {ariaLabelledBy: 'modal-basic-title'});
    modalRef.componentInstance.tittel = 'Bekreft godkjenning';
    modalRef.componentInstance.beskrivelse = 'Er du sikker på at du vil godkjenne varsler? Alle kontakter som tilfredstiller varslingsreglene og ikke allerede er varslet vil motta SMS-varsel.';
    let bleGodkjent: boolean = await modalRef.result;
    if (bleGodkjent) {
      await this.indekspasientService.godkjennVarsling(this.indekspasient.indekspasientId).toPromise();
      await this.lastIndekspasient();
      this.lastSmittekontakterForIndekspasient();
    }
  }

  kanGodkjennes(): boolean {
    return this.indekspasient.varslingsstatus == 'TilGodkjenning' && this.indekspasient.status == 'SmitteKontakt';
  }

  erKontaktDekryptert(smittekontakt: Smittekontakt): boolean {
    return this.smittekontaktService.erKontaktDekryptert(smittekontakt);
  }

  telefonnummerForKontakt(smittekontakt: Smittekontakt): string {
    return this.smittekontaktService.telefonnummerForKontakt(smittekontakt);
  }

  dekrypterKontakt(smittekontakt: Smittekontakt) {
    return this.smittekontaktService.dekrypterKontakt(smittekontakt);
  }

  visVarselinfo (smittekontaktId: number) {
    let modalRef = this.modalService.open(VarslingsinfoComponent, {ariaLabelledBy: 'modal-basic-title'});
    modalRef.componentInstance.smittekontaktId = smittekontaktId;
  }

  hentStedsinfoFor(smittekontakt: Smittekontakt): string {
    var pois = Object.keys(smittekontakt.interessepunkter);
    var maxPoi = pois
      .sort((a, b) => smittekontakt.interessepunkter[b] - smittekontakt.interessepunkter[a])[0];
    return maxPoi
      ? maxPoi + (pois.length > 1 ? ` (+${pois.length - 1})` : '')
      : '-';
  }

  async sendVarsel(smittekontakt: Smittekontakt) {
    let modalRef = this.modalService.open(BekreftComponent, {ariaLabelledBy: 'modal-basic-title'});
    modalRef.componentInstance.tittel = 'Bekreft SMS-varsel';
    modalRef.componentInstance.beskrivelse = 'Er du sikker på at du vi sende et SMS-varsel til denne smittekontakten? Dette varselet vil sendes uavhengig av de vanlige varslingsreglene.';
    if (await modalRef.result) {
      await this.smittekontaktService.sendVarsel(smittekontakt.smittekontaktId).toPromise();
      await this.lastSmittekontakterForIndekspasient();
    }
  }

}
