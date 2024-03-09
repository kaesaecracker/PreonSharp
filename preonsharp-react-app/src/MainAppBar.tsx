import React from "react";
import {AppBar, IconButton, Toolbar, Typography} from "@mui/material";
import AccountIcon from "@mui/icons-material/AccountCircleOutlined";

import DarkModeIcon from "@mui/icons-material/DarkModeOutlined";
import LightModeIcon from "@mui/icons-material/LightModeOutlined";

function DarkModeSwitcher(props: {
  colorScheme: string,
  setColorScheme: (scheme: string) => void
}) {
  return props.colorScheme === 'light'
    ? <IconButton aria-label='dark mode' onClick={() => props.setColorScheme('dark')}>
      <DarkModeIcon/>
    </IconButton>
    : <IconButton aria-label='light mode' onClick={() => props.setColorScheme('light')}>
      <LightModeIcon/>
    </IconButton>;
}

export default function MainAppBar(props: {
  openLogin: () => void;
  colorScheme: string;
  setColorScheme: (scheme: string) => void;

}) {
  return <AppBar position="static" elevation={0}>
    <Toolbar>
      <Typography variant="h6" component="div" sx={{flexGrow: 1}}>
        preon#
      </Typography>

      <DarkModeSwitcher colorScheme={props.colorScheme} setColorScheme={props.setColorScheme}/>

      <IconButton aria-label='login' onClick={props.openLogin}>
        <AccountIcon/>
      </IconButton>
    </Toolbar>
  </AppBar>;
}
