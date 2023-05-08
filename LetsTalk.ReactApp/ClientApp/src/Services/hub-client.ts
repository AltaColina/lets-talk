import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { ConnectMessage } from "../Messaging/connect-message";
import { ContentMessage } from "../Messaging/content-message";
import { DisconnectMessage } from "../Messaging/disconnect-message";
import { JoinRoomMessage } from "../Messaging/join-room-message";
import { LeaveRoomMessage } from "../Messaging/leave-room-message";
import { GetLoggedUsersResponse } from "../Rooms/get-logged-users-response";
import { GetUserRoomsResponse } from "../Rooms/get-user-rooms-response";
import { User } from "../Users/user";
import { messenger } from "./messenger";

const EMPTY_CONTENT_MESSAGES_ARRAY = new Array<ContentMessage>();

class Listener {
  private _connection: HubConnection | null;
  private _handlers: Map<string, (...args: any[]) => any>;
  private _contentMessages: Map<string, ContentMessage[]> = new Map();
  private _loggedUsers: Map<string, User> = new Map();

  public get isListening(): boolean { return !!this._connection; }
  public get loggedUsers(): Map<string, User> { return this._loggedUsers; }

  public constructor() {
    this._connection = null;
    this._handlers = new Map();
    this._handlers.set('ConnectMessage', this.handleConnect.bind(this));
    this._handlers.set('DisconnectMessage', this.handleDisconnect.bind(this));
    this._handlers.set('JoinRoomMessage', this.handleJoinRoom.bind(this));
    this._handlers.set('LeaveRoomMessage', this.handleLeaveRoom.bind(this));
    this._handlers.set('ContentMessage', this.handleContent.bind(this));
  }

  public getRoomMessages(roomId: string): ContentMessage[] {
    return this._contentMessages.get(roomId) || EMPTY_CONTENT_MESSAGES_ARRAY;
  }

  public async attach(connection: HubConnection): Promise<void> {
    if (this.isListening)
      throw new Error("Already listening to a connection");
    this._connection = connection;
    for (const [k, v] of this._handlers) {
      this._connection.on(k, v);
    }
    this._loggedUsers.clear();
    const response = await this._connection!.invoke<GetLoggedUsersResponse>('GetUsersLoggedInAsync');
    for (const user of response.users)
      this._loggedUsers.set(user.id, user);
  }

  public detach(): void {
    if (!this.isListening)
      throw new Error('Not listening to any connection');
    for (const [k, v] of this._handlers) {
      this._connection!.off(k, v);
    }
    this._connection = null;
  }

  private handleConnect(message: ConnectMessage) {
    this._loggedUsers.set(message.userId, { id: message.userId, name: message.userName, image: message.userImage });
    messenger.send('Connect', message);
  }

  private handleDisconnect(message: DisconnectMessage) {
    this._loggedUsers.delete(message.userId);
    messenger.send('Disconnect', message);
  }

  private handleJoinRoom(message: JoinRoomMessage) {
    messenger.send('JoinRoom', message);
  }

  private handleLeaveRoom(message: LeaveRoomMessage) {
    messenger.send('LeaveRoom', message);
  }

  private handleContent(message: ContentMessage) {
    let messages = this._contentMessages.get(message.roomId);
    if (!messages) {
      this._contentMessages.set(message.roomId, messages = []);
    }
    messages.push(message);
    messenger.send('Content', message);
  }
}

class HubClient {
  private readonly _url: string = '/hubs/letstalk';
  private _listener?: Listener;
  private _connection: HubConnection | undefined;
  private _isConnecting = false;

  public get isConnecting(): boolean { return this._isConnecting; }
  public get isConnected(): boolean { return !!this._connection && this._connection.state === HubConnectionState.Connected; }

  public get loggedUsers(): User[] { return Array.from(this._listener!.loggedUsers.values()); }

  public getRoomMessages(roomId: string): ContentMessage[] {
    if (!this.isConnected)
      throw new Error('Not connected');
    return this._listener!.getRoomMessages(roomId);
  }

  public async connect(): Promise<void> {
    if (this._isConnecting) {
      return;
    }
    
    this._isConnecting = true;

    try {
      if (this._connection) {
        await this.disconnect();
      }

      this._listener = new Listener();

      this._connection = new HubConnectionBuilder()
        .withUrl(this._url, {
          skipNegotiation: true,
          transport: HttpTransportType.WebSockets,
          headers: {
            'X-CSRF': '1'
          }
        })
        .configureLogging(LogLevel.Debug)
        .withAutomaticReconnect()
        .build();
      await this._connection.start();
      await this._listener.attach(this._connection);
    }
    finally {
      this._isConnecting = false;
    }
  }

  public async disconnect(): Promise<void> {
    if (this.isConnected) {
      this._listener!.detach();
      await this._connection!.stop();
    }
  }

  public async sendContentMessage(roomId: string, contentType: string, contentBase64: string): Promise<void> {
    if (!this.isConnected)
      throw new Error('Not connected');
    return await this._connection!.invoke('SendContentMessageAsync', roomId, contentType, contentBase64);
  }

  public async getRoomsWithUser(): Promise<GetUserRoomsResponse> {
    if (!this.isConnected)
      throw new Error('Not connected');
    return await this._connection!.invoke('GetRoomsWithUserAsync');
  }
}

export const hubClient = new HubClient();