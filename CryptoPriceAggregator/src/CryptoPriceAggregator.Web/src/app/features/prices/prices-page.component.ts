import { CommonModule } from '@angular/common';
import { Component, OnDestroy, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { apiErrorMessage } from '../../core/api-error';
import { AggregatedPrice, PriceHistory, SourcePrice } from './price.models';
import { PricesApiService } from './prices-api.service';

@Component({
  selector: 'app-prices-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './prices-page.component.html',
  styleUrl: './prices-page.component.css'
})
export class PricesPageComponent implements OnDestroy {
  private readonly pricesApi = inject(PricesApiService);
  private readonly subscriptions = new Subscription();

  instrument = 'BTC/USD';
  readonly hours = Array.from(
    {
      length: 24
    },
    (_, hour) => String(hour).padStart(2, '0')
  );
  timestampDate = this.utcDate(new Date());
  timestampHour = this.utcHour(new Date());
  fromDate = this.utcDate(new Date(Date.now() - 24 * 60 * 60 * 1000));
  fromHour = this.utcHour(new Date(Date.now() - 24 * 60 * 60 * 1000));
  toDate = this.utcDate(new Date());
  toHour = this.utcHour(new Date());

  aggregated: AggregatedPrice | null = null;
  history: PriceHistory | null = null;
  aggregatedLoading = false;
  historyLoading = false;
  aggregatedError = '';
  historyError = '';

  get todayUtc(): string {
    return this.utcDate(new Date());
  }

  getAggregated(): void {
    this.aggregatedError = '';
    const instrument = this.instrument.trim();
    if (!instrument || !this.isUtcHour(this.timestampDate, this.timestampHour)) {
      this.aggregatedError = 'Enter an instrument, date (UTC), and hour (UTC).';
      return;
    }
    if (this.timestampDate > this.todayUtc) {
      this.aggregatedError = 'Date (UTC) cannot be after today.';
      return;
    }
    this.aggregated = null;
    this.aggregatedLoading = true;
    this.subscriptions.add(
      this.pricesApi.getAggregated(instrument, this.toUtcIso(this.timestampDate, this.timestampHour)).subscribe({
        next: result => {
          this.aggregated = result;
          this.aggregatedLoading = false;
        },
        error: error => {
          this.aggregatedError = apiErrorMessage(error);
          this.aggregatedLoading = false;
        }
      })
    );
  }

  getHistory(): void {
    this.historyError = '';
    const instrument = this.instrument.trim();
    if (!instrument || !this.isUtcHour(this.fromDate, this.fromHour) || !this.isUtcHour(this.toDate, this.toHour)) {
      this.historyError = 'Enter an instrument, From date and hour (UTC), and To date and hour (UTC).';
      return;
    }
    if (this.fromDate > this.todayUtc || this.toDate > this.todayUtc) {
      this.historyError = 'History dates (UTC) cannot be after today.';
      return;
    }
    const fromUtc = this.toUtcIso(this.fromDate, this.fromHour);
    const toUtc = this.toUtcIso(this.toDate, this.toHour);
    if (fromUtc > toUtc) {
      this.historyError = 'From UTC must be before or equal to To UTC.';
      return;
    }
    this.history = null;
    this.historyLoading = true;
    this.subscriptions.add(
      this.pricesApi.getHistory(instrument, fromUtc, toUtc).subscribe({
        next: result => {
          this.history = result;
          this.historyLoading = false;
        },
        error: error => {
          this.historyError = apiErrorMessage(error);
          this.historyLoading = false;
        }
      })
    );
  }

  sourcePrice(sources: SourcePrice[], name: string): number | null {
    return sources.find(source => source.sourceName.toLowerCase() === name.toLowerCase())?.price ?? null;
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private utcDate(date: Date): string {
    return date.toISOString().slice(0, 10);
  }

  private utcHour(date: Date): string {
    return date.toISOString().slice(11, 13);
  }

  private isUtcHour(dateValue: string, hour: string): boolean {
    if (!/^\d{4}-\d{2}-\d{2}$/.test(dateValue) || !this.hours.includes(hour)) {
      return false;
    }
    const date = new Date(this.toUtcIso(dateValue, hour));
    return !Number.isNaN(date.getTime()) && date.toISOString() === this.toUtcIso(dateValue, hour).replace('Z', '.000Z');
  }

  private toUtcIso(dateValue: string, hour: string): string {
    return `${dateValue}T${hour}:00:00Z`;
  }
}
