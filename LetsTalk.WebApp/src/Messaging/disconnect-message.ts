import { UserTag } from "../Users/user-tag";
import { LTMessage } from "./lt-message";

export interface DisconnectMessage extends LTMessage {
    user: UserTag;
 }
