import { Component, OnInit } from '@angular/core';
import { LookupService } from '../dataservice/lookup.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;

  visManglerKontaktinfo: boolean = false;

  constructor(private lookupService: LookupService) {

  }

  ngOnInit(): void {
    this.lookupService.hentFunksjoner().subscribe(x => {
      this.visManglerKontaktinfo = x.tillatAngiKontaktinfoManuelt;
    });
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
