import { Box, Button, Divider, Drawer, List, ListItem, ListItemButton, ListItemText, Toolbar, Typography } from "@mui/material";
import { useSnackbar } from 'notistack';
import { useEffect, useState } from "react";
import { useMenuContext } from "../Context/Menu";
import { GetUserRoomsResponse } from "../Rooms/get-user-rooms-response";
import { hubClient } from "../Services/hub-client";
import { messenger } from "../Services/messenger";
import { Room } from "./Room";

export const Lobby = ({ getUserRooms }: { getUserRooms: () => GetUserRoomsResponse }) => {
    // const [ openMenu, setOpen ] = useState(true);
    const { rooms } = getUserRooms();
    const { enqueueSnackbar } = useSnackbar();
    const [roomId, setRoomId] = useState(rooms[0]?.id);
    const [badges, setBadges] = useState({} as Record<string, number | undefined>);
    const [users, setUsers] = useState(hubClient.loggedUsers);
    const onRoomClick = (roomId: string) => {
        setBadges({ ...badges, [roomId]: undefined });
        setRoomId(roomId);
    }
    useEffect(() => {
        const subscriptions = [
            messenger.on('Connect', e => {
                setUsers(hubClient.loggedUsers);
                enqueueSnackbar(`${e.detail.userName} has connected to the server.`);
            }),
            messenger.on('Disconnect', e => {
                setUsers(hubClient.loggedUsers);
                enqueueSnackbar(`${e.detail.userName} has disconnected from the server.`);
            }),
            messenger.on('Content', e => {
                if (e.detail.roomId !== roomId) {
                    const rid = e.detail.roomId;
                    setBadges({ ...badges, [rid]: (badges[rid] || 0) + 1 });
                }
            })
        ];
        return () => {
            subscriptions.forEach(s => s.dispose());
            subscriptions.length = 0;
        };
    });
    const drawerWidth = 240;
    const drawer = (
        <Box>
            <Toolbar>
                <Typography variant="h6" component="div" sx={{ flexGrow: 1, textAlign: "center" }}>
                    Rooms
                </Typography>
            </Toolbar>
            <Divider />
            <List>
                {rooms.map(room => (
                    <ListItem key={room.id} disablePadding>
                            <ListItemButton
                                onClick={() => onRoomClick(room.id)}
                                selected={room.id === roomId}>
                                <ListItemText
                                    primary={room.name}
                                    // secondary={room.id}
                                    />
                            </ListItemButton>
                    </ListItem>
                ))}
            </List>
        </Box>
    )
    const menuContext = useMenuContext();
    return (
        <Box>
            <Box
                component='nav'
                sx={{ width: { md: drawerWidth }, flexShrink: { sm: 0 } }}>
                <Drawer
                    variant='permanent'
                    open
                    sx={{
                        display: {
                            xs: 'none',
                            // sm: 'none',
                            md: 'block'
                        },
                        '& .MuiDrawer-paper': {
                            top: '64px',
                            height: 'calc(100% - 112px)',
                            width: drawerWidth
                        }
                    }}>
                    {drawer}
                </Drawer>
                <Drawer
                    variant='temporary'
                    open={menuContext.openMenu}
                    sx={{ display: {
                        sm: 'block',
                        md: 'none' }
                    }}>
                    {drawer}
                    <Button
                    onClick={menuContext.toggleMenu}
                    >Close</Button>
                </Drawer>
            </Box>
            <Box
                sx={{
                    flexGrow: 1,
                    p: 3,
                    overflow: 'auto',
                    width: {
                        md: `calc(100vw - ${drawerWidth}px)`
                    }
                }}>
                <Toolbar />
                <Room
                    //roomId={roomId}
                />
            </Box>
            {/* <Box
                // sx={{
                //     flexGrow: 1,
                //     height: '100vh',
                //     overflow: 'auto',
                //     backgroundColor: 'green'
                // }}
                sx={{
                    flexGrow: 1,
                    p: 3,
                    width: {
                        xs: `calc(100% - ${drawerWidth}px)`,
                        sm: `calc(100% - ${drawerWidth}px)`
                    }
                }}
                >
                <Toolbar />
                <Room roomId={roomId} />
                </Box> */}
                    {/* <Grid item xs={2} textAlign={'left'}>
                        <List>{
                            users.map(user => (
                                <ListItem key={user.id}>
                                    <ListItemButton>
                                        <ListItemAvatar>
                                            <Avatar alt={user.name} src={user.imageUrl} />
                                        </ListItemAvatar>
                                        <ListItemText primary={user.name} secondary={user.id} />
                                    </ListItemButton>
                                </ListItem>
                            ))
                        }</List>
                    </Grid> */}
                {/* </Grid> */}
        </Box>

        // <Box sx={{ display: 'flex' }}>
        
        // </Box>
    );
}