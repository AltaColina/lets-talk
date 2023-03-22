import { LTMessage } from "./lt-message";

export interface ConnectMessage extends LTMessage<'Connect'> {
    userId: string;
    userName: string;
    userImageUrl?: string;
}
