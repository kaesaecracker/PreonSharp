import {ReactNode} from 'react';

import './Page.css';

export default function Page({children, className}: {
  children: ReactNode,
  className?: string
}) {
  return <main className={'Page ' + (className !== undefined ? className : '')}>
    {children}
  </main>;
}
