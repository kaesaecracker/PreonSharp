import { AppBar, IconButton, Toolbar, Typography } from "@mui/material";
import AccountIcon from "@mui/icons-material/AccountCircleOutlined";

import DarkModeIcon from "@mui/icons-material/DarkMode";
import LightModeIcon from "@mui/icons-material/LightMode";
import SettingsIcon from "@mui/icons-material/Settings";

function DarkModeSwitcher(props: {
  colorScheme: string,
  setColorScheme: (scheme: string) => void
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
  openLogin: () => void;
  colorScheme: string;
  onSettingsClick: () => void;
  setColorScheme: (scheme: string) => void;
}) {
  return <AppBar position="static" elevation={0}>
    <Toolbar>
      <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
        preon#
      </Typography>

      <DarkModeSwitcher colorScheme={props.colorScheme} setColorScheme={props.setColorScheme} />

      <IconButton aria-label='login' onClick={props.openLogin} color="inherit">
        <AccountIcon />
      </IconButton>

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
