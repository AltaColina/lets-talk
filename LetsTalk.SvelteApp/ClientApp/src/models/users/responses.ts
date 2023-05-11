import type { Room } from "../rooms/room";
import type { User } from "./user";

export interface GetLoggedUsersResponse {
    users: User[];
}

export interface GetUserRoomsResponse {
    rooms: Room[];
}