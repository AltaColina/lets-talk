export interface LTMessageMap {
    'Content': ContentMessage;
    'Connect': ConnectMessage;
    'Disconnect': DisconnectMessage;
    'JoinRoom': JoinRoomMessage;
    'LeaveRoom': LeaveRoomMessage;
}

export interface LTMessage<T extends keyof LTMessageMap> {
    id: string;
    type: T;
    timestamp: string;
}

export interface ConnectMessage extends LTMessage<'Connect'> {
    userId: string;
    userName: string;
    userImage?: string;
}

export interface ContentMessage extends LTMessage<'Content'> {
    userId: string;
    userName: string;
    roomId: string;
    contentType: string;
    content: string;
}

export interface DisconnectMessage extends LTMessage<'Disconnect'> {
    userId: string;
    userName: string;
    userImage?: string;
}

export interface JoinRoomMessage extends LTMessage<'JoinRoom'> {
    roomId: string;
    roomName: string;
    userId: string;
    userName: string;
}

export interface LeaveRoomMessage extends LTMessage<'LeaveRoom'> {
    roomId: string;
    roomName: string;
    userId: string;
    userName: string;
}