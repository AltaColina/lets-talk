import { LTMessage } from "../Messaging/lt-message";
import { User } from "../Users/user";

export interface ContentMessage extends LTMessage<string> {
  sender: User;
  roomId: string;
  contentType: string;
}
