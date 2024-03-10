import {createTheme, Theme} from "@mui/material";

export const darkTheme: Theme = createTheme({
  palette: {
    mode: 'dark',
    primary: {main: '#0294EE'},
    secondary: {main: '#D69F32'}
  },
});

export const lightTheme: Theme = createTheme({
  palette: {
    mode: 'light',
    primary: {main: '#0294EE'},
    secondary: {main: '#D69F32'}
  },
});
