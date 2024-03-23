import {useState} from 'react';
import {Button, Typography} from "@mui/material";
import QueryResultView from './QueryResultView';
import Page from './components/Page';
import {QueryServerResponse} from './types';
import SearchBox from "./SearchBox";
import './MainPage.css';
import Section from "./components/Section.tsx";
import {Credentials} from "./models/Settings.ts";
import {fetchData} from "./fetchData.ts";
import {Guid} from "./models/Guid.ts";

async function onSearch(
  text: string,
  credentials: Credentials,
  setResponseOrder: (value: (prevState: string[]) => string[]) => void,
  setRunningQueries: (value: (prevState: Set<string>) => Set<string>) => void,
  setResponses: (value: (revState: Map<string, QueryServerResponse>) => Map<string, QueryServerResponse>) => void,
  setErrors: (value: (prevState: Map<string, string>) => Map<string, string>) => void
) {
  text = text.trim();

  setResponseOrder(responseOrder => [text, ...responseOrder.filter(t => t !== text)]);

  try {
    setRunningQueries(set => new Set(set).add(text));

    const url = new URL(import.meta.env.VITE_BACKEND_URL + `/normalizer/query`);
    url.searchParams.set('s', text);
    const response = await fetchData<QueryServerResponse>({url, credentials});

    setResponses(responses => new Map(responses).set(text, response));
    setErrors(errors => {
      if (!errors.has(text))
        return errors;

      const newErrors = new Map(errors);
      newErrors.delete(text);
      return newErrors;
    });
  } catch (e) {
    const message = e instanceof Error ? e.message : String(e);
    setErrors(errors => new Map(errors).set(text, message));
  } finally {
    setRunningQueries(prevState => {
      const newState = new Set(prevState);
      newState.delete(text);
      return newState;
    });
  }
}

function WelcomeSection() {
  return <Section direction='column'>
    <Typography variant="h6" component="div" sx={{flexGrow: 1}}>
      Welcome to EntitySearch!
    </Typography>
    <p>this is a tool for entity linking.</p>
    <p>use the search bar to get started!</p>
  </Section>;
}

function MainPage({credentials, openEntity}: {
  credentials: Credentials,
  openEntity: (id: Guid) => void
}) {
  const [responses, setResponses] = useState(() => new Map<string, QueryServerResponse>());
  const [responseOrder, setResponseOrder] = useState<string[]>(() => []);
  const [errors, setErrors] = useState(() => new Map<string, string>());
  const [runningQueries, setRunningQueries] = useState<Set<string>>(() => new Set<string>());

  const clear = () => {
    setResponses(new Map());
    setResponseOrder([]);
    setErrors(new Map());
    setRunningQueries(new Set());
  };

  return <Page>
    <SearchBox
      onSearch={async text => await onSearch(text, credentials, setResponseOrder, setRunningQueries, setResponses, setErrors)}/>

    <Section direction='row'>
      <Button onClick={() => {
        for (let i = 0; i < 20; i++) {
          onSearch(`test${i}`, credentials, setResponseOrder, setRunningQueries, setResponses, setErrors);
        }
      }}>torture test</Button>

      <Button onClick={clear}>clear</Button>

      <Button onClick={() => openEntity('0359abf0-1b17-4128-9083-07787abb4cf8')}> entity</Button>
    </Section>

    {responseOrder.length == 0 && <WelcomeSection/>}

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

export default MainPage;
