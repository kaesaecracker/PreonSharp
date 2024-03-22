import {AppBar, Divider, IconButton, List, ListItem, ListItemText, Toolbar, Typography} from "@mui/material";

import CloseIcon from '@mui/icons-material/Close';
import Page from "./components/Page.tsx";

export default function SettingsPage(props: {
  onClose: () => void;
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
      <List>
        {['user login', 'color theme'].map((text) => (
          <ListItem key={text}>
            <ListItemText primary={text}/>
          </ListItem>
        ))}
      </List>
      <Divider/>
      <List>
        {['torture test', 'clear'].map((text) => (
          <ListItemText primary={text} key={text}/>
        ))}
      </List>
    </Page>
  </>;
}
