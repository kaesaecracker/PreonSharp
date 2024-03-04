import {
  AppBar,
  ToggleButton,
  ToggleButtonGroup,
  Toolbar,
  Typography,
  ThemeProvider,
  CssBaseline, IconButton
} from "@mui/material";
import React, {ReactNode, useState} from "react";

import HomePage from "./HomePage";
import QueryPage from "./QueryPage";
import LoginDialog from "./LoginDialog";
import useStoredState from "./useStoredState";
import themes, {dark} from "./themes";
import AccountIcon from '@mui/icons-material/AccountCircleOutlined';

export default function App(props: any) {
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

  return <>
    <ThemeProvider theme={themes.get(colorScheme) || dark}>
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

          <IconButton aria-label='login' onClick={() => setLoginOpen(true)}>
            <AccountIcon/>
          </IconButton>
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
