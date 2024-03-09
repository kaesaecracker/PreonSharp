import React, {useState} from 'react';
import {Button, Paper} from "@mui/material";

import QueryResultView from './QueryResultView';
import Page from './components/Page';
import {QueryServerResponse} from './types';
import SearchBox from "./SearchBox";

import './QueryPage.css';

async function fetchData(
  text: string,
  userName: string,
  password: string
): Promise<QueryServerResponse> {
  const response = await fetch(`https://preon-api.services.zerforschen.plus/preon?s=${text}`, {
    headers: new Headers({
      "Authorization": `Basic ${btoa(`${userName}:${password}`)}`
    }),
  });

  if (!response.ok)
    throw new Error('server did not respond with success code');

  return await response.json() as QueryServerResponse;
}

async function onSearch(
  text: string,
  userName: string,
  password: string,
  setResponseOrder: (value: (prevState: string[]) => string[]) => void,
  setRunningQueries: (value: (prevState: Set<string>) => Set<string>) => void,
  setResponses: (value: (revState: Map<string, QueryServerResponse>) => Map<string, QueryServerResponse>) => void,
  setErrors: (value: (prevState: Map<string, string>) => Map<string, string>) => void
) {
  text = text.trim();

  setResponseOrder(responseOrder => {
    let newResponseOrder = [...responseOrder];
    if (responseOrder.indexOf(text) >= 0)
      newResponseOrder.splice(responseOrder.indexOf(text), 1);
    newResponseOrder.splice(0, 0, text);
    return newResponseOrder;
  });

  try {
    setRunningQueries(prevState => {
      const newState = new Set(prevState);
      newState.add(text);
      return newState;
    })

    const response = await fetchData(text, userName, password);

    setResponses(responses => {
      const newResponses = new Map(responses);
      newResponses.set(text, response);
      return newResponses;
    });
    setErrors(errors => {
      if (!errors.has(text))
        return errors;

      const newErrors = new Map(errors);
      newErrors.delete(text);
      return newErrors;
    });
  } catch (e: any) {
    setErrors(errors => {
      const newErrors = new Map(errors);
      newErrors.set(text, e.toString());
      return newErrors;
    });
  } finally {
    setRunningQueries(prevState => {
      const newState = new Set(prevState);
      newState.delete(text);
      return newState;
    })
  }
}

function QueryPage(props: { userName: string, password: string }) {
  const [responses, setResponses] = useState(() => new Map<string, QueryServerResponse>());
  const [responseOrder, setResponseOrder] = useState<string[]>(() => []);
  const [errors, setErrors] = useState(() => new Map<string, string>());
  const [runningQueries, setRunningQueries] = useState<Set<string>>(() => new Set<string>());

  return <Page>
    <SearchBox
      onSearch={async text => await onSearch(text, props.userName, props.password, setResponseOrder, setRunningQueries, setResponses, setErrors)}/>

    <Paper variant="outlined" sx={{
      display: 'flex',
      flexDirection: 'row',
      padding: '10px',
      gap: '20px'
    }}>
      <Button onClick={() => {
        for (let i = 0; i < 20; i++) {
          onSearch(`test${i}`, props.userName, props.password, setResponseOrder, setRunningQueries, setResponses, setErrors);
        }
      }}>torture test</Button>

      <Button onClick={() => {
        // TODO: keep running queries
        setResponses(new Map());
        setResponseOrder([]);
        setErrors(new Map());
        setRunningQueries(new Set());
      }}>clear</Button>
    </Paper>

    <div style={{
      display: "flex",
      flexDirection: "column"
    }}>
      {
        responseOrder.map((text) => {
          return <QueryResultView
            key={text}
            query={text}
            response={responses.get(text)}
            error={errors.get(text)}
            running={runningQueries.has(text)}
          />;
        })
      }
    </div>
  </Page>;
}

export default QueryPage;
