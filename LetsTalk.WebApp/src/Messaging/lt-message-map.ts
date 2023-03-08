import { ConnectMessage } from "./connect-message";
import { ContentMessage } from "./content-message";
import { DisconnectMessage } from "./disconnect-message";
import { JoinChatMessage } from "./join-chat-message";
import { LeaveChatMessage } from "./leave-chat-message";

export interface LTMessageMap {
  'content': ContentMessage;
  'connect': ConnectMessage;
  'disconnect': DisconnectMessage;
  'joinchat': JoinChatMessage;
  'leavechat': LeaveChatMessage;
}

export type LTMessageType = keyof LTMessageMap;