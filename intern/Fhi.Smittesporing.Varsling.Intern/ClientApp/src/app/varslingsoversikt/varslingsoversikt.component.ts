import { Component, OnInit } from '@angular/core';
import { IndekspasientService } from '../dataservice/indekspasient.service';
import { LookupService } from "../dataservice/lookup.service";
import { Indekspasient, IndekspasientFilter } from '../model/indekspasient';
import { Kommune } from "../model/kommune";
import { PagedList } from '../model/pagedList';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BekreftComponent } from '../dialoger/bekreft/bekreft.component';

@Component({
  selector: 'app-varslingsoversikt',
  templateUrl: './varslingsoversikt.component.html',
  styleUrls: ['./varslingsoversikt.component.css']
})
export class VarslingsoversiktComponent implements OnInit {
  private _aktivSide: number = 1;
  private _valgtKommune: Kommune;

  indekspasientListe: PagedList<Indekspasient>;
  kommuneList: Kommune[];
  valgtIndekspasient: Indekspasient;
  indekspasienterValgtTilGodkjenning: Indekspasient[] = [];
  laster = false;
  showGodkjenningAlert = false;

  get aktivSide(): number {
    return this._aktivSide;
  }
  set aktivSide(value: number) {
    this._aktivSide = value;
    this.hentIndekspasienter();
  }

  get valgtKommune(): Kommune {
    return this._valgtKommune;
  }
  set valgtKommune(value: Kommune) {
    this._valgtKommune = value;
    this._aktivSide = 1;
    this.hentIndekspasienter();
  }

  constructor(private indekspasientService: IndekspasientService, private lookupService: LookupService, private modalService: NgbModal) { }

  ngOnInit(): void {
    this.getKommuneList();
    this.hentIndekspasienter();
  }

  async hentIndekspasienter() {
    this.laster = true;

    const filter: IndekspasientFilter = {
      sideindeks: this._aktivSide -1,
      kreverGodkjenning: true,
      medSmittekontakt: true
    };

    if (this.valgtKommune) {
      filter.kommuneNr = this.valgtKommune.kommuneNr
    }

    this.indekspasientListe = await this.indekspasientService.hentListe(filter).toPromise();

    this.laster = false;
  }

  getKommuneList(): void {
    this.lookupService.getKommuneListe().subscribe(data => {
      this.kommuneList = data;
    });
  }

  velgIndekspasient(indekspasient: Indekspasient) {
    if (this.valgtIndekspasient == indekspasient) {
      this.valgtIndekspasient = null;
    } else {
      this.valgtIndekspasient = indekspasient;
    }
  }

  velgKommune(kommune: Kommune) {
    this.valgtKommune = kommune;
  }

  hentKommuneFor(indekspasient: Indekspasient): string {
    var kommune = this.kommuneList && this.kommuneList.find(k => k.kommuneId == indekspasient.kommuneId);
    return kommune ? kommune.navn : '-';
  }

  erBunkeTom(): boolean {
    return this.indekspasienterValgtTilGodkjenning.length < 1;
  }

  onChangeVelgAlle(isChecked: boolean) {
    if (isChecked) {
      this.indekspasientListe.resultater.forEach(p => {
        if (p.kanGodkjennesForVarsling && this.indekspasienterValgtTilGodkjenning.indexOf(p) < 0) {
          this.indekspasienterValgtTilGodkjenning.push(p);
        }
      });
    } else {
      this.indekspasienterValgtTilGodkjenning.length = 0;
    }
  }

  onChangeValgtSmittetilfelle(value: Indekspasient, isChecked: boolean) {
    if (isChecked) {
      this.indekspasienterValgtTilGodkjenning.push(value);
    } else {
      const index = this.indekspasienterValgtTilGodkjenning.findIndex(p => p.indekspasientId === value.indekspasientId);
      this.indekspasienterValgtTilGodkjenning.splice(index, 1);
    }
  }
  
  erValgt(value: Indekspasient): boolean {
    if (this.indekspasienterValgtTilGodkjenning != null) {
      return this.indekspasienterValgtTilGodkjenning.find(el => el.indekspasientId === value.indekspasientId) != null;
    } else {
      return false;
    }
  }

  async godkjennVarslingForValgte() {
    let modalRef = this.modalService.open(BekreftComponent, {ariaLabelledBy: 'modal-basic-title'});
    modalRef.componentInstance.tittel = 'Bekreft godkjenning';
    modalRef.componentInstance.beskrivelse = 'Er du sikker på at du vil godkjenne varsler? Alle kontakter som tilfredstiller varslingsreglene for de valgte indekspasientene og ikke allerede er varslet vil motta SMS-varsel.';
    let bleGodkjent: boolean = await modalRef.result;
    if (bleGodkjent) {
      let iderTilgodkjenneing = this.indekspasienterValgtTilGodkjenning.map(p => p.indekspasientId);
      try {
        await this.indekspasientService.godkjennVarslingBatch(iderTilgodkjenneing).toPromise();
        this.velgKommune(this.valgtKommune);
        this.valgtIndekspasient = null;
        this.indekspasienterValgtTilGodkjenning = [];
      } catch (e) {
        this.showGodkjenningAlert = true;
        window.scrollTo(0, 0);
      }
    }
  }
}
