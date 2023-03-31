import { Helmet } from 'react-helmet-async';
import { Button, Typography, Container } from '@mui/material';
import { Link } from 'react-router-dom';

export default function Page404() {
    return (
        <>
            <Helmet>
                <title> 404 Page Not Found </title>
            </Helmet>

            <Container>
                <Typography variant="h3" paragraph>
                    Sorry, page not found!
                </Typography>

                <Typography sx={{ color: 'text.secondary' }}>
                    The page you’re looking for could not be found.
                    Perhaps you’ve mistyped the URL? Be sure to check your spelling.
                </Typography>

                <Button to="/" size="large" variant="contained" component={Link}>
                    Go to Home
                </Button>
            </Container>
        </>
    );
}