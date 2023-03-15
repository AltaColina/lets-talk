import { IMessenger } from "../Services/messenger";

export const attachMessageHandlers = (messenger: IMessenger) => {
    const handleMessageEvent = (e: CustomEvent) => console.log(e);

    window.addEventListener('load', () => {
        messenger.on('Connect', handleMessageEvent);
        messenger.on('Disconnect', handleMessageEvent);
        messenger.on('JoinRoom', handleMessageEvent);
        messenger.on('LeaveRoom', handleMessageEvent);
        messenger.on('Content', handleMessageEvent);
        console.log('attached console handlers');
    });
    window.addEventListener('unload', () => {
        messenger.off('Connect', handleMessageEvent);
        messenger.off('Disconnect', handleMessageEvent);
        messenger.off('JoinRoom', handleMessageEvent);
        messenger.off('LeaveRoom', handleMessageEvent);
        messenger.off('Content', handleMessageEvent);
        console.log('detached console handlers');
    });
}