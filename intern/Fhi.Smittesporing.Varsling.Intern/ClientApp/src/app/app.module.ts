import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MomentModule } from 'ngx-moment';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { VarslingsoversiktComponent } from './varslingsoversikt/varslingsoversikt.component';
import { ManglendeKontaktInfoComponent } from './manglendeKontaktInfo/manglendeKontaktInfo.component';

import { InnsynService } from './dataservice/innsyn.service';
import { IndekspasientService } from './dataservice/indekspasient.service';
import { SmittekontaktService } from './dataservice/smittekontakt.service';
import { LookupService } from "./dataservice/lookup.service";
import { StandardSmsMalComponent } from './smsMal/standard-sms-mal/standard-sms-mal.component';
import { SmsMalEditorComponent } from './smsMal/sms-mal-editor/sms-mal-editor.component';
import { WidgetNyeIndekspasienterComponent } from "./varslingsoversikt/widget-nyeindekspasienter/widget-nyeindekspasienter.component";

import { ChartsModule } from 'ng2-charts';
import { IndekspasienterComponent } from './indekspasienter/indekspasienter.component';
import { IndekspasientDetaljerComponent } from './indekspasienter/indekspasient-detaljer/indekspasient-detaljer.component';
import { SimulerVarslingComponent } from './dialoger/simuler-varsling/simuler-varsling.component';
import { InnsynComponent } from './innsyn/innsyn.component';
import { SmittekontaktDetaljerComponent } from './indekspasienter/indekspasient-detaljer/smittekontakt-detaljer/smittekontakt-detaljer.component';
import { DurationPipe } from './utils/duration.pipe';
import { VarslingsinfoComponent } from './dialoger/varslingsinfo/varslingsinfo.component';
import { BekreftComponent } from './dialoger/bekreft/bekreft.component';
import { EncodeHttpParamsInterceptor } from './utils/encodeHttpParamsInterceptor';
import { NationalidValidatorDirective } from './utils/norwegiannationalidvalidator';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    VarslingsoversiktComponent,
    StandardSmsMalComponent,
    SmsMalEditorComponent,
    WidgetNyeIndekspasienterComponent,
    ManglendeKontaktInfoComponent,
    IndekspasienterComponent,
    IndekspasientDetaljerComponent,
    SmittekontaktDetaljerComponent,
    SimulerVarslingComponent,
    VarslingsinfoComponent,
    InnsynComponent,
    BekreftComponent,
    DurationPipe,
    NationalidValidatorDirective
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ChartsModule,
    MomentModule,
    NgbModule,
    RouterModule.forRoot([
      { path: '', component: VarslingsoversiktComponent, pathMatch: 'full' },
      { path: 'sms-mal', component: StandardSmsMalComponent },
      { path: 'manglende-kontakt', component: ManglendeKontaktInfoComponent },
      { 
        path: 'indekspasienter',
        children: [
          { path: '', pathMatch: 'full', component: IndekspasienterComponent },
          { path: ':indekspasientId', component: IndekspasientDetaljerComponent },
          { path: ':indekspasientId/smittekontakter/:smittekontaktId', component: SmittekontaktDetaljerComponent }
        ]
      },
      {
        path: 'innsyn', component: InnsynComponent
      }
    ])
  ],
  providers: [
    IndekspasientService,
    InnsynService,
    SmittekontaktService,
    LookupService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: EncodeHttpParamsInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
