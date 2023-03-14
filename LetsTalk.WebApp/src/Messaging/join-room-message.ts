import { LTMessage } from "./lt-message";

export interface JoinRoomMessage extends LTMessage {
  roomId: string;
  roomName: string;
  userId: string;
  userName: string;
}

