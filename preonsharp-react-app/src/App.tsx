import {CssBaseline, ThemeProvider} from "@mui/material";
import {useState} from "react";

import {darkTheme, lightTheme} from "./themes";
import useStoredState from "./useStoredState";

import MainPage from "./MainPage.tsx";
import LoginDialog from "./LoginDialog";
import MainAppBar from "./MainAppBar";

export default function App() {
  const [loginOpen, setLoginOpen] = useState(false);
  const [userName, setUserName] = useStoredState('userName', '');
  const [password, setUserPassword] = useStoredState('userPassword', '');
  const [colorScheme, setColorScheme] = useStoredState('colorScheme', 'dark');

  return <ThemeProvider theme={colorScheme === 'light' ? lightTheme : darkTheme}>
    <CssBaseline/>

    <MainAppBar openLogin={() => setLoginOpen(true)} setColorScheme={setColorScheme} colorScheme={colorScheme}/>

    <LoginDialog
      open={loginOpen} setOpen={setLoginOpen}
      password={password} setPassword={setUserPassword}
      userName={userName} setUserName={setUserName}/>

    <MainPage userName={userName} password={password}/>
  </ThemeProvider>;
}
