import {AppBar, IconButton, LinearProgress, Toolbar, Typography} from "@mui/material";

import CloseIcon from '@mui/icons-material/Close';
import Page from "./components/Page.tsx";

import {useEffect, useState} from 'react';
import {Entity} from "./models/Entity.ts";
import {Guid} from "./models/Guid.ts";
import {Credentials} from "./models/Settings.ts";
import {fetchData} from "./fetchData.ts";

function EntityView({entity}: { entity: Entity }) {
  return <div>
    {JSON.stringify(entity)}
  </div>;
}

const queryUrl = import.meta.env.VITE_BACKEND_URL + `/taxonomy/`;

export default function EntityPage({onClose, entityId, credentials}: {
  onClose: () => void;
  entityId: Guid;
  credentials: Credentials
}) {
  const [data, setData] = useState<Entity | null>(null);

  useEffect(() => {
    if (data !== null)
      return;
    try {
      const url = new URL(queryUrl + entityId);
      fetchData<Entity>({credentials, url})
        .then(setData);
    } catch (e) {
      console.log(e);
    }
  }, [data, credentials, entityId]);

  return <>
    <AppBar sx={{flexGrow: 1}} position="static" elevation={0}>
      <Toolbar>
        <Typography variant="h6" component="div" sx={{flexGrow: 1}}>
          Entity {entityId}
        </Typography>
        <IconButton onClick={onClose} color="inherit">
          <CloseIcon/>
        </IconButton>
      </Toolbar>
    </AppBar>

    <Page>
      {
        data === null
          ? <LinearProgress/>
          : <EntityView entity={data}/>
      }
    </Page>
  </>;
}
