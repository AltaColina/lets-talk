import { Room } from "../Rooms/room";
import { User } from "../Users/user";
import { LTMessage } from "./lt-message";

export interface JoinRoomMessage extends LTMessage<User> {
  room: Room;
}

