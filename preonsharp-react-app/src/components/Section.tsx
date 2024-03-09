import {Paper} from "@mui/material";
import {ReactNode} from "react";

import './Section.css';

function Section(props: { children: ReactNode }) {
  return <Paper className='Section' variant="outlined">
    {props.children}
  </Paper>;
}

export default Section;
