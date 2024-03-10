import { CssBaseline, ThemeProvider } from "@mui/material";
import { useState } from "react";

import { darkTheme, lightTheme } from "./themes";
import useStoredState from "./useStoredState";

import MainPage from "./MainPage.tsx";
import LoginDialog from "./LoginDialog";
import MainAppBar from "./MainAppBar";
import SettingsPage from "./SettingsPage";

/*
const drawerWidth = 240;

const AppBar = styled(MuiAppBar, {
  shouldForwardProp: (prop) => prop !== 'open',
})<AppBarProps>(({ theme, open }) => ({
  transition: theme.transitions.create(['margin', 'width'], {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  ...(open && {
    width: `calc(100% - ${drawerWidth}px)`,
    transition: theme.transitions.create(['margin', 'width'], {
      easing: theme.transitions.easing.easeOut,
      duration: theme.transitions.duration.enteringScreen,
    }),
    marginRight: drawerWidth,
  }),
}));
*/

export default function App() {
  const [loginOpen, setLoginOpen] = useState(false);
  const [userName, setUserName] = useStoredState('userName', '');
  const [password, setUserPassword] = useStoredState('userPassword', '');
  const [colorScheme, setColorScheme] = useStoredState('colorScheme', 'dark');
  const [settingsOpen, setSettingsOpen] = useState(false);

  return <ThemeProvider theme={colorScheme === 'light' ? lightTheme : darkTheme}>
    <CssBaseline />
    <div style={{
      display: "flex",
      flexDirection: 'row'
    }}>
      <div style={{ flexGrow: 2 }}>
        <MainAppBar
          openLogin={() => setLoginOpen(true)}
          setColorScheme={setColorScheme}
          colorScheme={colorScheme}
          onSettingsClick={() => setSettingsOpen(!settingsOpen)} />

        <LoginDialog
          open={loginOpen} setOpen={setLoginOpen}
          password={password} setPassword={setUserPassword}
          userName={userName} setUserName={setUserName} />

        <MainPage userName={userName} password={password} />
      </div>

      {settingsOpen && <SettingsPage onClose={() => setSettingsOpen(false)} />}

    </div>
  </ThemeProvider>;
}
