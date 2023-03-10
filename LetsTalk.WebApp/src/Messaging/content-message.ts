import { LTMessage } from "../Messaging/lt-message";
import { UserTag } from "../Users/user-tag";

export interface ContentMessage extends LTMessage {
  sender: UserTag;
  roomId: string;
  contentType: string;
  content: string;
}
