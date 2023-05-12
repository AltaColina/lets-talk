import type { LTMessageMap } from "../models/messaging/messages.js";
import type { Disposable } from "./disposable.js";

export type MessengerMap = LTMessageMap;

export type MessageType = keyof MessengerMap;

function makeType<T extends MessageType>(type: T, token: any) {
    return typeof token === 'string' ? type + token : type;
}

function makeHandler<T extends MessageType>(tokenOrHandler: string | ((e: CustomEvent<MessengerMap[T]>) => any), handler?: (e: CustomEvent<MessengerMap[T]>) => any) {
    if (typeof tokenOrHandler !== 'string') {
        return tokenOrHandler;
    }
    else if (handler) {
        return handler;
    }
    else {
        throw new Error('Invalid event handler');
    }
}

function makeData<T extends MessageType>(tokenOrData: string | MessengerMap[T], data?: MessengerMap[T]) {
    if (typeof tokenOrData !== 'string') {
        return tokenOrData;
    }
    else if (data) {
        return data;
    }
    else {
        throw new Error('Invalid event data');
    }
}

export class Messenger {

    on<T extends MessageType>(type: T, handler: (e: CustomEvent<MessengerMap[T]>) => any): Disposable
    on<T extends MessageType>(type: T, token: string, handler: (e: CustomEvent<MessengerMap[T]>) => any): Disposable
    on<T extends MessageType>(type: T, tokenOrHandler: string | ((e: CustomEvent<MessengerMap[T]>) => any), handler?: (e: CustomEvent<MessengerMap[T]>) => any): Disposable {
        const eventType = makeType(type, tokenOrHandler);
        const eventHandler = makeHandler(tokenOrHandler, handler);
        document.addEventListener<any>(eventType, eventHandler);
        return () => document.removeEventListener<any>(eventType, eventHandler);
    }

    once<T extends MessageType>(type: T, handler: (e: CustomEvent<MessengerMap[T]>) => any): void
    once<T extends MessageType>(type: T, token: string, handler: (e: CustomEvent<MessengerMap[T]>) => any): void
    once<T extends MessageType>(type: T, tokenOrHandler: string | ((e: CustomEvent<MessengerMap[T]>) => any), handler?: (e: CustomEvent<MessengerMap[T]>) => any): void {
        const eventHandler = makeHandler(tokenOrHandler, handler);
        if (typeof tokenOrHandler !== 'string') {
            const dispose = messenger.on<T>(type, e => {
                eventHandler(e);
                dispose();
            });
        }
        else {
            const dispose = messenger.on<T>(type, tokenOrHandler, e => {
                eventHandler(e);
                dispose();
            });
        }
    }

    send<T extends MessageType>(type: T, data: MessengerMap[T]): void
    send<T extends MessageType>(type: T, token: string, data: MessengerMap[T]): void
    send<T extends MessageType>(type: T, tokenOrData: string | MessengerMap[T], data?: MessengerMap[T]): void {
        const eventType = makeType(type, tokenOrData);
        const eventData = {
            detail: makeData(tokenOrData, data)
        };
        document.dispatchEvent(new CustomEvent(eventType, eventData));
    }
}

export const messenger = new Messenger();