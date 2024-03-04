import {createTheme, Theme} from "@mui/material";

export const dark: Theme = createTheme({
  palette: {
    mode: 'dark',
    primary: {main: '#0294EE'},
    secondary: {main: '#D69F32'}
  },
});

export const light: Theme = createTheme({
  palette: {
    mode: 'light',
    primary: {main: '#0294EE'},
    secondary: {main: '#D69F32'}
  },
})

const themes = new Map<string, Theme>([
  ['light', light],
  ['dark', dark]
]);

export default themes;
