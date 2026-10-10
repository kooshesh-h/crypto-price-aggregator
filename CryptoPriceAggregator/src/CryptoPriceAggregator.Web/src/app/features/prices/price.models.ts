export interface SourcePrice {
  sourceName: string;
  price: number;
}

export interface AggregatedPrice {
  instrument: string;
  timestampUtc: string;
  price: number;
  isFromCache: boolean;
  sourcePrices: SourcePrice[];
}

export interface PriceHistoryItem {
  timestampUtc: string;
  price: number;
  sourcePrices: SourcePrice[];
}

export interface PriceHistory {
  instrument: string;
  fromUtc: string;
  toUtc: string;
  items: PriceHistoryItem[];
}
