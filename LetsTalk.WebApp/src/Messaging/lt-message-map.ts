import { ConnectMessage } from "./connect-message";
import { ContentMessage } from "./content-message";
import { DisconnectMessage } from "./disconnect-message";
import { JoinRoomMessage } from "./join-room-message";
import { LeaveRoomMessage } from "./leave-room-message";

export interface LTMessageMap {
  'content': ContentMessage;
  'connect': ConnectMessage;
  'disconnect': DisconnectMessage;
  'joinroom': JoinRoomMessage;
  'leaveroom': LeaveRoomMessage;
}

export type LTMessageType = keyof LTMessageMap;