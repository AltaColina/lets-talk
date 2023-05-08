import { ConnectMessage } from "./connect-message";
import { ContentMessage } from "./content-message";
import { DisconnectMessage } from "./disconnect-message";
import { JoinRoomMessage } from "./join-room-message";
import { LeaveRoomMessage } from "./leave-room-message";

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