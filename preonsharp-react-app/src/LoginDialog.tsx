import { Button, Dialog, DialogTitle, TextField } from '@mui/material';

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
        <DialogTitle>Title</DialogTitle>
        <TextField required
            label="user name"
            type='text'
            value={props.userName}
            onChange={(event: any) => {
                props.setUserName(event.target.value);
            }}
        />
        <TextField required
            label="password"
            type='password'
            value={props.password}
            onChange={(event: any) => { props.setPassword(event.target.value); }}
        />
        <Button onClick={closeDialog}>Done</Button>
    </Dialog>
}

export default LoginDialog;