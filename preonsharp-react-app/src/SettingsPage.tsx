import { AppBar, Divider, IconButton, List, ListItem, ListItemText, Toolbar, Typography } from "@mui/material";
import { useState } from "react";

import CloseIcon from '@mui/icons-material/Close';

export default function SettingsPage(props: {
  onClose: () => void;
}) {
  return <div style={{ flexGrow: 1 }}>
    <AppBar position="static" elevation={0}>
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          Settings
        </Typography>
        <IconButton onClick={props.onClose} color="inherit">
          <CloseIcon />
        </IconButton>
      </Toolbar>
    </AppBar>

    <Divider />
    <List>
      {['user login', 'color theme'].map((text) => (
        <ListItem>
          <ListItemText primary={text} />
        </ListItem>
      ))}
    </List>
    <Divider />
    <List>
      {['torture test', 'clear'].map((text) => (
        <ListItemText primary={text} />
      ))}
    </List>
  </div>
}