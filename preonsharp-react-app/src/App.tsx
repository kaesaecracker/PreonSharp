import {AppBar, CssBaseline, ThemeProvider, Toolbar, useMediaQuery} from "@mui/material";
import {ReactNode, useState} from "react";

import {darkTheme, lightTheme} from "./themes";
import {useStoredObjectState} from "./useStoredState";

import MainPage from "./MainPage.tsx";
import MainAppBar from "./MainAppBar";
import SettingsPage from "./SettingsPage";
import {ColorScheme, Settings} from "./models/Settings.ts";

function getDefaultSettings(): Settings {
  return {
    colorScheme: "dark",
    credentials: {
      userName: '',
      password: ''
    }
  };
}

export default function App() {
  const [settings, mutateSettings] = useStoredObjectState<Settings>('settings', getDefaultSettings);
  const [subPage, setSubPage] = useState<string | null>(null);

  const theme = settings.colorScheme === 'light' ? lightTheme : darkTheme;

  let subContent: ReactNode | null;
  switch (subPage) {
    case 'settings':
      subContent = <SettingsPage
        settings={settings} mutateSettings={mutateSettings}
        onClose={() => setSubPage(null)}/>;
      break;
    default:
      // default empty app bar for animation
      subContent = <AppBar sx={{flexGrow: 1}} position="static" elevation={0}>
        <Toolbar/>
      </AppBar>;
  }

  const mainContent = <>
    <MainAppBar
      setColorScheme={(colorScheme: ColorScheme) => mutateSettings(oldState => {
        return {...oldState, colorScheme};
      })}
      colorScheme={settings.colorScheme}
      onSettingsClick={() => setSubPage('settings')}/>

    <MainPage userName={settings.credentials?.userName} password={settings.credentials?.password}/>
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
          transition: 'width 0.3s ease-in',
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
