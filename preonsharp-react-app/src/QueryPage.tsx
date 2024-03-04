import { useState } from 'react';
import { Button } from '@mui/material-next';
import { TextField } from '@mui/material';

import ResultsView from './ResultsView';
import Page from './Page';
import Section from './Section';

import './QueryPage.css';

function QueryPage(props: { userName: string, password: string }) {
  const [inputValue, setInputValue] = useState('');
  const [responseData, setResponseData] = useState(null);
  const [error, setError] = useState(null);

  const elevation = 1;

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

  return <Page>
    <Section>
      <div className="request-field-button-container">
        <TextField
          required
          label="request"
          type='text'
          value={inputValue}
          onChange={(event: any) => setInputValue(event.target.value)}
          className="query-input"
        />
        <Button onClick={fetchData} className="query-button">
          send
        </Button>
      </div>
    </Section>

    <Section>
      <ResultsView data={responseData} />
      <p>Error: {error}</p>
    </Section>
  </Page >;
}

export default QueryPage;
