﻿import { styled } from '@mui/material/styles';
import { Outlet } from 'react-router-dom';
import { MenuContextProvider } from "../Context/Menu";
import { RoomsContextProvider } from "../Context/Rooms";
import { ProfileContextProvider } from '../Context/User';
import Top from '../Components/Top';

const APP_BAR_MOBILE = 64;
const APP_BAR_DESKTOP = 92;
const StyledRoot = styled('div')({
    display: 'flex',
    minHeight: '100%',
    overflow: 'hidden',
    //backgroundColor: 'red'
});
const Main = styled('div')(({ theme }) => ({
    flexGrow: 1,
    overflow: 'auto',
    minHeight: '100%',
    paddingTop: APP_BAR_MOBILE + 24,
    paddingBottom: theme.spacing(10),
    [theme.breakpoints.up('lg')]: {
        paddingTop: APP_BAR_DESKTOP + 24,
        paddingLeft: theme.spacing(2),
        paddingRight: theme.spacing(2),
    },
}));

export default function Layout() {
    return (
        <StyledRoot>
            <ProfileContextProvider>
                <MenuContextProvider>
                    <RoomsContextProvider>
                        <Top />
                        <Main>
                            <Outlet />
                        </Main>
                    </RoomsContextProvider>
                </MenuContextProvider>
            </ProfileContextProvider>
        </StyledRoot>
    );
}