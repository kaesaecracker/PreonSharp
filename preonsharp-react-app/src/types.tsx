export type QueryServerResponse = {
  executionTime: string;
  foundIds: {
    name: string;
    ids: string[];
  }[];
};

export type QueryWithServerResponse = {
  id: number;
  query: string;
  response: QueryServerResponse;
};