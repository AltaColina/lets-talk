import { User } from "../Users/user";
import { LTMessage } from "./lt-message";

export interface ConnectMessage extends LTMessage<User> { }
