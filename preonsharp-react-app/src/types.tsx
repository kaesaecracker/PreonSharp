export type QueryServerResponse = {
  executionTime: string;
  foundIds: {
    name: string;
    ids: string[];
  }[];
};

export type QueryWithServerResponse = {
  query: string;
  response: QueryServerResponse;
};