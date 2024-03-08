import {useState} from 'react';
import {Alert} from '@mui/material';

import ResultsView from './ResultsView';
import Page from './components/Page';
import {QueryServerResponse} from './types';
import SearchBox from "./SearchBox";

import './QueryPage.css';
import React from 'react';
import Section from './components/Section';

function QueryPage(props: { userName: string, password: string }) {
  const [responses, setResponses] = useState(() => new Map<string, QueryServerResponse>());
  const [responseOrder, setResponseOrder] = useState<string[]>(() => []);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async (text: string) => {
    let jsonData = responses.get(text);
    if (jsonData !== undefined) {
      let newResponseOrder = [...responseOrder];
      newResponseOrder.splice(responseOrder.indexOf(text), 1);
      newResponseOrder.splice(0, 0, text);
      setResponseOrder(newResponseOrder);
      return;
    }

    console.log('Anfrage senden mit Wert:', text);
    try {
      const response = await fetch(`https://preon-api.services.zerforschen.plus/preon?s=${text}`, {
        headers: new Headers({
          "Authorization": `Basic ${btoa(`${props.userName}:${props.password}`)}`
        }),
      });

      if (!response.ok) {
        setError('server did not respond with success code');
        return;
      }

      const jsonData = await response.json() as QueryServerResponse;
      const newResponses = new Map(responses);
      newResponses.set(text, jsonData);

      setResponses(newResponses);
      setResponseOrder([
        text,
        ...responseOrder
      ])
      setError(null); // Zurücksetzen des Fehlerzustands, wenn die Anfrage erfolgreich war
    } catch (e: any) {
      setError(e.toString());
    }
  };

  return <Page>
    <SearchBox onSearch={fetchData}/>

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
