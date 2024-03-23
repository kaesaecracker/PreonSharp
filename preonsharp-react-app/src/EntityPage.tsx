import {IconButton, LinearProgress} from "@mui/material";
import CloseIcon from '@mui/icons-material/Close';
import {useEffect, useState} from 'react';
import Page from "./components/Page.tsx";
import {Entity} from "./models/Entity.ts";
import {Guid} from "./models/Guid.ts";
import {Credentials} from "./models/Settings.ts";
import {fetchData} from "./fetchData.ts";
import CustomAppBar from "./components/CustomAppBar.tsx";

function EntityView({entity}: { entity: Entity }) {
  return <div>
    {JSON.stringify(entity)}
  </div>;
}

const queryUrl = import.meta.env.VITE_BACKEND_URL + `/taxonomy/`;

export default function EntityPage({onClose, entityId, credentials}: {
  onClose: () => void;
  entityId: Guid;
  credentials: Credentials;
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
    <CustomAppBar title={`Entity ${entityId}`}>
      <IconButton onClick={onClose} color="inherit">
        <CloseIcon/>
      </IconButton>
    </CustomAppBar>

    <Page>
      {
        data === null
          ? <LinearProgress/>
          : <EntityView entity={data}/>
      }
    </Page>
  </>;
}
