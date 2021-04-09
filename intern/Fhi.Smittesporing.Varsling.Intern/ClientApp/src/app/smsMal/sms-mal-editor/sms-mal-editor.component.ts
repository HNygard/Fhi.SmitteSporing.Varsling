import { Component, OnInit, Input } from '@angular/core';
import { SmsMal, SmsFlettefelt } from 'src/app/model/smsMal';

// https://en.wikipedia.org/wiki/GSM_03.38#GSM_7-bit_default_alphabet_and_extension_table_of_3GPP_TS_23.038_.2F_GSM_03.38
const gsmBasicSet =
    "@£$¥èéùìòÇ\nØø\rÅåΔ_ΦΓΛΩΠΨΣΘΞÆæßÉ !\"#¤%&'()*+,-./0123456789:;<=>?¡ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÑÜ§¿abcdefghijklmnopqrstuvwxyzäöñüà";
const gsmExtensionSet =
    "\f|^€{}[]~\\€";

@Component({
  selector: 'app-sms-mal-editor',
  templateUrl: './sms-mal-editor.component.html',
  styleUrls: ['./sms-mal-editor.component.css']
})
export class SmsMalEditorComponent implements OnInit {
  _valgtFlettefelt: string = '';

  @Input()
  smsMal: SmsMal;

  @Input()
  flettefelter: SmsFlettefelt;

  get valgtFlettefelt(): string {
    return this._valgtFlettefelt;
  }
  set valgtFlettefelt(value: string) {
    this._valgtFlettefelt = value;
    // Sett inn i melding og resett
    setTimeout(() => {
      this.smsMal.meldingsinnhold += value;
      this._valgtFlettefelt = '';
    });
  }

  get harTegnUtenforGsmTegnsett(): boolean {
    for (var x = 0; x < this.smsMal.meldingsinnhold.length; x++)
    {
      var c = this.smsMal.meldingsinnhold.charAt(x);
      if (gsmExtensionSet.indexOf(c) < 0 && gsmBasicSet.indexOf(c) < 0) {
        return true;
      }
    }
    return false;
  }

  get smsLendgeInfo(): string {
    let melding = this.smsMal.meldingsinnhold;
    let gsmLendge = 0;
    let harSpesialtegn = false;
    for (var x = 0; x < melding.length; x++)
    {
      var c = melding.charAt(x);
      if (gsmExtensionSet.indexOf(c) >= 0) {
        gsmLendge += 2;
      } else if (gsmBasicSet.indexOf(c) >= 0) {
        gsmLendge += 1;
      } else {
        harSpesialtegn = true;
        break;
      }
    }

    let antallSegmenter, antallTegn;
    if (harSpesialtegn) {
      antallSegmenter = melding.length <= 70 ? 1 : Math.ceil(melding.length / 67);
      antallTegn = melding.length;
    } else {
      antallSegmenter = gsmLendge <= 160 ? 1 : Math.ceil(gsmLendge / 153);
      antallTegn = gsmLendge;
    }

    return `Estimert ${antallTegn} tegn fordelt på ${antallSegmenter} delmeldinger. Faktisk lengde kan variere avhengig av flettedata.`;
  }

  constructor() { }

  ngOnInit() {
  }

}
