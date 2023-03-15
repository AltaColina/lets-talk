import { Avatar, Badge, Grid, List, ListItem, ListItemAvatar, ListItemButton, ListItemText } from "@mui/material";
import { useSnackbar } from 'notistack';
import { useEffect, useState } from "react";
import { ConnectMessage } from "../Messaging/connect-message";
import { ContentMessage } from "../Messaging/content-message";
import { DisconnectMessage } from "../Messaging/disconnect-message";
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
        const handleConnectMessage = (e: CustomEvent<ConnectMessage>) => {
            setUsers(hubClient.loggedUsers);
            enqueueSnackbar(`${e.detail.userName} has connected to the server.`);
        };
        const handleDisconnectMessage = (e: CustomEvent<DisconnectMessage>) => {
            setUsers(hubClient.loggedUsers);
            enqueueSnackbar(`${e.detail.userName} has disconnected from the server.`);
        };
        const handleContentMessage = (e: CustomEvent<ContentMessage>) => {
            if (e.detail.roomId !== roomId) {
                const rid = e.detail.roomId;
                setBadges({ ...badges, [rid]: (badges[rid] || 0) + 1 });
            }
        };
        messenger.on('Connect', handleConnectMessage);
        messenger.on('Disconnect', handleDisconnectMessage);
        messenger.on('Content', handleContentMessage);
        return () => {
            messenger.off('Connect', handleConnectMessage);
            messenger.off('Disconnect', handleDisconnectMessage);
            messenger.off('Content', handleContentMessage);
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