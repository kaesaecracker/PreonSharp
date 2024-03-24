import {IconButton} from '@mui/material';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import SettingsIcon from '@mui/icons-material/Settings';
import {ColorScheme} from './models/Settings.ts';
import CustomAppBar from './components/CustomAppBar.tsx';

function DarkModeSwitcher(props: {
  colorScheme: ColorScheme,
  setColorScheme: (scheme: ColorScheme) => void
}) {
  return props.colorScheme === 'light'
    ? <IconButton aria-label='dark mode' onClick={() => props.setColorScheme('dark')} color="inherit">
      <DarkModeIcon/>
    </IconButton>
    : <IconButton aria-label='light mode' onClick={() => props.setColorScheme('light')} color="inherit">
      <LightModeIcon/>
    </IconButton>;
}

export default function MainAppBar({colorScheme, onSettingsClick, setColorScheme}: {
  colorScheme: ColorScheme;
  onSettingsClick: () => void;
  setColorScheme: (scheme: ColorScheme) => void;
}) {
  return <CustomAppBar title='EntitySearch'>
    <DarkModeSwitcher colorScheme={colorScheme} setColorScheme={setColorScheme}/>
    <IconButton
      color="inherit"
      aria-label="open drawer"
      edge="end"
      onClick={onSettingsClick}
    >
      <SettingsIcon/>
    </IconButton>
  </CustomAppBar>;
}
