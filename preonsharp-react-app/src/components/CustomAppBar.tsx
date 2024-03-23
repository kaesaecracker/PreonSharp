import {AppBar, Toolbar, Typography} from "@mui/material";
import {ReactNode} from "react";

export default function CustomAppBar({children, title}: {
  children?: ReactNode;
  title?: string;
}) {
  return <AppBar sx={{flexGrow: 1}} position="static" elevation={0}>
    <Toolbar>
      {
        title && <Typography variant="h6" component="div" sx={{flexGrow: 1}}>
          {title}
        </Typography>
      }
      {children}
    </Toolbar>
  </AppBar>;
}
