import { Avatar, Badge, Grid, List, ListItem, ListItemAvatar, ListItemButton, ListItemText } from "@mui/material";
import { useSnackbar } from 'notistack';
import { useEffect, useState } from "react";
import { GetUserRoomsResponse } from "../Rooms/get-user-rooms-response";
import { hubClient } from "../Services/hub-client";
import { messenger } from "../Services/messenger";
import { Room } from "./Room";

export const Lobby = ({ getUserRooms }: { getUserRooms: () => GetUserRoomsResponse }) => {
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
    return (
        <Grid container spacing={2}>
            <Grid item xs={2}>
                <List>{
                    rooms.map(room => (
                        <ListItem key={room.id}>
                            <Badge badgeContent={badges[room.id]} color="info">
                                <ListItemButton
                                    onClick={() => onRoomClick(room.id)}
                                    selected={room.id === roomId}>
                                    <ListItemText primary={room.name} secondary={room.id} />
                                </ListItemButton>
                            </Badge>
                        </ListItem>
                    ))
                }</List>
            </Grid>
            <Grid item xs={8} style={{ height: "100%" }}>
                <Room roomId={roomId} />
            </Grid>
            <Grid item xs={2} textAlign={'left'}>
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
            </Grid>
        </Grid>
    );
}