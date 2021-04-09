import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SmittekontaktService } from 'src/app/dataservice/smittekontakt.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Smittekontakt } from 'src/app/model/smittekontakt';

@Component({
  selector: 'app-smittekontakt-detaljer',
  templateUrl: './smittekontakt-detaljer.component.html',
  styleUrls: ['./smittekontakt-detaljer.component.css']
})
export class SmittekontaktDetaljerComponent implements OnInit {
  private _aktiveKart: {[detaljerId: number]: boolean} = {};

  indekspasientId: number;
  smittekontaktId: number;

  oppsummertKontaktGrafUrl: string;
  gpsHistPlotUrl: string;

  laster: boolean = true;

  smittekontakt: Smittekontakt;


  constructor(private route: ActivatedRoute, private smittekontaktService: SmittekontaktService, private domSanitizer: DomSanitizer) { }

  ngOnInit(): void {
    this.route.params.subscribe(p => {
      this.indekspasientId = +p.indekspasientId;
      this.smittekontaktId = +p.smittekontaktId;
      this.lastData();
    });
  }

  async lastData() {
    this.laster = true;
    this.oppsummertKontaktGrafUrl = this.smittekontaktService.lagOppsummertKontaktGrafUrl(this.smittekontaktId);
    this.gpsHistPlotUrl = this.smittekontaktService.lagGpsHistPlotUrlUrl(this.smittekontaktId);
    this.smittekontakt = await this.smittekontaktService.hentSmittekontakt(this.smittekontaktId).toPromise();
    this.laster = false;
  }

  hentKartUrlForDag(smittekontaktDagId: number): SafeResourceUrl {
    return this.domSanitizer.bypassSecurityTrustResourceUrl(
      this.smittekontaktService.lagKartForDagHtmlUrl(this.smittekontaktId, smittekontaktDagId));
  }

  kartForDetaljerAktivt(smittekontaktDagId: number): boolean {
    return !!this._aktiveKart[smittekontaktDagId];
  }

  aktiverKartForDetaljer(smittekontaktDagId: number) {
    return this._aktiveKart[smittekontaktDagId] = true;
  }

}
