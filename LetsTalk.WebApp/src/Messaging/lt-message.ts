export interface LTMessage<T> {
  id: string;
  timestamp: string;
  content: T;
}