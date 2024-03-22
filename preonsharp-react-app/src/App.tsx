import {AppBar, CssBaseline, ThemeProvider, Toolbar, useMediaQuery} from "@mui/material";
import {ReactNode, useState} from "react";

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
  const [subPage, setSubPage] = useState<string | null>(null);

  const theme = colorScheme === 'light' ? lightTheme : darkTheme;

  let subContent: ReactNode | null;
  switch (subPage) {
    case 'settings':
      subContent = <SettingsPage onClose={() => setSubPage(null)}/>;
      break;
    default:
      // default empty app bar for animation
      subContent = <AppBar sx={{flexGrow: 1}} position="static" elevation={0}>
        <Toolbar/>
      </AppBar>;
  }

  const mainContent = <>
    <MainAppBar
      openLogin={() => setLoginOpen(true)}
      setColorScheme={setColorScheme}
      colorScheme={colorScheme}
      onSettingsClick={() => setSubPage('settings')}/>

    <LoginDialog
      open={loginOpen} setOpen={setLoginOpen}
      password={password} setPassword={setUserPassword}
      userName={userName} setUserName={setUserName}/>

    <MainPage userName={userName} password={password}/>
  </>;

  return <ThemeProvider theme={theme}>
    <CssBaseline/>

    {useMediaQuery('(min-width: 1000px)')
      ? <div style={{
        display: "flex",
        flexDirection: 'row',
      }}>
        <div style={{
          flexBasis: 0,
          flexGrow: 2,
        }}>
          {mainContent}
        </div>

        <div style={{
          flexBasis: 0,
          transition: 'width 0.5s ease-in',
          transitionProperty: 'width, flex-grow',
          minWidth: 0,
          flexGrow: subPage !== null ? 1 : 0
        }}>
          {subContent}
        </div>
      </div>
      : subContent ?? mainContent}

  </ThemeProvider>;
}
