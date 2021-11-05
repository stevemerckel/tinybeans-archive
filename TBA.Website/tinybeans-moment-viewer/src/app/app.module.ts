import { NgModule } from '@angular/core';
import { BrowserModule, Title } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MomentComponent } from './components/moment/moment.component';
import { MomentListComponent } from './components/moment-list/moment-list.component';
import { FamilyInfoComponent } from './components/family-info/family-info.component';

@NgModule({
  declarations: [
    AppComponent,
    MomentComponent,
    MomentListComponent,
    FamilyInfoComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [
    Title
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
