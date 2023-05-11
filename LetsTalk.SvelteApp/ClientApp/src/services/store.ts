import { writable } from "svelte/store";
import type { Room } from "../models/rooms/room";
import { UserProfile } from "../models/security/userProfile";
import type { User } from "../models/users/user";

export const greeting = writable('');

export const userProfile = writable(UserProfile.Guest);

export const rooms = writable(new Array<Room>());

export const onlineUsersById = writable(new Map<string, User>());