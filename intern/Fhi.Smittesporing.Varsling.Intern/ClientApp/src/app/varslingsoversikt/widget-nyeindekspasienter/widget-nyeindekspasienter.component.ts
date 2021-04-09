import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AntallIndekspasienterRapport } from "../../model/indekspasienterRapport";
import { Kommune } from "../../model/kommune";
import { IndekspasientService } from "../../dataservice/indekspasient.service";

@Component({
  selector: 'app-widget-nyeindekspasienter',
  templateUrl: './widget-nyeindekspasienter.component.html'
})

export class WidgetNyeIndekspasienterComponent implements OnInit, OnChanges  {
  private _isInitialized: boolean = false;

  @Input() kommune: Kommune;
  @Input() totalOversikt: boolean;
  isLoading = true;
  chartType = 'line';
  lineChartColors: Array<any> = [
    { // orange
      backgroundColor: 'rgba(246,114,101,1)',
      borderColor: 'rgba(246,114,101,1)',
      pointBackgroundColor: 'rgba(246,114,101,1)',
      pointBorderColor: 'rgba(246,114,101,1)',
      pointHoverBackgroundColor: 'rgba(246,114,101,1)',
      pointHoverBorderColor: 'rgba(246,114,101,1)',
      fill: false
    },
    { // grey
      backgroundColor: 'rgba(148,159,177,1)',
      borderColor: 'rgba(148,159,177,1)',
      pointBackgroundColor: 'rgba(148,159,177,1)',
      pointBorderColor: 'rgba(148,159,177,1)',
      pointHoverBackgroundColor: 'rgba(148,159,177,1)',
      pointHoverBorderColor: 'rgba(148,159,177,0.8)',
      fill: false
    }
  ];

  antallSmittetilfelleRapport: AntallIndekspasienterRapport;

  constructor(private smittetilfelleService: IndekspasientService) { }

  ngOnInit(): void {
    this.getData();
    this._isInitialized = true;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this._isInitialized) {
      this.getData();
    }
  }

  async getData() {
    if (this.totalOversikt && !this.kommune) {
      this.isLoading = true;
      this.antallSmittetilfelleRapport = await this.smittetilfelleService.getAntallSmittetilfelleRapport().toPromise();
      this.isLoading = false;
      return;
    }

    if (this.totalOversikt && this.kommune) {
      return;
    }

    if (this.kommune) {
      this.isLoading = true;
      this.antallSmittetilfelleRapport = await this.smittetilfelleService.getAntallSmittetilfelleRapportForKommune(this.kommune.kommuneNr).toPromise();
      this.isLoading = false;
    }
  }
}
