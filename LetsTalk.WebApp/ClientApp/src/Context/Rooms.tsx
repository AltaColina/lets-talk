import { createContext, Dispatch, ReactNode, SetStateAction, useContext, useEffect, useState } from "react";
import { Room } from "../Rooms/room";
import { hubClient } from "../Services/hub-client";

export type RoomsContextType = {
    rooms: Room[];
    activeRoom: string;
    setActiveRoom: Dispatch<SetStateAction<string>>;
};

export const RoomsContext = createContext<RoomsContextType | null>(null);

type Props = {
    children: ReactNode;
};

export const RoomsContextProvider = ({ children }: Props) => {
    const [rooms, setRooms] = useState([] as Room[]);
    const [activeRoom, setActiveRoom] = useState('');

    const loadRooms = async () => {
        const roomsData = await hubClient.getUserRooms();
        setRooms(roomsData.rooms);
        setActiveRoom(roomsData.rooms[0].id);
    }

    useEffect(() => {
        loadRooms();
    }, [])

    return (
        <RoomsContext.Provider value={{ rooms, activeRoom, setActiveRoom }}>
            { children }
        </RoomsContext.Provider>
    );
};

export const useRoomsContext = () => {
    const roomsContext = useContext(RoomsContext);
    if (!roomsContext) throw new Error('Context most be within provider!');
    return roomsContext;
}