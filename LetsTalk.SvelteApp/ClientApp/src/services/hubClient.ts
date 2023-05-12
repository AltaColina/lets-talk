import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import type { ConnectMessage, ContentMessage, DisconnectMessage, JoinRoomMessage, LeaveRoomMessage } from "../models/messaging/messages.js";
import type { GetLoggedUsersResponse, GetUserRoomsResponse } from "../models/users/responses.js";
import type { User } from "../models/users/user.js";
import type { AsyncDisposable } from "./disposable.js";
import { messenger } from "./messenger.js";
import { onlineUsersById } from "./store.js";

const EMPTY_CONTENT_MESSAGES_ARRAY = new Array<ContentMessage>();

class HubClient {
    readonly #url: string = '/hubs/letstalk';
    #messagesByRoom = new Map<string, ContentMessage[]>();
    #messageHandlers: Map<string, (...args: any[]) => any>;

    #connection: HubConnection | undefined;
    #isConnecting = false;
    #disposable?: AsyncDisposable;

    public get isConnecting(): boolean { return this.#isConnecting; }
    public get isConnected(): boolean { return !!this.#connection && this.#connection.state === HubConnectionState.Connected; }

    constructor() {
        this.#messageHandlers = new Map<string, (...args: any[]) => any>([
            ['ConnectMessage', this.#onConnect.bind(this)],
            ['DisconnectMessage', this.#onDisconnect.bind(this)],
            ['JoinRoomMessage', this.#onJoinRoom.bind(this)],
            ['LeaveRoomMessage', this.#onLeaveRoom.bind(this)],
            ['ContentMessage', this.#onContent.bind(this)]
        ]);
    }

    async connect(): Promise<AsyncDisposable> {
        if (this.#isConnecting) {
            return this.#disposable!;
        }

        this.#isConnecting = true;

        try {
            if (this.#connection) {
                await this.disconnect();
            }

            this.#connection = new HubConnectionBuilder()
                .withUrl(this.#url, {
                    skipNegotiation: true,
                    transport: HttpTransportType.WebSockets,
                    headers: {
                        'X-CSRF': '1'
                    }
                })
                .configureLogging(LogLevel.Debug)
                .withAutomaticReconnect()
                .build();

            await this.#connection.start();

            for (const [messageType, handler] of this.#messageHandlers) {
                this.#connection.on(messageType, handler);
            }

            const response = await this.#connection.invoke<GetLoggedUsersResponse>('GetUsersLoggedInAsync');
            onlineUsersById.set(new Map([...response.users.map<[string, User]>(u => [u.id, u])]));
        }
        finally {
            this.#isConnecting = false;
        }
        return (this.#disposable = () => this.disconnect());
    }

    async disconnect(): Promise<void> {
        if (this.isConnected) {
            for (const [messageType, handler] of this.#messageHandlers) {
                this.#connection!.off(messageType, handler);
            }
            this.#connection = undefined;
            await this.#connection!.stop();
        }

    }

    async getRoomMessages(roomId: string): Promise<ContentMessage[]> {
        if (!this.isConnected)
            throw new Error('Not connected');
        return this.#messagesByRoom.get(roomId) || EMPTY_CONTENT_MESSAGES_ARRAY;
    }

    async sendContentMessage(roomId: string, contentType: string, contentBase64: string): Promise<void> {
        if (!this.isConnected)
            throw new Error('Not connected');
        return await this.#connection!.invoke('SendContentMessageAsync', roomId, contentType, contentBase64);
    }

    async getRoomsWithUser(): Promise<GetUserRoomsResponse> {
        if (!this.isConnected)
            throw new Error('Not connected');
        return await this.#connection!.invoke('GetRoomsWithUserAsync');
    }

    #onConnect(message: ConnectMessage) {
        onlineUsersById.update(map => {
            map.set(message.userId, {
                id: message.userId,
                name: message.userName,
                image: message.userImage
            });
            return map;
        });
        messenger.send('Connect', message);
    }

    #onDisconnect(message: DisconnectMessage) {
        onlineUsersById.update(map => {
            map.delete(message.userId);
            return map;
        });
        messenger.send('Disconnect', message);
    }

    #onJoinRoom(message: JoinRoomMessage) {
        messenger.send('JoinRoom', message.roomId, message);
    }

    #onLeaveRoom(message: LeaveRoomMessage) {
        messenger.send('LeaveRoom', message.roomId, message);
    }

    #onContent(message: ContentMessage) {
        console.warn(message);
        let messages = this.#messagesByRoom.get(message.roomId);
        if (!messages) {
            this.#messagesByRoom.set(message.roomId, messages = []);
        }
        messages.push(message);
        messenger.send('Content', message.roomId, message);
    }
}

export const hubClient = new HubClient();