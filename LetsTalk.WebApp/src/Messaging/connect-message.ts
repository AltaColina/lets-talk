import { UserTag } from "../Users/user-tag";
import { LTMessage } from "./lt-message";

export interface ConnectMessage extends LTMessage {
    user: UserTag;
}
