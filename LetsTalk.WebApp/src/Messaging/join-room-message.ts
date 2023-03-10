import { UserTag } from "../Users/user-tag";
import { LTMessage } from "./lt-message";

export interface JoinRoomMessage extends LTMessage {
  roomId: string;
  user: UserTag;
}

