
import { Typography } from '@mui/material';
import { Button } from '@mui/material-next';
import Page from './Page';
import Section from './Section';

function HomePage(props: { onStartClick: () => void }) {
    return <Page>
        <Section>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>welcome to preon#</Typography>
            <p>this is a tool for entity linking</p>
            <Button variant='outlined' onClick={props.onStartClick}>Start</Button>
        </Section>
    </Page>;
}

export default HomePage;