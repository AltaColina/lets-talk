import { Box, Stack, AppBar, Toolbar, IconButton, Link, Avatar } from '@mui/material';
import { styled } from '@mui/material/styles';
import Logo from './Logo';
import { useEffect, useState } from 'react';
import { UserProfile } from '../Security/user-profile';
import { hubClient } from '../Services/hub-client';
import { Navigate } from 'react-router-dom';

const NAV_WIDTH = 0;
const HEADER_MOBILE = 64;
const HEADER_DESKTOP = 92;
const StyledRoot = styled(AppBar)(({ theme }) => ({
    backgroundColor: theme.palette.background.default,
    color: theme.palette.text.primary,
    boxShadow: 'none',
    [theme.breakpoints.up('lg')]: {
        width: `calc(100% - ${NAV_WIDTH + 0}px)`,
    },
}));
const StyledToolbar = styled(Toolbar)(({ theme }) => ({
    minHeight: HEADER_MOBILE,
    [theme.breakpoints.up('lg')]: {
        minHeight: HEADER_DESKTOP,
        padding: theme.spacing(0, 5),
    },
}));

export default function Top() {
    const [userProfile, setUserProfile] = useState(UserProfile.guestProfile);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    useEffect(() => {
        (async () => {
            const profile = await UserProfile.currentProfile;
            if (profile.isLoggedIn) {
                if (!hubClient.isConnected && !hubClient.isConnecting) {
                    await hubClient.connect();
                }
                setIsLoggedIn(true);
            }
            setUserProfile(profile);
        })();
    }, []);
    return (
        <StyledRoot>
            <StyledToolbar>
                {isLoggedIn && (<Navigate to="/" replace={true} />)}
                <Logo />

                <Box sx={{ flexGrow: 1 }} />

                <Stack
                    direction="row"
                    alignItems="center"
                    spacing={{
                        xs: 0.5,
                        sm: 1,
                    }}
                >
                    <Link
                        href="/bff/login"
                    >
                        Login
                    </Link>
                    <Avatar />
                </Stack>
            </StyledToolbar>
        </StyledRoot>
    );
}