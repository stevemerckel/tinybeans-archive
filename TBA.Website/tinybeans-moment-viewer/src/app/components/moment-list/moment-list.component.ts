import { Component, OnInit } from '@angular/core';
import { ItemService, JournalMoment } from 'src/app/item-service';

@Component({
  selector: 'app-moment-list',
  templateUrl: './moment-list.component.html',
  styleUrls: ['./moment-list.component.scss']
})
export class MomentListComponent implements OnInit {

  moments: JournalMoment[] = [];
  targetDate = '';
  isLoading = false;

  constructor(private itemService: ItemService) { }

  ngOnInit(): void { }



  getSelectedDate() {
    return new Date(this.targetDate + 'T00:00:00');
  }

  getDayMoments() {
    var dateToUse = this.targetDate === undefined || this.targetDate === ''
      ? new Date()
      : new Date(this.targetDate);

    this.targetDate = dateToUse.toISOString().split('T')[0];
    this.isLoading = true;
    this.itemService.getSpecificDayInfo(this.targetDate)
      .subscribe(moments => {
        const delayInMs = 3000;  // artificial padding to simulate large server-side action
        console.log('sleeping for ', delayInMs, ' ms');
        setTimeout(() => {
          this.isLoading = false;
          console.log(`done with sleeping for ${delayInMs} ms`);
        }, delayInMs);
      });
  }

  changeDate() {
    var found = prompt('What date would you like?', this.targetDate);
    if (found === undefined || found === '') {
      return;
    }
    console.log('found ', found);
    var converted = Date.parse(found?.trim() ?? '');
    if (converted === undefined || converted === NaN) {
      return;
    }
    console.log('converted ', converted);
    this.targetDate = new Date(found?.trim() ?? '').toISOString().split('T')[0];
    console.log(this.targetDate);
    this.getDayMoments();
  }

  // @ts-ignore -- reason: ignore for now until proper type can be decided
  dateSelected(change, event) {
    console.log(event.value.toString());
    console.log(event.value);
    var converted = Date.parse(event.value);
    console.log('converted ', converted);
    this.targetDate = new Date(event.value).toISOString().split('T')[0];
    this.getDayMoments();
  }
}