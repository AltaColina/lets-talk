import { IMessenger } from "../Services/messenger";

export const attachMessageHandlers = (messenger: IMessenger) => {
    const handleMessageEvent = (e: CustomEvent) => console.log(e);

    window.addEventListener('load', () => {
        messenger.on('connect', handleMessageEvent);
        messenger.on('disconnect', handleMessageEvent);
        messenger.on('joinroom', handleMessageEvent);
        messenger.on('leaveroom', handleMessageEvent);
        messenger.on('content', handleMessageEvent);
        console.log('attached console handlers');
    });
    window.addEventListener('unload', () => {
        messenger.off('connect', handleMessageEvent);
        messenger.off('disconnect', handleMessageEvent);
        messenger.off('joinroom', handleMessageEvent);
        messenger.off('leaveroom', handleMessageEvent);
        messenger.off('content', handleMessageEvent);
        console.log('detached console handlers');
    });
}