<script lang="ts">
    import { onDestroy, onMount } from 'svelte';
    import { writable } from 'svelte/store';
    import type { ContentMessage } from '../../../models/messaging/messages';
    import { encodeText } from '../../../services/content';
    import type { Disposable } from '../../../services/disposable';
    import { hubClient } from '../../../services/hubClient';
    import { messenger } from '../../../services/messenger';
    import type { RoomPageData } from './+page';
    import Message from './Message.svelte';

    export let data: RoomPageData;

    const roomMessages = writable(new Array<ContentMessage>());

    let contentSubscription = undefined as Disposable | undefined;
    onMount(async () => {
        roomMessages.set(await hubClient.getRoomMessages(data.roomId));
        messenger.on('Content', data.roomId, (msg) => {
            roomMessages.update((list) => [...list, msg.detail]);
        });
    });

    onDestroy(() => contentSubscription?.call(undefined));

    let messageContent: string | undefined = '';

    function onSendClick() {
        if (messageContent) {
            hubClient.sendContentMessage(data.roomId, 'text/plain', encodeText(messageContent));
            messageContent = undefined;
        }
    }

    function onKeyDown(e: KeyboardEvent) {
        if (e.key === 'Enter') {
            onSendClick();
        }
    }
</script>

<div class="flex flex-col flex-auto h-full w-full">
    <div class="flex flex-col flex-auto flex-shrink-0 rounded-2xl h-full p-4">
        <div class="flex flex-col h-full overflow-x-auto mb-4">
            <div class="flex flex-col h-full">
                <div class="grid grid-cols-12 gap-y-0">
                    {#each $roomMessages as message}
                        <Message {message} />
                    {/each}
                </div>
            </div>
        </div>
        <div class="flex flex-row items-center h-20 rounded-xl bg-secondary-700-200-token w-full px-4">
            <div>
                <button class="flex items-center justify-center text-primary-500 hover:text-primary-700">
                    <svg
                        class="w-5 h-5"
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                        xmlns="http://www.w3.org/2000/svg"
                    >
                        <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13"
                        />
                    </svg>
                </button>
            </div>
            <div class="flex-grow ml-4">
                <div class="relative w-full">
                    <input
                        type="text"
                        class="flex w-full border rounded-xl bg-tertiary-50-900-token focus:outline-none text-primary-300 focus:border-primary-500 pl-4 h-14"
                        bind:value={messageContent}
                        on:keydown={onKeyDown}
                    />
                    <button
                        class="absolute flex items-center justify-center h-full w-12 right-0 top-0 text-primary-500 hover:text-primary-700"
                    >
                        <svg
                            class="w-6 h-6"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                            xmlns="http://www.w3.org/2000/svg"
                        >
                            <path
                                stroke-linecap="round"
                                stroke-linejoin="round"
                                stroke-width="2"
                                d="M14.828 14.828a4 4 0 01-5.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                            />
                        </svg>
                    </button>
                </div>
            </div>
            <div class="ml-4">
                <button
                    class="flex items-center justify-center bg-tertiary-50-900-token hover:bg-tertiary-200-700-token rounded-xl text-primary-500 hover:text-primary-500 px-4 py-1 flex-shrink-0"
                    disabled={!messageContent}
                    on:click={onSendClick}
                >
                    <span>Send</span>
                    <span class="ml-2">
                        <svg
                            class="w-4 h-4 transform rotate-45 -mt-px"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                            xmlns="http://www.w3.org/2000/svg"
                        >
                            <path
                                stroke-linecap="round"
                                stroke-linejoin="round"
                                stroke-width="2"
                                d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8"
                            />
                        </svg>
                    </span>
                </button>
            </div>
        </div>
    </div>
</div>
