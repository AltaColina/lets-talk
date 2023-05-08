import { Messenger } from "../Services/messenger";
import IDisposable from "./idisposable";

export default function attachDebugListeners(messenger: Messenger): void {
    const handleMessageEvent = (e: CustomEvent) => console.log(e);

    const subscriptions = new Array<IDisposable>();

    window.addEventListener('load', () => {
        subscriptions.push(
            messenger.on('Connect', handleMessageEvent),
            messenger.on('Disconnect', handleMessageEvent),
            messenger.on('JoinRoom', handleMessageEvent),
            messenger.on('LeaveRoom', handleMessageEvent),
            messenger.on('Content', handleMessageEvent));
        console.info('attached listeners');
    });
    window.addEventListener('unload', () => {
        subscriptions.forEach(s => s.dispose());
        subscriptions.length = 0;
        console.info('detached listeners');
    });
}