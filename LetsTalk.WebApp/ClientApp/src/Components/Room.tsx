import Send from '@mui/icons-material/Send';
import { Box, Button, Grid, List, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { ContentMessage } from "../Messaging/content-message";
import { hubClient } from "../Services/hub-client";
import { messenger } from "../Services/messenger";
import { Message } from './Message';

const encode = function() {
    const encoder = new TextEncoder();
    return (content: string | undefined) => encoder.encode(content);
}();

export const Room = ({ roomId }: { roomId: string }) => {
    const [messages, setMessages] = useState(new Map<string, ContentMessage>());
    useEffect(() => {
        const roomMessages = hubClient.getRoomMessages(roomId!);
        setMessages(new Map(roomMessages.map(m => [m.id, m])));
        return messenger.on('Content', e => {
            if(e.detail.roomId === roomId && !messages.get(e.detail.id)) {
                messages.set(e.detail.id, e.detail);
                setMessages(new Map(messages));
            }
        }).dispose;
    }, [messages]);
    
    const [content, setContent] = useState('');
    const onTextFieldChanged = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        setContent(e.target.value);
    };
    const sendMessage = async () => {
        await hubClient.sendContentMessage(roomId, 'text/plain', Buffer.from(encode(content)).toString('base64'));
        setContent('');
    };
    const onKeyDown = async (e: React.KeyboardEvent<HTMLInputElement>) => {
        if(e.key === 'Enter') {
            e.preventDefault();
            await sendMessage();
        }
    }

    return (
        <Box
            component="form"
            sx={{ '& .MuiTextField-root': { m: 1, width: '25ch' } }}
            noValidate
            autoComplete="off">
                <Grid container alignItems={ 'center' }>
                    <Grid item xs={10}>
                        <TextField
                            style={{width: '100%'}}
                            value={content}
                            onChange={onTextFieldChanged}
                            onKeyDown={onKeyDown}/>
                    </Grid>
                    <Grid item xs={2}>
                        <Button
                            onClick={sendMessage}
                            variant="contained"
                            disabled={!content}
                            endIcon={<Send />}>
                            Send
                        </Button>
                    </Grid>
                </Grid>
                <List>
                {
                    Array.from(messages.values()).map(m => (<Message key={m.id} message={m} />))
                }
                </List>
            
        </Box>
    );
}