export type LTMessageType = 'Connect' | 'Disconnect' | 'JoinRoom' | 'LeaveRoom' | 'Content';
export interface LTMessage<T extends LTMessageType> {
  id: string;
  type: T;
  timestamp: string;
}