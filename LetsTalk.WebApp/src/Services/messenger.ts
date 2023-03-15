import { LTMessageType } from "../Messaging/lt-message";
import { LTMessageMap } from "../Messaging/lt-message-map";

export interface IMessenger {
  on<T extends LTMessageType>(type: T, handler: (e: CustomEvent<LTMessageMap[T]>) => any): any,
  off<T extends LTMessageType>(type: T, handler: (e: CustomEvent<LTMessageMap[T]>) => any): any,
  once<T extends LTMessageType>(type: T, handler: (e: CustomEvent<LTMessageMap[T]>) => any): any,
  emit<T extends LTMessageType>(type: T, data: LTMessageMap[T]): any
}

export const messenger: IMessenger = {
  on: <T extends LTMessageType>(type: T, handler: (e: CustomEvent<LTMessageMap[T]>) => any) => {
    document.addEventListener<any>(type, handler);
  },
  off: <T extends LTMessageType>(type: T, handler: (e: CustomEvent<LTMessageMap[T]>) => any) => {
    document.removeEventListener<any>(type, handler);
  },
  once: <T extends LTMessageType>(type: T, handler: (e: CustomEvent<LTMessageMap[T]>) => any) => {
    messenger.on(type, handleOnce);
    function handleOnce(e: CustomEvent<LTMessageMap[T]>) {
      handler(e);
      messenger.off(type, handleOnce);
    }
  },
  emit: <T extends LTMessageType>(type: T, data: LTMessageMap[T]) => {
    document.dispatchEvent(new CustomEvent(type, { detail: data }));
  }
}