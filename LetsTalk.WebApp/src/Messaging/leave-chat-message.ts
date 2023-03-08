import { Chat } from "../Chats/chat";
import { User } from "../Users/user";
import { LTMessage } from "./lt-message";

export interface LeaveChatMessage extends LTMessage<User> {
    chat: Chat;
}
