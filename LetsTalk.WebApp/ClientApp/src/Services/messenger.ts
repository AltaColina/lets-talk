import { Theme } from "@mui/material";
import IDisposable from "../Callbacks/idisposable";
import { LTMessageMap } from "../Messaging/lt-message";

export interface UIMessageMap {
  'ThemeChanged': Theme;
}

export type MessengerMap = LTMessageMap & UIMessageMap;

export type MessageType = keyof MessengerMap;

export class Messenger {
  on<T extends MessageType>(type: T, handler: (e: CustomEvent<MessengerMap[T]>) => any): IDisposable {
    document.addEventListener<any>(type, handler);
    return {
      dispose() {
        document.removeEventListener<any>(type, handler)
      }
    };
  }

  once<T extends MessageType>(type: T, handler: (e: CustomEvent<MessengerMap[T]>) => any): void {
    const subscription = messenger.on<T>(type, e => {
      handler(e);
      subscription.dispose();
    });
  }

  send<T extends MessageType>(type: T, data: MessengerMap[T]): void {
    document.dispatchEvent(new CustomEvent(type, { detail: data }));
  }
}

export const messenger = new Messenger();