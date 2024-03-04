import { ReactNode } from 'react';

import './Page.css'

function Page(props: { children: ReactNode }) {
  return <main className='Page'>
    {props.children}
  </main>
}

export default Page;