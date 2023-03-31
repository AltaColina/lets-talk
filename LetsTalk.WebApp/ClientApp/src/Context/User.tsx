import { createContext, Dispatch, ReactNode, SetStateAction, useContext, useEffect, useState } from "react";
import { Room } from "../Rooms/room";
import { UserProfile } from "../Security/user-profile";
import { httpClient } from "../Services/http-client";

export type ProfileContextType = {
    profile: UserProfile
};

export const ProfileContext = createContext<ProfileContextType | null>(null);

type Props = {
    children: ReactNode;
};

export const ProfileContextProvider = ({ children }: Props) => {
    const [profile, setProfile] = useState(UserProfile.guestProfile);

    const loadProfile = async () => {
        const profile = await UserProfile.currentProfile;
        setProfile(profile);
    }

    useEffect(() => {
        loadProfile();
    }, [])

    return (
        <ProfileContext.Provider value={{ profile }}>
            { children }
        </ProfileContext.Provider>
    );
};

export const useProfileContext = () => {
    const profileContext = useContext(ProfileContext);
    if (!profileContext) throw new Error('Context must be within provider!');
    return profileContext;
}