import { Component, OnInit, OnDestroy } from '@angular/core';
import { SmsMalService } from 'src/app/dataservice/smsMal.service';
import { SmsMal, SmsFlettefelt, SmsHendelse, SmsFletteinnstillinger } from 'src/app/model/smsMal';
import { interval, Subscription } from 'rxjs';
import { v4 as uuidv4 } from 'uuid';
import { Kommune } from 'src/app/model/kommune';
import { LookupService } from 'src/app/dataservice/lookup.service';

@Component({
  selector: 'app-standard-sms-mal',
  templateUrl: './standard-sms-mal.component.html',
  styleUrls: ['./standard-sms-mal.component.css']
})
export class StandardSmsMalComponent implements OnInit, OnDestroy {
  private _aktivTestmeldingSub: Subscription;
  private _valgtKommuneSmsInfo: Kommune = null;

  flettefelter: SmsFlettefelt[] = [];
  testmeldingshendelser: SmsHendelse[] = [];

  kommuner: Kommune[] = [];

  smsMalStatus: string = null;
  smsMal: SmsMal;
  erLagret: boolean;

  flettinnstillingerStatus: string = null;
  fletteinnstillinger: SmsFletteinnstillinger = null;

  kommuneInfoStatus: string = null;
  get valgtKommuneSmsInfo(): Kommune {
    return this._valgtKommuneSmsInfo;
  };
  set valgtKommuneSmsInfo(value: Kommune) {
    this._valgtKommuneSmsInfo = value;
    this.valgtKommuneSmsInfoTekst = value.smsFletteinfo;
    this.kommuneInfoStatus = null;
  };
  valgtKommuneSmsInfoTekst: string = '';


  testTelefonnummer: string = '';
  testKommune: Kommune = null;
  testDatoSisteKontakt: string = null;
  testRisikokategori: string = null;
  risikokategorier: string[] = ['low', 'medium', 'high']

  senderTestmelding: boolean;
  testmeldingFeilet: boolean;
  testmeldingSendt: boolean;

  constructor(private smsMalService: SmsMalService, private lookupService: LookupService) { }

  async ngOnInit() {
    this.flettefelter = await this.smsMalService.hentFlettefelter().toPromise();
    this.fletteinnstillinger = await this.smsMalService.hentFletteinnstillinger().toPromise();
    this.kommuner = await this.lookupService.getKommuneListe().toPromise();
    try {
      this.smsMal = await this.smsMalService.hentStandardSmsMal().toPromise();
      this.erLagret = true;
    } catch (e) {
      // Antar at sms-mal bare ikke er opprettet enda..
      console.error(e);
      this.smsMal = {
        avsender: 'FHI',
        meldingsinnhold: ''
      };
      this.erLagret = false;
    }
  }

  ngOnDestroy(): void {
    if (this._aktivTestmeldingSub) {
      this._aktivTestmeldingSub.unsubscribe();
      this._aktivTestmeldingSub = null;
    }
  }

  async sendTestmelding() {
    if (this._aktivTestmeldingSub) {
      this._aktivTestmeldingSub.unsubscribe();
      this._aktivTestmeldingSub = null;
    }
    this.senderTestmelding = true;
    this.testmeldingFeilet = false;
    this.testmeldingSendt = false;
    this.testmeldingshendelser = [];
    try {
      const smsRef = uuidv4();
      await this.smsMalService.sendTestmeldingStandardmal({
        referanse: smsRef,
        telefonnummer: this.testTelefonnummer,
        kommuneId: this.testKommune ? this.testKommune.kommuneId : null,
        risikokategori: this.testRisikokategori,
        datoSisteKontakt: this.testDatoSisteKontakt
      }).toPromise();
      this._aktivTestmeldingSub = interval(5000).subscribe(async _ => await this.oppdaterHendelser(smsRef));
      await this.oppdaterHendelser(smsRef);
      this.testmeldingSendt = true;
    } catch {
      this.testmeldingFeilet = true;
    }
    this.senderTestmelding = false;
  }

  private async oppdaterHendelser(smsRef: string): Promise<any> {
    this.testmeldingshendelser = await this.smsMalService.hentTestmeldingshendelser(smsRef).toPromise();
    if (this.testmeldingshendelser.length && this.testmeldingshendelser[0].gjeldeneStatus == 'Levert' && this._aktivTestmeldingSub) {
      this._aktivTestmeldingSub.unsubscribe();
      this._aktivTestmeldingSub = null;
    }
    return this.testmeldingshendelser;
  }

  async lagreSmsMal() {
    this.smsMalStatus = 'Lagrer...';
    this.erLagret = false;
    try {
      await this.smsMalService.lagreStandardSmsMal(this.smsMal).toPromise();
      this.erLagret = true;
      this.smsMalStatus = null;
    } catch {
      this.smsMalStatus = 'Noe gikk galt ved lagring.';
    }
  }

  async lagreKommunetekst() {
    this.kommuneInfoStatus = 'Lagrer...';
    try {
      const nyTekst = this.valgtKommuneSmsInfoTekst;
      await this.smsMalService.oppdaterSmsInfoForKommune(this.valgtKommuneSmsInfo.kommuneId, nyTekst).toPromise();
      this.valgtKommuneSmsInfo.smsFletteinfo = nyTekst;
      this.kommuneInfoStatus = null;
    } catch {
      this.kommuneInfoStatus = 'Noe gikk galt ved lagring.';
    }
  }

  async lagreFletteinnstillinger() {
    this.flettinnstillingerStatus = 'Lagrer...';
    try {
      await this.smsMalService.oppdaterFletteinnstillinger(this.fletteinnstillinger).toPromise();
      this.flettinnstillingerStatus = null;
    } catch {
      this.flettinnstillingerStatus = 'Noe gikk galt ved lagring.';
    }
  }

}
