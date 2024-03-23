import {CssBaseline, ThemeProvider, useMediaQuery} from "@mui/material";
import {ReactNode, useState} from "react";
import {darkTheme, lightTheme} from "./themes";
import {useStoredObjectState} from "./useStoredState";
import MainPage from "./MainPage.tsx";
import MainAppBar from "./MainAppBar";
import SettingsPage from "./SettingsPage";
import {ColorScheme, getDefaultSettings, Settings} from "./models/Settings.ts";
import EntityPage from "./EntityPage.tsx";
import {EmptyGuid, Guid} from "./models/Guid.ts";
import CustomAppBar from "./components/CustomAppBar.tsx";

type PageName = 'settings' | 'entity' | null;

export default function App() {
  const [settings, mutateSettings] = useStoredObjectState<Settings>('settings', getDefaultSettings);
  const [subPage, setSubPage] = useState<PageName>(null);
  const [entityId, setEntityId] = useState<Guid>(EmptyGuid);

  const theme = settings.colorScheme === 'light' ? lightTheme : darkTheme;

  const closePage = () => setSubPage(null);
  const openEntity=(id: Guid) => {
    setSubPage('entity');
    setEntityId(id);
  };

  let subContent: ReactNode | null;
  switch (subPage) {
    case 'settings':
      subContent = <SettingsPage
        settings={settings} mutateSettings={mutateSettings}
        onClose={closePage}/>;
      break;
    case 'entity':
      subContent = <EntityPage
        entityId={entityId}
        onClose={closePage}
        credentials={settings.credentials}/>;
      break;
    default:
      // default empty app bar for animation
      subContent = <CustomAppBar/>;
  }

  const mainContent = <>
    <MainAppBar
      setColorScheme={(colorScheme: ColorScheme) => mutateSettings(oldState => {
        return {...oldState, colorScheme};
      })}
      colorScheme={settings.colorScheme}
      onSettingsClick={() => setSubPage('settings')}/>

    <MainPage credentials={settings.credentials} openEntity={openEntity}/>
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
          transitionDuration: '0.25s',
          transitionProperty: 'width, flex-grow',
          transitionTimingFunction: 'ease-in-out',
          minWidth: 0,
          flexGrow: subPage !== null ? 1 : 0
        }}>
          {subContent}
        </div>
      </div>
      : subPage !== null
        ? subContent
        : mainContent}

  </ThemeProvider>;
}
