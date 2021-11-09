import { Component, OnInit, Input, Output } from '@angular/core';
import { JournalMoment } from 'src/app/item-service';

@Component({
  selector: 'app-moment',
  templateUrl: './moment.component.html',
  styleUrls: ['./moment.component.scss']
})
export class MomentComponent implements OnInit {

  @Input() moment!: JournalMoment;

  constructor() { }

  ngOnInit(): void {
  }

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