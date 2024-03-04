import {Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField} from '@mui/material';

import './LoginDialog.css'

function LoginDialog(props: {
    open: boolean,
    setOpen: (open: boolean) => void,
    userName: string,
    setUserName: (userName: string) => void,
    password: string,
    setPassword: (password: string) => void
}) {
    const closeDialog = () => props.setOpen(false);

    return <Dialog open={props.open} onClose={closeDialog}>
        <DialogTitle>Login</DialogTitle>
        <DialogContent className='LoginDialog-Content'>
            <TextField required fullWidth
                       variant="outlined"
                       label="user"
                       type='text'
                       value={props.userName}
                       onChange={(event: any) => {
                           props.setUserName(event.target.value);
                       }}
            />
            <TextField required fullWidth
                       variant="outlined"
                       label="password"
                       type='password'
                       value={props.password}
                       onChange={(event: any) => {
                           props.setPassword(event.target.value);
                       }}
            />
        </DialogContent>
        <DialogActions>
            <Button onClick={closeDialog}>Done</Button>
        </DialogActions>
    </Dialog>
}

export default LoginDialog;
