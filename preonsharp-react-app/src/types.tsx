export type QueryServerResponse = {
  executionTime: string;
  foundIds: {
    name: string;
    ids: string[];
  }[];
};
