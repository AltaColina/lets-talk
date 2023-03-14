import { LTMessage } from "../Messaging/lt-message";

export interface ContentMessage extends LTMessage {
  userId: string;
  userName: string;
  roomId: string;
  contentType: string;
  content: string;
}
