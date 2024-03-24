import {Accordion, AccordionDetails, AccordionSummary, Alert, AlertTitle, LinearProgress, Link} from '@mui/material';
import {useState} from 'react';
import {SearchResult, TextMatch} from './models/SearchResult';
import {Guid} from './models/Guid';

function ErrorView({error}: { error: string }) {
  return <Alert severity={'error'}>
    <AlertTitle>Error occurred</AlertTitle>
    {error}
  </Alert>;
}

function MatchView({match, openEntity}: {
  match: TextMatch;
  openEntity: (id: Guid) => void;
}) {
  return <>
    <p>text: {match.text}</p>
    <div style={{flexDirection: 'row', flexWrap: 'wrap', gap: '16px', display: 'flex'}}>
      {
        match.entityIds.map(id => (
          <Link key={id} onClick={() => openEntity(id)}>
            {id}
          </Link>
        ))
      }
    </div>
  </>;
}

function SearchResultView({response, openEntity}: {
  response: SearchResult;
  openEntity: (id: Guid) => void;
}) {
  return <div className='SearchResultView'>
    <p>Searched as: {response.transformedQuery}</p>
    <p>Result kind: {response.kind}</p>
    <p>Query time: {response.queryTime}</p>

    {
      response.matches.map((value) => (
        <MatchView key={value.text} match={value} openEntity={openEntity}/>
      ))
    }
  </div>;
}

export default function QueryResultView({error, query, response, running, openEntity}: {
  query: string;
  response?: SearchResult;
  error?: string;
  running: boolean;
  openEntity: (id: Guid) => void;
}) {
  const [expanded, setExpanded] = useState(true);

  return <Accordion
    variant="outlined"
    expanded={expanded}
    onChange={(_event, newExpanded) => setExpanded(newExpanded)}>
    <AccordionSummary>
      Query: "{query}"
    </AccordionSummary>
    <AccordionDetails>
      {running && <LinearProgress/>}
      {error && <ErrorView error={error}/>}
      {response && <SearchResultView response={response} openEntity={openEntity}/>}
    </AccordionDetails>
  </Accordion>;
}
