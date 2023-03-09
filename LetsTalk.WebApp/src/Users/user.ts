export interface User
{
    id: string;
    name: string;
    roles: string[];
    rooms: string[];
    creationTime: string;
    lastLoginTime: string;
}