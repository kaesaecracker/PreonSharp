import { DataGrid } from '@mui/x-data-grid';

function ResultsView(props: { data: null | { executionTime: string, foundIds: { name: string, ids: string[] }[] } }) {
    if (props.data === null)
        return <p>no results</p>

    return <>
        <p>Time: {props.data.executionTime}</p>
        <DataGrid
            rows={props.data.foundIds}
            columns={[
                { field: 'name', headerName: 'name', flex: 1, maxWidth: 200 },
                {
                    field: 'ids', headerName: 'ids', flex: 1, minWidth: 200,
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
    </>;
}


export default ResultsView;