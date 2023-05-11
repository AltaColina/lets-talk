<script lang="ts">
    import { derived } from 'svelte/store';
    import type { ContentMessage } from '../../../models/messaging/messages';
    import { decodeText } from '../../../services/content';
    import { userProfile } from '../../../services/store';

    export let message: ContentMessage;

    const ownStyle = {
        columnAlignment: 'col-start-6 col-end-13',
        rowAlignment: 'flex-row-reverse justify-start',
        bgColor: 'bg-tertiary-100-800-token',
        ml: 'ml-4',
    };

    const otherStyle = {
        columnAlignment: 'col-start-1 col-end-8',
        rowAlignment: 'flex-row',
        bgColor: 'bg-tertiary-400-500-token',
        ml: 'ml-0',
    };

    const styling = derived(userProfile, (user) => (user.subjectId === message.userId ? ownStyle : otherStyle));
</script>

<div class="{$styling.columnAlignment} p-3 rounded-lg">
    <div class="flex {$styling.rowAlignment} items-center">
        <div
            class="flex items-center justify-center h-10 w-10 rounded-full {$styling.bgColor} flex-shrink-0 {$styling.ml}"
        >
            {message.userName
                .split(' ')
                .map((s) => s[0].toUpperCase())
                .join()}
        </div>
        <div class="relative ml-3 text-sm {$styling.bgColor} py-2 px-4 shadow rounded-xl">
            {#if message.contentType === 'text/plain'}
                <div>{decodeText(message.content)}</div>
            {:else}
                <div>
                    {message.contentType} ({message.content.length} bytes)
                </div>
            {/if}
        </div>
    </div>
</div>
