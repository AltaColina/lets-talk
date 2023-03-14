import { Grid, List, ListItemButton, ListItemText } from "@mui/material";
import { useSnackbar } from 'notistack';
import { useEffect, useState } from "react";
import { ConnectMessage } from "../Messaging/connect-message";
import { DisconnectMessage } from "../Messaging/disconnect-message";
import { GetUserRoomsResponse } from "../Rooms/get-user-rooms-response";
import { messenger } from "../Services/messenger";
import { Room } from "./Room";

export const Lobby = ({ getUserRooms }: { getUserRooms: () => GetUserRoomsResponse }) => {
    const { rooms } = getUserRooms();
    const [roomId, setRoomId] = useState(rooms[0]?.id);
    const { enqueueSnackbar } = useSnackbar();

    useEffect(() => {
        const handleConnectMessage = (e: CustomEvent<ConnectMessage>) => enqueueSnackbar(`${e.detail.userName} has connected to the server.`);
        const handleDisconnectMessage = (e: CustomEvent<DisconnectMessage>) => enqueueSnackbar(`${e.detail.userName} has disconnected from the server.`);
        messenger.on('connect', handleConnectMessage);
        messenger.on('disconnect', handleDisconnectMessage);
        return () => {
            messenger.off('connect', handleConnectMessage);
            messenger.off('disconnect', handleDisconnectMessage);
        };
    });
    return (
        <Grid container spacing={2}>
            <Grid item xs={2}>
                <List>{
                    rooms.map(room => (
                        <ListItemButton
                            key={room.id}
                            onClick={() => setRoomId(room.id)}
                            selected={room.id === roomId}>
                            <ListItemText primary={room.name} secondary={room.id} />
                        </ListItemButton>
                    ))
                }</List>
            </Grid>
            <Grid item xs={10} style={{ height: "100%" }}>
                <Room roomId={roomId} />
            </Grid>
        </Grid>
    );
}