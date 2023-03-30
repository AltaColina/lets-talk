import { Box, Drawer, ListItemText, ListItem, ListItemButton, List, Divider } from '@mui/material';
import useMediaQuery from '@mui/material/useMediaQuery';
import { useTheme } from '@mui/material/styles';
import { useMenuContext } from "../Context/Menu";
import Logo from "./Logo";
import { useRoomsContext } from '../Context/Rooms';

const NAV_WIDTH = 280;

export default function Nav() {
    const roomsContext = useRoomsContext();
    const renderRooms = (
        <Box
            sx={{
                height: 1,
                '& .simplebar-content': { height: 1, display: 'flex', flexDirection: 'column' },
            }}
        >
            <Box sx={{ px: 2.5, py: 3, display: 'inline-flex' }}>
                <Logo />
            </Box>
            <Divider />
            <List>
                {roomsContext.rooms.map(room => (
                    <ListItem key={room.id}
                    // disablePadding
                    >
                        <ListItemButton
                            onClick={() => roomsContext.setActiveRoom(room.id)}
                            selected={room.id === roomsContext.activeRoom}
                            sx={{
                                borderRadius: 3,
                                border: 1,
                                borderColor: 'primary.main',
                                //"&.Mui-selected": {
                                //    backgroundColor: "pink",
                                //    "&:hover": {
                                //        backgroundColor: "green"
                                //    },
                                //},
                                //":hover": {
                                //    backgroundColor: "red"
                                //}
                            }}
                        >
                            <ListItemText
                                primary={room.name}
                            // secondary={room.id}
                            />
                        </ListItemButton>
                    </ListItem>
                ))}
            </List>
            <Box sx={{ flexGrow: 1 }} />
        </Box>
    )
    const theme = useTheme();
    const isDesktop = useMediaQuery(theme.breakpoints.up('lg'));
    const menuContext = useMenuContext();
    return (
        <Box
            component="nav"
            sx={{
                flexShrink: { lg: 0 },
                width: { lg: NAV_WIDTH },
            }}
        >
            {isDesktop ? (
                <Drawer
                    open
                    variant="permanent"
                    PaperProps={{
                        sx: {
                            width: NAV_WIDTH,
                            bgcolor: 'background.default',
                            borderRightStyle: 'dashed',
                        },
                    }}
                >
                    {renderRooms}
                </Drawer>
            ) : (
                <Drawer
                    open={menuContext.openMenu}
                    onClose={menuContext.toggleMenu}
                    ModalProps={{
                        keepMounted: true,
                    }}
                    PaperProps={{
                        sx: { width: NAV_WIDTH },
                    }}
                >
                    {renderRooms}
                </Drawer>
            )}
        </Box>
    );
}