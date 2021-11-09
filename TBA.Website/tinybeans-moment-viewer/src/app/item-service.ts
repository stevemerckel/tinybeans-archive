import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ItemService {
    baseUrl = "http://localhost:44080/api/";         // HTTP
    //baseUrl = "http://localhost:44343/api/";       // HTTPS

    constructor(private http: HttpClient) {}

    getDayInfo() {
        console.log("received call to getDayInfo");
        return this.http
            .get<JournalMoment[]>(`${this.baseUrl}day`, { responseType:'json'})
            .pipe(map(response => {
                console.log('JSON.stringify of objects:', JSON.parse(JSON.stringify(response)));
                return response;
            }),
            catchError(this.handleError));
    }

    getSpecificDayInfo(date: String) {
        console.log(`received call to getSpecificDayInfo with '${date}'`);
        return this.http
            .get<JournalMoment[]>(`${this.baseUrl}day/${date}`, { responseType:'json'})
            .pipe(map(response => {
                console.log('JSON.stringify of objects:', JSON.parse(JSON.stringify(response)));
                return response;
            }),
            catchError(this.handleError));
    }

    getSummary() {
        console.log('received call to getSummary');
        return this.http
            .get<JournalSummary>(`${this.baseUrl}summary`, {responseType: 'json'})
            .pipe(map(response => {
                console.log('JSON.stringify of objects:', JSON.parse(JSON.stringify(response)));
                return response;
            }),
            catchError(this.handleError));
    }

    private handleError(error: HttpErrorResponse) {
        console.log(error);
        return throwError("The gremlins shrugged, please try again.");
    }
}

export interface JournalMoment {
    id: number;
    journalId: number;
    type: string;
    caption: string;
    localDate: string;
    url: string;
}

export interface JournalSummary {
    familyName: string;
    parents: string[];
    children: string[];
}