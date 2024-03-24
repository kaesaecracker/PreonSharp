import {Paper} from '@mui/material';
import {ReactNode} from 'react';

import './Section.css';

function Section({children, direction}: {
  children: ReactNode;
  direction: 'row' | 'column';
}) {
  return <Paper className='Section' variant="outlined" sx={{flexDirection: direction}}>
    {children}
  </Paper>;
}

export default Section;
