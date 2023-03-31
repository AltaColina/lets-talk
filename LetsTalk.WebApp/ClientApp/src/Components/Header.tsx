import { Box, Stack, AppBar, Toolbar, IconButton } from '@mui/material';
import { styled } from '@mui/material/styles';
import { useMenuContext } from "../Context/Menu";
import MenuIcon from '@mui/icons-material/Menu';
import SearchIcon from '@mui/icons-material/Search';
import UserPopover from './User';

const NAV_WIDTH = 280;
const HEADER_MOBILE = 64;
const HEADER_DESKTOP = 92;
const StyledRoot = styled(AppBar)(({ theme }) => ({
    backgroundColor: theme.palette.background.default,
    color: theme.palette.text.primary,
    boxShadow: 'none',
    [theme.breakpoints.up('lg')]: {
        width: `calc(100% - ${NAV_WIDTH + 1}px)`,
    },
}));
const StyledToolbar = styled(Toolbar)(({ theme }) => ({
    minHeight: HEADER_MOBILE,
    [theme.breakpoints.up('lg')]: {
        minHeight: HEADER_DESKTOP,
        padding: theme.spacing(0, 5),
    },
}));

export default function Header() {
    const menuContext = useMenuContext();
    return (
        <StyledRoot>
            <StyledToolbar>
                <IconButton
                    onClick={menuContext.toggleMenu}
                    sx={{
                        mr: 1,
                        color: 'text.primary',
                        display: { lg: 'none' },
                    }}
                >
                    <MenuIcon />
                </IconButton>

                <SearchIcon />
                <Box sx={{ flexGrow: 1 }} />

                <Stack
                    direction="row"
                    alignItems="center"
                    spacing={{
                        xs: 0.5,
                        sm: 1,
                    }}
                >
                    <Box>Lang</Box>
                    <Box>Not</Box>
                    <UserPopover />
                </Stack>
            </StyledToolbar>
        </StyledRoot>
    );
}