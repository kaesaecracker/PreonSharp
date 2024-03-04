import { DataGrid } from '@mui/x-data-grid';
import Section from './Section';
import { QueryWithServerResponse } from './types';

function ResultsView(props: QueryWithServerResponse) {
  return <Section>
    <p>result for query text: {props.query} </p>
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
  </Section>;
}


export default ResultsView;