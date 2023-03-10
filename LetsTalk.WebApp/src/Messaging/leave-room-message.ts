import { UserTag } from "../Users/user-tag";
import { LTMessage } from "./lt-message";

export interface LeaveRoomMessage extends LTMessage {
    roomId: string;
    user: UserTag;
}
