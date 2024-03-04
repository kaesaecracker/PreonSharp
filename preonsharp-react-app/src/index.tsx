import './index.css';

import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';

import ReactDOM from 'react-dom/client';
import { CssVarsProvider } from '@mui/material-next';
import { useState, ReactNode, StrictMode } from 'react';
import { AppBar, Button, Toolbar, Typography } from '@mui/material';

import HomePage from './HomePage';
import QueryPage from './QueryPage';
import LoginDialog from './LoginDialog'

// primary: '#0294EE',
// secondary: '#D69F32',

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

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
  pages.set('home', (<HomePage onStartClick={() => setCurrentPage('query')} />));
  pages.set('query', (<QueryPage userName={userName} password={password} />));

  return <>
    <AppBar position="static">
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          preon#
        </Typography>
        <Button onClick={() => setLoginOpen(true)} color="inherit">Login</Button>
      </Toolbar>
    </AppBar>

    <LoginDialog
      open={loginOpen} setOpen={setLoginOpen}
      password={password} setPassword={setUserPassword}
      userName={userName} setUserName={setUserName} />

    {pages.get(currentPage) || pages.get('home')}
  </>
}

root.render(
  <StrictMode>
    <CssVarsProvider>
      <AppFrame />
    </CssVarsProvider>
  </StrictMode>
);

