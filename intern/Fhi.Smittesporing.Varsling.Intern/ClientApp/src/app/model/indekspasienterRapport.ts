import { Periode } from './periode';
import { ChartData } from './chartData';

export class AntallIndekspasienterRapport {
  periode: Periode;
  generertdato: Date;
  sisteOpprettet: Date;
  antallSisteDag: number;
  chartData: ChartData;
}

export class RapportFilter {
  fraOgMed?: string;
  tilOgMed?: string;
  kommuneNr?: string;
}
