import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SmittekontaktService } from 'src/app/dataservice/smittekontakt.service';
import { SmsVarsel } from 'src/app/model/smittekontakt';

@Component({
  selector: 'app-varslingsinfo',
  templateUrl: './varslingsinfo.component.html',
  styleUrls: ['./varslingsinfo.component.css']
})
export class VarslingsinfoComponent implements OnInit {

  private _smittekontaktId: number = null;

  smsVarsler: SmsVarsel[] = [];

  get smittekontaktId(): number {
    return this._smittekontaktId;
  }
  set smittekontaktId(value: number) {
    this._smittekontaktId = value;
    this.lastVarslerForSmittekontakt();
  }

  constructor(public activeModal: NgbActiveModal, private smittekontaktService: SmittekontaktService) {}

  ngOnInit(): void {
  }

  async lastVarslerForSmittekontakt() {
    if (!this._smittekontaktId) {
      this.smsVarsler = [];
    } else {
      this.smsVarsler = await this.smittekontaktService.hentVarselInfo(this._smittekontaktId).toPromise();
    }
  }

}
