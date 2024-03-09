import {DataGrid} from '@mui/x-data-grid';
import {QueryServerResponse} from './types';
import {Accordion, AccordionDetails, AccordionSummary, Alert, AlertTitle, LinearProgress} from '@mui/material';
import React from 'react';

export default function QueryResultView(props: {
  query: string;
  response?: QueryServerResponse;
  error?: string;
  running: boolean;
}) {
  const [expanded, setExpanded] = React.useState(true);

  return <Accordion variant="outlined"
    expanded={expanded}
    onChange={(event, newExpanded) => setExpanded(newExpanded)}>
    <AccordionSummary>
      Query: "{props.query}"
    </AccordionSummary>
    <AccordionDetails>
      {props.running && <LinearProgress/>}

      {
        props.error && <Alert severity={"error"}>
          <AlertTitle>Error occurred</AlertTitle>
          {props.error}
        </Alert>
      }

      {
        props.response && <>
          <p>full search time: {props.response.executionTime}</p>
          <DataGrid
            rows={props.response.foundIds}
            columns={[
              {field: 'name', headerName: 'Name', flex: 1, maxWidth: 200},
              {
                field: 'ids', headerName: 'IDs', flex: 1, minWidth: 200,
                renderCell: (params) => (<div>
                  {params.value.map((id: string) => (<>{id}<br/></>))}
                </div>)
              }
            ]}
            getRowId={row => row.name}
            initialState={{
              pagination: {
                paginationModel: {page: 0, pageSize: 10},
              },
            }}
            checkboxSelection
          />
        </>
      }
    </AccordionDetails>
  </Accordion>;
}
