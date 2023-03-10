import { List } from '@mui/icons-material';
import Send from '@mui/icons-material/Send';
import { Button, Grid, ListItem, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { ContentMessage } from "../Messaging/content-message";
import { hubClient } from "../Services/hub-client";
import { messenger } from "../Services/messenger";

const decodeTextPlain = (content: string): string => {
    return Buffer.from(content, 'base64').toString();
}

const encode = function() {
    const encoder = new TextEncoder();
    return (content: string | undefined) => encoder.encode(content);
}();

export const Room = ({ roomId }: { roomId: string }) => {
    const [msgs, setMsgs] = useState(new Array<ContentMessage>());
    const handleContentMessage = (m: CustomEvent<ContentMessage>) => {
        if(m.detail.roomId === roomId) {
            setMsgs([...msgs, m.detail]);
        }
    };
    useEffect(() => {
        messenger.on('content', handleContentMessage);
        const messages = hubClient.getRoomMessages(roomId!);
        setMsgs(messages);
        return () => messenger.off('content', handleContentMessage);
    });
    
    const [canSend, setCanSend] = useState(false);
    let content: string | undefined = undefined;
    const setContent = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        content = e.target.value
        setCanSend(!!content);
    };
    const sendMessage = async () => {
        await hubClient.sendContentMessage(roomId, 'text/plain', Buffer.from(encode(content)).toString('base64'));
    };

    return (
        <Grid container direction="column" spacing={2}>
            <Grid item container xs={10}>
                <List>
                {
                    msgs.map(msg => (
                        <ListItem key={msg.id}>
                            {decodeTextPlain(msg.content)}
                        </ListItem>
                    ))
                }
                </List>
            </Grid>
            <Grid item xs={2}>
                <Grid item container spacing={2}>
                    <Grid item xs={10}>
                        <TextField onChange={setContent} />
                    </Grid>
                    <Grid item xs={2}>
                        <Button
                            onClick={sendMessage}
                            variant="contained"
                            disabled={!canSend}
                            endIcon={<Send />}>
                            Send
                        </Button>
                    </Grid>
                </Grid>
            </Grid>
        </Grid>
    );
}