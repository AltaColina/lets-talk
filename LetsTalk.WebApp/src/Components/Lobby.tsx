import { Grid, List, ListItemButton, ListItemText } from "@mui/material";
import { useState } from "react";
import { GetUserRoomsResponse } from "../Rooms/get-user-rooms-response";
import { Room } from "./Room";

export const Lobby = ({ getUserRooms }: { getUserRooms: () => GetUserRoomsResponse } ) => {
    const { rooms } = getUserRooms();
    const [roomId, setRoomId] = useState(rooms[0]?.id);
    return (
        <Grid container spacing={2}>
            <Grid item xs={2}>
                <List>{
                    rooms.map(room => (
                        <ListItemButton
                            key={room.id}
                            onClick={() => setRoomId(room.id)}
                            selected={room.id === roomId}>
                            <ListItemText primary={room.name} secondary={room.id}/>
                        </ListItemButton>
                    ))
                }</List>
            </Grid>
            <Grid item xs={10} style={{height: "100%"}}>
                <Room roomId={roomId} />
            </Grid>
        </Grid>
    );
  }