import { AppBar, IconButton, Toolbar, Typography } from "@mui/material";

import DarkModeIcon from "@mui/icons-material/DarkMode";
import LightModeIcon from "@mui/icons-material/LightMode";
import SettingsIcon from "@mui/icons-material/Settings";
import {ColorScheme} from "./models/Settings.ts";

function DarkModeSwitcher(props: {
  colorScheme: ColorScheme,
  setColorScheme: (scheme: ColorScheme) => void
}) {
  return props.colorScheme === 'light'
    ? <IconButton aria-label='dark mode' onClick={() => props.setColorScheme('dark')} color="inherit">
      <DarkModeIcon />
    </IconButton>
    : <IconButton aria-label='light mode' onClick={() => props.setColorScheme('light')} color="inherit">
      <LightModeIcon />
    </IconButton>;
}

export default function MainAppBar(props: {
  colorScheme: ColorScheme;
  onSettingsClick: () => void;
  setColorScheme: (scheme: ColorScheme) => void;
}) {
  return <AppBar position="static" elevation={0}>
    <Toolbar>
      <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
        preon#
      </Typography>

      <DarkModeSwitcher colorScheme={props.colorScheme} setColorScheme={props.setColorScheme} />

      <IconButton
        color="inherit"
        aria-label="open drawer"
        edge="end"
        onClick={props.onSettingsClick}
      >
        <SettingsIcon />
      </IconButton>

    </Toolbar>
  </AppBar>;
}
