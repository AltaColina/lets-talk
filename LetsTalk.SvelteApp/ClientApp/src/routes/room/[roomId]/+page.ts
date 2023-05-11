import { error, type LoadEvent } from "@sveltejs/kit";

interface RoomPageRouteParams extends Partial<Record<string, string>> {
    roomId: string;
}

export interface RoomPageData {
    roomId: string;
}

export function load({ params }: LoadEvent<RoomPageRouteParams>): RoomPageData {
    if (typeof params.roomId === 'string') {
        return {
            roomId: params.roomId
        }
    }

    throw error(404, 'Not found');
}