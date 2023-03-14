import { LTMessage } from "./lt-message";

export interface ConnectMessage extends LTMessage {
    userId: string;
    userName: string;
    userImageUrl?: string;
}
