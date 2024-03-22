import {
  AppBar,
  IconButton,
  Paper, TextField,
  Toolbar,
  Typography
} from "@mui/material";

import CloseIcon from '@mui/icons-material/Close';
import Page from "./components/Page.tsx";
import * as React from "react";
import {Settings} from "./models/Settings.ts";

export default function SettingsPage(props: {
  onClose: () => void;
  settings: Settings;
  mutateSettings: (mutator: (oldState: Settings) => Settings) => void;
}) {
  return <>
    <AppBar sx={{flexGrow: 1}} position="static" elevation={0}>
      <Toolbar>
        <Typography variant="h6" component="div" sx={{flexGrow: 1}}>
          Settings
        </Typography>
        <IconButton onClick={props.onClose} color="inherit">
          <CloseIcon/>
        </IconButton>
      </Toolbar>
    </AppBar>

    <Page>
      <Paper variant="outlined" sx={{
        gap: '24px',
        display: 'flex',
        flexDirection: 'column',
        padding: '10px'
      }}>
        <h4>Login</h4>
        <TextField
          required fullWidth
          variant='outlined'
          label="user"
          type='text'
          value={props.settings.credentials.userName}
          onChange={(event: React.ChangeEvent<HTMLInputElement>) =>
            props.mutateSettings((s: Settings): Settings => ({
              ...s, credentials: {
                ...s.credentials,
                userName: event.target.value
              }
            }))}
        />
        <TextField
          required fullWidth
          variant="outlined"
          label="password"
          type='password'
          value={props.settings.credentials.password}
          onChange={(event: React.ChangeEvent<HTMLInputElement>) =>
            props.mutateSettings((s: Settings): Settings => ({
              ...s, credentials: {
                ...s.credentials,
                password: event.target.value
              }
            }))}
        />
      </Paper>
    </Page>
  </>;
}
