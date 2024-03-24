import {IconButton, Paper, TextField, Typography} from "@mui/material";
import React, {ReactNode} from 'react';
import CloseIcon from '@mui/icons-material/Close';
import Page from "./components/Page.tsx";
import {Settings} from "./models/Settings.ts";
import CustomAppBar from "./components/CustomAppBar.tsx";

function SettingsSection({children, title}: {
  children: ReactNode;
  title: string;
}): ReactNode {
  return <Paper variant="outlined" sx={{
    gap: '24px',
    display: 'flex',
    flexDirection: 'column',
    padding: '10px',
  }}>
    <Typography variant="h6" component="div" sx={{flexGrow: 1}}>
      {title}
    </Typography>
    {children}
  </Paper>;
}

export default function SettingsPage(props: {
  onClose: () => void;
  settings: Settings;
  mutateSettings: (mutator: (oldState: Settings) => Settings) => void;
}) {
  const onUserNameFieldChange = (event: React.ChangeEvent<HTMLInputElement>) =>
    props.mutateSettings((s: Settings): Settings => ({
      ...s, credentials: {
        ...s.credentials,
        userName: event.target.value
      }
    }));
  const onPasswordFieldChange = (event: React.ChangeEvent<HTMLInputElement>) =>
    props.mutateSettings((s: Settings): Settings => ({
      ...s, credentials: {
        ...s.credentials,
        password: event.target.value
      }
    }));

  return <>
    <CustomAppBar title='Settings'>
      <IconButton onClick={props.onClose} color="inherit">
        <CloseIcon/>
      </IconButton>
    </CustomAppBar>

    <Page>
      <SettingsSection title='Credentials'>
        <TextField
          required fullWidth
          variant='outlined'
          label="user"
          type='text'
          value={props.settings.credentials.userName}
          onChange={onUserNameFieldChange}
        />
        <TextField
          required fullWidth
          variant="outlined"
          label="password"
          type='password'
          value={props.settings.credentials.password}
          onChange={onPasswordFieldChange}
        />
      </SettingsSection>
    </Page>
  </>;
}
