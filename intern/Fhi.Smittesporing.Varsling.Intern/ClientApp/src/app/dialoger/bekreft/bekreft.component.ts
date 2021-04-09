import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-bekreft',
  templateUrl: './bekreft.component.html',
  styleUrls: ['./bekreft.component.css']
})
export class BekreftComponent implements OnInit {

  tittel: string;
  beskrivelse: string;
  bekreftTekst: string;
  avbrytTekst: string;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }

}
