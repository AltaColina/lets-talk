import { Link as RouterLink } from 'react-router-dom';
import { Box, Link, Typography } from '@mui/material';

const logo = (
    <Box
        component="img"
        src="discussion.png"
        sx={{ width: 40, height: 40, cursor: 'pointer' }}
    />
);

export default function Logo() {
    return (
        <>
        <Link to="/" component={RouterLink} sx={{ display: 'contents' }}>
            {logo}
        </Link>
            <Typography>Let's Talk</Typography>
        </>
    );
}
