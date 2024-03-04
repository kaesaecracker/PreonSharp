import { KeyboardEvent, useState } from 'react';
import { TextField, Button } from '@mui/material';

import ResultsView from './ResultsView';
import Page from './Page';
import Section from './Section';

import './QueryPage.css';

function QueryPage(props: { userName: string, password: string }) {
  const [inputValue, setInputValue] = useState('');
  const [responseData, setResponseData] = useState(null);
  const [error, setError] = useState(null);

  const fetchData = async () => {
    try {
      const controller = new AbortController()
      console.log('Anfrage senden mit Wert:', inputValue);
      const response = await fetch(`https://preon-api.services.zerforschen.plus/preon?s=${inputValue}`, {
        //mode: 'no-cors',
        signal: controller.signal,
        headers: new Headers({
          "Authorization": `Basic ${btoa(`${props.userName}:${props.password}`)}`
        }),
      });
      if (!response.ok) {
        throw new Error('Fehler beim Abrufen der Daten');
      }
      const jsonData = await response.json();
      setResponseData(jsonData);
      setError(null); // Zurücksetzen des Fehlerzustands, wenn die Anfrage erfolgreich war
    } catch (error: any) {
      setError(error.message); // Setzen des Fehlerzustands, wenn ein Fehler auftritt
    }
  };

  const onKeyDown = (event: KeyboardEvent) => {
    if (event.key === 'Enter') {
      fetchData();
    }
  };

  return <Page>
    <div className="request-field-button-container">
      <TextField
        required
        label="request"
        type='text'
        value={inputValue}
        onChange={(event: any) => setInputValue(event.target.value)}
        className="query-input"
        onKeyDown={onKeyDown}
      />
      <Button onClick={fetchData} variant='outlined'>
        send
      </Button>
    </div>

    <Section>
      {error && (<p>Error: {error}</p>)}
      <ResultsView data={responseData} />
    </Section>
  </Page >;
}

export default QueryPage;
