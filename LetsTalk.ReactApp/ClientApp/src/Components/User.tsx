import { Box, Divider, Typography, Stack, MenuItem, Avatar, IconButton, Popover, Link } from '@mui/material';
import { useState } from 'react';
import { useProfileContext } from '../Context/User';
import { Home, Person, Settings, Logout } from '@mui/icons-material';

const capitalize = (str: string) => {
    return str.charAt(0).toUpperCase() + str.slice(1);
};

export default function UserPopover() {
    const profileContext = useProfileContext();
    const profile = profileContext.profile;
    const [open, setOpen] = useState(null);

    const handleOpen = (event: any) => {
        setOpen(event.currentTarget);
    };

    const handleClose = () => {
        setOpen(null);
    };

    const MENU_OPTIONS = [
        {
            label: 'Home',
            path: '/',
            icon: <Home />,
        },
        {
            label: 'Profile',
            path: '/profile',
            icon: <Person />,
        },
        {
            label: 'Settings',
            path: '/settings',
            icon: <Settings />,
        },
        {
            label: 'Logout',
            path: profile.logoutUrl,
            icon: <Logout />,
        }
    ];

    //const handleCloseLink = (nav?: string) => {
    //    setOpen(null);
    //    if (nav)
    //        window.location.href = nav;
    //};

    return (
        <>
            <IconButton
                onClick={handleOpen}
                
            >
                <Avatar src={ profile.picture } alt={ profile.displayName } />
            </IconButton>

            <Popover
                open={Boolean(open)}
                anchorEl={open}
                onClose={handleClose}
                //onClose={() => { handleCloseLink("/#about") }}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
                transformOrigin={{ vertical: 'top', horizontal: 'right' }}
                PaperProps={{
                    sx: {
                        p: 0,
                        mt: 1.5,
                        ml: 0.75,
                        width: 180,
                        '& .MuiMenuItem-root': {
                            typography: 'body2',
                            borderRadius: 0.75,
                        },
                    },
                }}
            >
                <Box sx={{ my: 1.5, px: 2.5 }}>
                    <Typography variant="subtitle2" noWrap>
                        {
                            profileContext.profile.displayName
                                ? capitalize(profileContext.profile.displayName)
                                : 'Guest'
                        }
                    </Typography>
                    <Typography variant="body2" sx={{ color: 'text.secondary' }} noWrap>
                        user.email
                    </Typography>
                </Box>

                <Divider sx={{ borderStyle: 'dashed' }} />

                <Stack
                    //sx={{ p: 1 }}
                >
                    {MENU_OPTIONS.map((option) => (
                        <MenuItem
                            component={Link}
                            href={option.path}
                            sx={{ m: 1 }}
                            onClick={handleClose}
                        >
                            { option.icon }
                            { option.label }
                        </MenuItem>
                    ))}
                </Stack>

                {/*<Divider sx={{ borderStyle: 'dashed' }} />*/}

                {/*<MenuItem*/}
                {/*    component={Link}*/}
                {/*    href={profile.logoutUrl}*/}
                {/*    sx={{ m: 1 }}*/}
                {/*    onClick={handleClose}*/}
                {/*>*/}
                {/*    <Logout />*/}
                {/*    Logout*/}
                {/*</MenuItem>*/}
            </Popover>
        </>
    );
}