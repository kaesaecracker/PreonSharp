import {CssBaseline, ThemeProvider} from "@mui/material";
import React, {ReactNode, useState} from "react";

import {darkTheme, lightTheme} from "./themes";
import useStoredState from "./useStoredState";

import HomePage from "./HomePage";
import QueryPage from "./QueryPage";
import LoginDialog from "./LoginDialog";
import MainAppBar from "./MainAppBar";

export default function App() {
  const [loginOpen, setLoginOpen] = useState(false);
  const [userName, setUserName] = useStoredState('userName', '');
  const [password, setUserPassword] = useStoredState('userPassword', '');
  const [currentPage, setCurrentPage] = useState('home');
  const [colorScheme, setColorScheme] = useStoredState('colorScheme', 'dark');

  const pages = new Map<string, ReactNode>();
  pages.set('home', (<HomePage onStartClick={() => setCurrentPage('query')}/>));
  pages.set('query', (<QueryPage userName={userName} password={password}/>));

  return <ThemeProvider theme={colorScheme === 'light' ? lightTheme : darkTheme}>
    <CssBaseline/>

    <MainAppBar openLogin={() => setLoginOpen(true)} setColorScheme={setColorScheme} colorScheme={colorScheme}/>

    <LoginDialog
      open={loginOpen} setOpen={setLoginOpen}
      password={password} setPassword={setUserPassword}
      userName={userName} setUserName={setUserName}/>

    {pages.get(currentPage) || pages.get('home')}
  </ThemeProvider>;
}
