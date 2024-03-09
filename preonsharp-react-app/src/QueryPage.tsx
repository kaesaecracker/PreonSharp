import React, {useState} from 'react';
import QueryResultView from './QueryResultView';
import Page from './components/Page';
import {QueryServerResponse} from './types';
import SearchBox from "./SearchBox";
import Section from './components/Section';

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

    <Section>
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
    </Section>
  </Page>;
}

export default QueryPage;
