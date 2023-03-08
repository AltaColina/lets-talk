export interface User
{
    id: string;
    name: string;
    roles: string[];
    chats: string[];
    creationTime: string;
    lastLoginTime: string;
}