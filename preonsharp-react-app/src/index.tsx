import './index.css';

import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';

import ReactDOM from 'react-dom/client';
import {useState, ReactNode, StrictMode} from 'react';
import {AppBar, Button, Toolbar, Typography, ToggleButtonGroup, ToggleButton} from '@mui/material';
import React from 'react';
import {ThemeProvider, createTheme} from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';

import HomePage from './HomePage';
import QueryPage from './QueryPage';
import LoginDialog from './LoginDialog'

const darkTheme = createTheme({
  palette: {
    mode: 'dark',
    primary: {main: '#0294EE'},
    secondary: {main: '#D69F32'}
  },
});

const lightTheme = createTheme({
  palette: {
    mode: 'light',
    primary: {main: '#0294EE'},
    secondary: {main: '#D69F32'}
  },
});

function useStoredState(storageKey: string, initialState: string): [string, ((newState: string) => void)] {
  const [state, setState] = useState<string>(localStorage.getItem(storageKey) || initialState);

  const setSavedState = (newState: string) => {
    localStorage.setItem(storageKey, newState);
    setState(newState);
  };

  return [state, setSavedState];
}

function AppFrame(props: any) {
  const [loginOpen, setLoginOpen] = useState(false);
  const [userName, setUserName] = useStoredState('userName', '');
  const [password, setUserPassword] = useStoredState('userPassword', '');
  const [currentPage, setCurrentPage] = useState('home');

  const pages = new Map<string, ReactNode>();
  pages.set('home', (<HomePage onStartClick={() => setCurrentPage('query')}/>));
  pages.set('query', (<QueryPage userName={userName} password={password}/>));

  const [colorScheme, setColorScheme] = useStoredState('colorScheme', 'dark');

  const handleChange = (
    event: React.MouseEvent<HTMLElement>,
    newColorScheme: string,
  ) => {
    setColorScheme(newColorScheme);
  };

  const theme = colorScheme === 'dark' ? darkTheme : lightTheme;

  return <>
    <ThemeProvider theme={theme}>
      <CssBaseline/>
      <AppBar position="static" elevation={0}>
        <Toolbar>
          <Typography variant="h6" component="div" sx={{flexGrow: 1}}>
            preon#
          </Typography>
          <ToggleButtonGroup
            color="secondary"
            value={colorScheme}
            exclusive
            onChange={handleChange}
            aria-label="Platform"
          >
            <ToggleButton value="light">light</ToggleButton>
            <ToggleButton value="dark">dark</ToggleButton>
          </ToggleButtonGroup>
          <Button onClick={() => setLoginOpen(true)} color="inherit">Login</Button>
        </Toolbar>
      </AppBar>

      <LoginDialog
        open={loginOpen} setOpen={setLoginOpen}
        password={password} setPassword={setUserPassword}
        userName={userName} setUserName={setUserName}/>

      {pages.get(currentPage) || pages.get('home')}
    </ThemeProvider>
  </>
}

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  <StrictMode>
    <AppFrame/>
  </StrictMode>
);

