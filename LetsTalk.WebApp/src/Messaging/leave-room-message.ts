import { LTMessage } from "./lt-message";

export interface LeaveRoomMessage extends LTMessage {
    roomId: string;
    roomName: string;
    userId: string;
    userName: string;
}
