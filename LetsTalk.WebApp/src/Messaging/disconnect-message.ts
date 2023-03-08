import { User } from "../Users/user";
import { LTMessage } from "./lt-message";

export interface DisconnectMessage extends LTMessage<User> { }
