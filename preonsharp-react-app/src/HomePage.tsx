
import { Typography, Button } from '@mui/material';
import Page from './Page';
import Section from './Section';

function HomePage(props: { onStartClick: () => void }) {
    return <Page>
        <Section>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>welcome to preon#</Typography>
            <p>this is a tool for entity linking</p>
            <Button variant='contained' disableElevation onClick={props.onStartClick}>
                Start
            </Button>
        </Section>
    </Page>;
}

export default HomePage;