import {IconButton, LinearProgress, Link} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import {ReactNode, useEffect, useState} from 'react';
import Page from './components/Page.tsx';
import {Entity, EntityRelation, EntityTag} from './models/Entity.ts';
import {Guid} from './models/Guid.ts';
import {Credentials} from './models/Settings.ts';
import {fetchData} from './fetchData.ts';
import CustomAppBar from './components/CustomAppBar.tsx';
import {DataGrid} from '@mui/x-data-grid';

function TagsView({tags}: { tags: EntityTag[] }): ReactNode {
  if (tags.length === 0)
    return null;

  return <DataGrid
    className='DataGrid-Footerless'
    disableRowSelectionOnClick
    rows={tags.map((value, index) => ({...value, id: index}))}
    columns={[
      {field: 'kind', headerName: 'Kind', flex: 1},
      {field: 'value', headerName: 'Value', flex: 1}
    ]}
  />;
}

function RelationsView({relations, openEntity}: {
  relations: EntityRelation[];
  openEntity: (id: Guid) => void;
}) {
  if (relations.length === 0)
    return null;

  const renderLinkCell = ({row}: { row: EntityRelation }): ReactNode => {
    return <Link onClick={() => openEntity(row.other)}>
      {row.other}
    </Link>;
  };

  return <DataGrid
    className='DataGrid-Footerless'
    disableRowSelectionOnClick
    rows={relations.map((value, index) => ({...value, id: index}))}
    columns={[
      {field: 'kind', headerName: 'Kind', flex: 1},
      {field: 'other', headerName: 'Other', flex: 1, renderCell: renderLinkCell}
    ]}
  />;
}

const queryUrl = import.meta.env.VITE_BACKEND_URL + '/taxonomy/';

export default function EntityPage({onClose, entityId, credentials, openEntity}: {
  onClose: () => void;
  entityId: Guid;
  credentials: Credentials;
  openEntity: (id: Guid) => void;
}) {
  const [data, setData] = useState<Entity | null>(null);

  useEffect(() => {
    try {
      const url = new URL(queryUrl + entityId);
      fetchData<Entity>({credentials, url})
        .then(newData => {
          if (newData.id !== data?.id)
            setData(newData);
        });
    } catch (e) {
      console.log(e);
    }
  }, [entityId, data, credentials]);

  return <>
    <CustomAppBar title={`Entity ${entityId}`}>
      <IconButton onClick={onClose} color='inherit'>
        <CloseIcon/>
      </IconButton>
    </CustomAppBar>

    {
      data === null
        ? <LinearProgress/>
        : <Page className='EntityPage'>
          <>
            <TagsView tags={data.names}/>
            <TagsView tags={data.tags}/>
            <RelationsView relations={data.relations} openEntity={openEntity}/>
          </>
        </Page>
    }
  </>;
}
