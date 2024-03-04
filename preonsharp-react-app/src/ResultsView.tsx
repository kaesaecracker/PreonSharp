import { DataGrid } from '@mui/x-data-grid';
import { QueryWithServerResponse } from './types';
import { Accordion, AccordionDetails, AccordionSummary } from '@mui/material';
import React from 'react';

function ResultsView(props: QueryWithServerResponse) {
  const [expanded, setExpanded] = React.useState(true);

  return <Accordion
    expanded={expanded}
    onChange={(event, newExpanded) => setExpanded(newExpanded)} >
    <AccordionSummary >
      <p>result for query: {props.query} </p>
    </AccordionSummary>
    <AccordionDetails>
      <p>full seach time: {props.response.executionTime}</p>
      <DataGrid
        rows={props.response.foundIds}
        columns={[
          { field: 'name', headerName: 'Name', flex: 1, maxWidth: 200 },
          {
            field: 'ids', headerName: 'IDs', flex: 1, minWidth: 200,
            renderCell: (params) => (<div>
              {params.value.map((id: string) => (<>{id}<br /></>))}
            </div>)
          }
        ]}
        getRowId={row => row.name}
        initialState={{
          pagination: {
            paginationModel: { page: 0, pageSize: 10 },
          },
        }}
        checkboxSelection
      />
    </AccordionDetails>
  </Accordion>;
}


export default ResultsView;