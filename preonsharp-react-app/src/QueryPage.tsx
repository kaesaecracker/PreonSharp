import {useState} from 'react';
import {Alert} from '@mui/material';

import ResultsView from './ResultsView';
import Page from './components/Page';
import {QueryServerResponse} from './types';
import SearchBox from "./SearchBox";

import './QueryPage.css';
import React from 'react';
import Section from './components/Section';


async function fetchData(
  text: string,
  userName: string,
  password: string,
  setError: (value: string | null) => void,
  setResponses: (value: (prevState: Map<string, QueryServerResponse>) => Map<string, QueryServerResponse>) => void,
  setResponseOrder: (value: (prevState: string[]) => string[]) => void
) {
  console.log('Anfrage senden mit Wert:', text);
  try {
    const response = await fetch(`https://preon-api.services.zerforschen.plus/preon?s=${text}`, {
      headers: new Headers({
        "Authorization": `Basic ${btoa(`${userName}:${password}`)}`
      }),
    });

    if (!response.ok) {
      setError('server did not respond with success code');
      return;
    }

    const jsonData = await response.json() as QueryServerResponse;

    setResponses(responses => {
      const newResponses = new Map(responses);
      newResponses.set(text, jsonData);
      return newResponses;
    });
    setResponseOrder(responseOrder => [
      text,
      ...responseOrder
    ])
    setError(null); // Zurücksetzen des Fehlerzustands, wenn die Anfrage erfolgreich war
  } catch (e: any) {
    setError(e.toString());
  }
}

function QueryPage(props: { userName: string, password: string }) {
  const [responses, setResponses] = useState(() => new Map<string, QueryServerResponse>());
  const [responseOrder, setResponseOrder] = useState<string[]>(() => []);
  const [error, setError] = useState<string | null>(null);

  const onSearch = async (text: string) => {
    let jsonData = responses.get(text);
    if (jsonData !== undefined) {
      setResponseOrder(responseOrder => {
        let newResponseOrder = [...responseOrder];
        newResponseOrder.splice(responseOrder.indexOf(text), 1);
        newResponseOrder.splice(0, 0, text);
        return newResponseOrder;
      });
      return;
    }

    await fetchData(text, props.userName, props.password, setError, setResponses, setResponseOrder);
  };

  return <Page>
    <SearchBox onSearch={onSearch}/>

    {error && (<Alert severity='error'>{error}</Alert>)}

    <Section>
      {
        responseOrder.map((text) =>
          <ResultsView key={text} query={text} response={responses.get(text)!}/>)
      }
    </Section>
  </Page>;
}

export default QueryPage;
