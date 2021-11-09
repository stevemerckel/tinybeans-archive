import { Component, OnInit } from '@angular/core';
import { ItemService, JournalSummary } from 'src/app/item-service';

@Component({
  selector: 'app-family-info',
  templateUrl: './family-info.component.html',
  styleUrls: ['./family-info.component.scss']
})
export class FamilyInfoComponent implements OnInit {

  isMemberNamesHidden = true;
  journalSummary!: JournalSummary;

  constructor(private itemService: ItemService) { }

  ngOnInit(): void {
    //this.getJournalSummary();
  }

  getJournalSummary() {
    this.itemService.getSummary()
      .subscribe((summary: JournalSummary) => {
        this.journalSummary = summary;
    });
  }

  toggleMemberVisibility() {
    console.log(`changing ${this.isMemberNamesHidden} to ${!this.isMemberNamesHidden}`);
    this.isMemberNamesHidden = !this.isMemberNamesHidden;
  }

}