import { LTMessage } from "./lt-message";

export interface DisconnectMessage extends LTMessage<'Disconnect'> {
    userId: string;
    userName: string;
    userImageUrl?: string;
 }
