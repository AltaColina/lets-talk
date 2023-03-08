import { LTMessage } from "../Messaging/lt-message";
import { User } from "../Users/user";

export interface ContentMessage extends LTMessage<Uint8Array> {
  sender: User;
  chatId: string;
  contentType: string;
}
