import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../core/api.config';
import { AggregatedPrice, PriceHistory } from './price.models';

@Injectable({
  providedIn: 'root'
})
export class PricesApiService {
  private readonly http = inject(HttpClient);

  getAggregated(instrument: string, timestampUtc: string): Observable<AggregatedPrice> {
    const params = new HttpParams().set('instrument', instrument).set('timestampUtc', timestampUtc);
    return this.http.get<AggregatedPrice>(`${API_BASE_URL}/prices/aggregated`, {
      params
    });
  }

  getHistory(instrument: string, fromUtc: string, toUtc: string): Observable<PriceHistory> {
    const params = new HttpParams().set('instrument', instrument).set('fromUtc', fromUtc).set('toUtc', toUtc);
    return this.http.get<PriceHistory>(`${API_BASE_URL}/prices/history`, {
      params
    });
  }
}
