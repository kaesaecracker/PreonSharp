import {useState} from 'react';
import {Alert} from '@mui/material';

import ResultsView from './ResultsView';
import Page from './components/Page';
import { QueryWithServerResponse, QueryServerResponse } from './types';
import SearchBox from "./SearchBox";

import './QueryPage.css';

function QueryPage(props: { userName: string, password: string }) {
  const [responseDatas, setResponseDatas] = useState<QueryWithServerResponse[]>([]);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async (text: string) => {
    const controller = new AbortController()
    console.log('Anfrage senden mit Wert:', text);
    const response = await fetch(`https://preon-api.services.zerforschen.plus/preon?s=${text}`, {
      signal: controller.signal,
      headers: new Headers({
        "Authorization": `Basic ${btoa(`${props.userName}:${props.password}`)}`
      }),
    });

    if (!response.ok) {
      setError('server did not respond with success code');
      return;
    }

    const jsonData = await response.json() as QueryServerResponse;
    setResponseDatas([{ response: jsonData, query: text }].concat(responseDatas));
    setError(null); // Zurücksetzen des Fehlerzustands, wenn die Anfrage erfolgreich war
  };

  return <Page>
    <SearchBox onSearch={fetchData}/>

    {error && (<Alert severity='error'>{error}</Alert>)}
    {responseDatas.map((props) => <ResultsView {...props}/>)}
  </Page>;
}

export default QueryPage;
