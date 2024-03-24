import {CssBaseline, ThemeProvider, useMediaQuery} from '@mui/material';
import {ReactNode, useState} from 'react';
import {darkTheme, lightTheme} from './themes';
import {useStoredObjectState} from './useStoredState';
import MainPage from './MainPage.tsx';
import MainAppBar from './MainAppBar';
import SettingsPage from './SettingsPage';
import {ColorScheme, getDefaultSettings, Settings} from './models/Settings.ts';
import EntityPage from './EntityPage.tsx';
import {EmptyGuid, Guid} from './models/Guid.ts';
import CustomAppBar from './components/CustomAppBar.tsx';
import './App.css';

type PageName = 'settings' | 'entity' | null;

export default function App() {
  const [settings, mutateSettings] = useStoredObjectState<Settings>('settings', getDefaultSettings);
  const [subPage, setSubPage] = useState<PageName>(null);
  const [entityId, setEntityId] = useState<Guid>(EmptyGuid);

  const theme = settings.colorScheme === 'light' ? lightTheme : darkTheme;

  const closePage = () => setSubPage(null);
  const openEntity = (id: Guid) => {
    setSubPage('entity');
    setEntityId(id);
  };

  let rightColumn: ReactNode | null;
  switch (subPage) {
    case 'settings':
      rightColumn = <SettingsPage
        settings={settings} mutateSettings={mutateSettings}
        onClose={closePage}/>;
      break;
    case 'entity':
      rightColumn = <EntityPage
        entityId={entityId}
        onClose={closePage}
        openEntity={openEntity}
        credentials={settings.credentials}/>;
      break;
    default:
      // default empty app bar for animation
      rightColumn = <CustomAppBar/>;
  }

  const leftColumn = <>
    <MainAppBar
      setColorScheme={(colorScheme: ColorScheme) => mutateSettings(oldState => {
        return {...oldState, colorScheme};
      })}
      colorScheme={settings.colorScheme}
      onSettingsClick={() => setSubPage('settings')}/>

    <MainPage credentials={settings.credentials} openEntity={openEntity}/>
  </>;

  let content: ReactNode;
  if (useMediaQuery('(min-width: 1000px)')) {
    // multi column layout
    content = <div className='App-MultiColumn'>
      <div className='App-MultiColumn-Left'>
        {leftColumn}
      </div>

      <div
        className='App-MultiColumn-Right'
        style={{flexGrow: subPage !== null ? 1 : 0}}
      >
        {rightColumn}
      </div>
    </div>;
  } else {
    // single column layout
    if (subPage === null)
      content = leftColumn;
    else
      content = rightColumn;
  }

  return <ThemeProvider theme={theme}>
    <CssBaseline/>
    {content}
  </ThemeProvider>;
}
