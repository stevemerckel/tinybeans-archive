import { Component, Input, OnInit } from '@angular/core';
import { JournalMoment } from 'src/app/item-service';

@Component({
  selector: 'app-moment-list',
  templateUrl: './moment-list.component.html',
  styleUrls: ['./moment-list.component.scss']
})
export class MomentListComponent implements OnInit {

  @Input() moment!: JournalMoment;

  constructor() { }

  ngOnInit(): void { }

  public getDisplayName() : string {
      var url = this.moment.url;  
      if (url == null || url == '') {
          return '';
      }

      
      var splitIndex = url.lastIndexOf('/');
      if (splitIndex < 0) {
          return url;
      }

      return url.substring(splitIndex);
  }

  public hasCaption() : boolean {
      var caption = this.moment.caption;
      return caption !== undefined && caption !== null && caption !== '';
  }
}