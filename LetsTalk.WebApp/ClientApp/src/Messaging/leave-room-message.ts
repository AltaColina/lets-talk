import { LTMessage } from "./lt-message";

export interface LeaveRoomMessage extends LTMessage<'LeaveRoom'> {
    roomId: string;
    roomName: string;
    userId: string;
    userName: string;
}
