import { LTMessage } from "./lt-message";

export interface DisconnectMessage extends LTMessage {
    userId: string;
    userName: string;
    userImageUrl?: string;
 }
