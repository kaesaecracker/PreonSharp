import { AppBar, Divider, IconButton, List, ListItem, ListItemButton, ListItemIcon, ListItemText, TextField, Toolbar, Typography } from "@mui/material";

import AccountIcon from "@mui/icons-material/AccountCircleOutlined";

import CloseIcon from '@mui/icons-material/Close';

export default function SettingsPage(props: {
  onClose: () => void;
  open: boolean,
  setOpen: (open: boolean) => void,
  userName: string,
  setUserName: (userName: string) => void,
  password: string,
  setPassword: (password: string) => void
  openLogin: () => void;
}) {
  const closeDialog = () => props.setOpen(false);

  return <>
    <AppBar position="static" elevation={0}>
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          Settings
        </Typography>
        <IconButton onClick={props.onClose} color="inherit">
          <CloseIcon />
        </IconButton>
      </Toolbar>
    </AppBar>

    <Divider />
    <List sx={{
      textAlign: 'left',
    }}>
      <ListItem
        key='user login'
        sx={{
          gap: 2,
          display: 'flex',
          flexDirection: 'column',
          paddingTop: 1,
        }}>
        <div style={{
          
        }}>
          <ListItemIcon aria-label='login' onClick={props.openLogin} color="inherit">
            <AccountIcon />
          </ListItemIcon>
          <ListItemText >Login</ListItemText>
        </div>


        <TextField required fullWidth
          variant="outlined"
          label="user"
          type='text'
          value={props.userName}
          onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
            props.setUserName(event.target.value);
          }}
        />
        <TextField required fullWidth
          variant="outlined"
          label="password"
          type='password'
          value={props.password}
          onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
            props.setPassword(event.target.value);
          }}
        />
        <ListItemButton
          onClick={closeDialog}
        >
          Done</ListItemButton>
      </ListItem>

      <Divider />

      <ListItem key='color theme'>
        <ListItemText primary='color theme' />
      </ListItem>
    </List >
    <Divider />
    <List>
      {['torture test', 'clear'].map((text) => (
        <ListItemText primary={text} key={text} />
      ))}
    </List>
  </>
}
