export interface User
{
    id: string;
    name: string;
    imageUrl?: string;
    roles: string[];
    rooms: string[];
    creationTime: string;
    lastLoginTime: string;
}