import {CssBaseline, ThemeProvider, useMediaQuery} from "@mui/material";
import {useState} from "react";

import {darkTheme, lightTheme} from "./themes";
import useStoredState from "./useStoredState";

import MainPage from "./MainPage.tsx";
import LoginDialog from "./LoginDialog";
import MainAppBar from "./MainAppBar";
import SettingsPage from "./SettingsPage";

export default function App() {
  const [loginOpen, setLoginOpen] = useState(false);
  const [userName, setUserName] = useStoredState('userName', '');
  const [password, setUserPassword] = useStoredState('userPassword', '');
  const [colorScheme, setColorScheme] = useStoredState('colorScheme', 'dark');
  const [settingsOpen, setSettingsOpen] = useState(false);
  const isMultiColumn = useMediaQuery('(min-width: 600px)');

  const theme = colorScheme === 'light' ? lightTheme : darkTheme;
  const settingsWidth = !settingsOpen ? 0 : isMultiColumn ? '33%' : '100%';

  return <ThemeProvider theme={theme}>
    <CssBaseline/>

    <div style={{
      display: "flex",
      flexDirection: 'row',
      flexWrap: "wrap",
      overflowX: "hidden",
    }}>

      {(isMultiColumn || !settingsOpen) &&
        <div style={{
          flexBasis: 0,
          flexGrow: 1,
          flexShrink: 0,
          transition: 'width 0.5s ease-in',
          width: '100%',
        }}>
          <MainAppBar
            openLogin={() => setLoginOpen(true)}
            setColorScheme={setColorScheme}
            colorScheme={colorScheme}
            onSettingsClick={() => setSettingsOpen(!settingsOpen)}/>

          <LoginDialog
            open={loginOpen} setOpen={setLoginOpen}
            password={password} setPassword={setUserPassword}
            userName={userName} setUserName={setUserName}/>

          <MainPage userName={userName} password={password}/>
        </div>
      }

      <div style={{
        flexBasis: 0,
        flexShrink: 1,
        transition: 'width 0.5s ease-in',
        width: settingsWidth,
        flexGrow: settingsOpen ? 1 : 0
      }}>
        <SettingsPage onClose={() => setSettingsOpen(false)}/>
      </div>

    </div>

  </ThemeProvider>;
}
