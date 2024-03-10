import {Paper} from "@mui/material";
import {ReactNode} from "react";

import './Section.css';

function Section(props: { children: ReactNode, direction: ('row' | 'column') }) {
  return <Paper className='Section' variant="outlined" sx={{flexDirection: props.direction}}>
    {props.children}
  </Paper>;
}

export default Section;
