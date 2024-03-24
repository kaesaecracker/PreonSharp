import {Guid} from './Guid';

export type SearchResult = {
  readonly originalQuery: string;
  readonly transformedQuery: string;
  readonly queryTime: string;
  readonly kind: SearchResultKind;
  readonly matches: TextMatch[];
};

export type SearchResultKind = 'None' | 'Exact' | 'ClosestName' | 'InternalId';

export type TextMatch = {
  readonly text: string;
  readonly entityIds: Guid[];
};
