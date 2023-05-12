<script lang="ts">
    import '@skeletonlabs/skeleton/styles/skeleton.css';
    import '@skeletonlabs/skeleton/themes/theme-skeleton.css';
    import { onDestroy, onMount } from 'svelte';
    import { derived } from 'svelte/store';
    import '../app.postcss';
    import type { AsyncDisposable } from '../services/disposable';
    import { bffApi, greetApi } from '../services/httpClient';
    import { hubClient } from '../services/hubClient';
    import { greeting, onlineUsersById, rooms, userProfile } from '../services/store';

    let profileMenuButton: HTMLButtonElement;
    let profileContextMenu: HTMLDivElement;
    let isProfileMenuOpen = false;

    const onlineUsers = derived(onlineUsersById, (users) => Array.from(users.values()));

    const onProfileMenuClick = () => {
        if (!isProfileMenuOpen) {
            const onOutsideClick = (e: MouseEvent) => {
                if (isProfileMenuOpen) {
                    const isOutside = !profileContextMenu.contains(e.target as Node);
                    const isProfileMenuClick = profileMenuButton.contains(e.target as Node);
                    if (isOutside && !isProfileMenuClick) {
                        document.removeEventListener('mousedown', onOutsideClick);
                        isProfileMenuOpen = false;
                    }
                }
            };
            document.addEventListener('mousedown', onOutsideClick);
            isProfileMenuOpen = true;
        }
    };

    let dispose = undefined as AsyncDisposable | undefined;
    onMount(async () => {
        userProfile.set(await bffApi.user());
        greeting.set(await greetApi.get());
        if ($userProfile.isLoggedIn) {
            dispose = await hubClient.connect();
            const roomsResponse = await hubClient.getRoomsWithUser();
            rooms.set(roomsResponse.rooms);
        }
    });
    onDestroy(() => {
        if (dispose) dispose();
    });
</script>

<!-- This is an example component -->
<div class="container h-screen w-screen m-0 shadow-lg rounded-lg">
    <!-- header -->
    <div class="px-5 py-5 flex justify-between items-center bg-secondary-700 border-b-2">
        <a href="/" class="flex items-center flex-shrink-0 text-primary-500 mr-6">
            <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 24 24"
                stroke-width="1"
                stroke="black"
                class="fill-current w-8 h-8 mr-2 stroke-secondary-500"
            >
                <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M20.25 8.511c.884.284 1.5 1.128 1.5 2.097v4.286c0 1.136-.847 2.1-1.98 2.193-.34.027-.68.052-1.02.072v3.091l-3-3c-1.354 0-2.694-.055-4.02-.163a2.115 2.115 0 01-.825-.242m9.345-8.334a2.126 2.126 0 00-.476-.095 48.64 48.64 0 00-8.048 0c-1.131.094-1.976 1.057-1.976 2.192v4.286c0 .837.46 1.58 1.155 1.951m9.345-8.334V6.637c0-1.621-1.152-3.026-2.76-3.235A48.455 48.455 0 0011.25 3c-2.115 0-4.198.137-6.24.402-1.608.209-2.76 1.614-2.76 3.235v6.226c0 1.621 1.152 3.026 2.76 3.235.577.075 1.157.14 1.74.194V21l4.155-4.155"
                />
            </svg>
            <span class="font-semibold text-xl tracking-tight">Let's Talk</span>
        </a>
        <div>
            <div>
                <button
                    type="button"
                    class="w-12 h-12 rounded-full bg-tertiary-500 hover:bg-tertiary-300 shadow-sm shadow-primary-950"
                    bind:this={profileMenuButton}
                    on:click={onProfileMenuClick}
                >
                    {#if $userProfile.isLoggedIn}
                        <img class="rounded-full" src={$userProfile.picture} alt="user avatar" />
                    {:else}
                        <div class="text-center text-1xl font-semibold">
                            <span class="align-middle">G</span>
                        </div>
                    {/if}
                </button>
            </div>
            <div
                class="absolute right-2 top-16 z-10 m-2 w-56 origin-top-right rounded-md text-primary-900 bg-tertiary-300 shadow-lg shadow-primary-950 ring-1 ring-black ring-opacity-5 focus:outline-none"
            >
                <div bind:this={profileContextMenu} class="py-1" class:hidden={isProfileMenuOpen === false}>
                    {#if $userProfile.isLoggedIn}
                        <a href={$userProfile.logoutUrl} class="block px-4 py-2 hover:bg-tertiary-500" id="menu-item-0"
                            >Sign out</a
                        >
                    {:else}
                        <a href="/bff/login" class="block px-4 py-2 hover:bg-tertiary-500" id="menu-item-0">Sign in</a>
                    {/if}
                </div>
            </div>
        </div>
    </div>
    <!-- end header -->
    <!-- Chatting -->
    <div class="flex flex-row justify-between">
        <!-- chat list -->
        <div class="flex flex-col w-2/5 overflow-y-auto">
            <!-- chat -->
            {#each $rooms as room}
                <a
                    href="/room/{room.id}"
                    class="flex flex-row my-0.5 justify-center items-center bg-secondary-500 hover:bg-primary-300"
                >
                    <div class="w-1/4 p-2">
                        <div class="flex flex-col w-12 h-12 bg-secondary-700 rounded-full text-2xl font-semibold">
                            <div class="pl-4 pt-2 text-primary-100">{room.name[0].toUpperCase()}</div>
                        </div>
                    </div>
                    <div class="w-full">
                        <div class="text-lg font-semibold text-primary-100">{room.name}</div>
                    </div>
                </a>
            {/each}
            <!-- end user list -->
        </div>
        <!-- end chat list -->
        <!-- message -->
        <div class="w-full px-5 flex flex-col justify-between">
            <slot />
        </div>
        <!-- end message -->

        <div class="flex flex-col w-2/5 overflow-y-auto rounded-lg">
            <!-- chat -->
            {#each $onlineUsers as onlineUser}
                <div class="flex flex-row my-0.5 justify-center items-center bg-secondary-500 hover:bg-primary-300">
                    <div class="w-full pl-4">
                        <div class="text-lg font-semibold text-primary-100">{onlineUser.name}</div>
                    </div>
                    <div class="w-1/4 p-2">
                        <div class="flex flex-col w-12 h-12 bg-secondary-700 rounded-full text-2xl font-semibold">
                            <div class="pl-4 pt-2 text-primary-100">{onlineUser.name[0].toUpperCase()}</div>
                        </div>
                    </div>
                </div>
            {/each}
            <!-- end user list -->
        </div>
    </div>
</div>

<!-- <svelte:window bind:innerWidth={width} /> -->

<!-- <nav class="flex items-center justify-between flex-wrap bg-seceondary-500 p-6">
    <div class="flex items-center flex-shrink-0 text-primary-500 mr-6">
        <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            stroke-width="1"
            stroke="black"
            class="fill-current w-8 h-8 mr-2 stroke-secondary-500"
        >
            <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M20.25 8.511c.884.284 1.5 1.128 1.5 2.097v4.286c0 1.136-.847 2.1-1.98 2.193-.34.027-.68.052-1.02.072v3.091l-3-3c-1.354 0-2.694-.055-4.02-.163a2.115 2.115 0 01-.825-.242m9.345-8.334a2.126 2.126 0 00-.476-.095 48.64 48.64 0 00-8.048 0c-1.131.094-1.976 1.057-1.976 2.192v4.286c0 .837.46 1.58 1.155 1.951m9.345-8.334V6.637c0-1.621-1.152-3.026-2.76-3.235A48.455 48.455 0 0011.25 3c-2.115 0-4.198.137-6.24.402-1.608.209-2.76 1.614-2.76 3.235v6.226c0 1.621 1.152 3.026 2.76 3.235.577.075 1.157.14 1.74.194V21l4.155-4.155"
            />
        </svg>
        <span class="font-semibold text-xl tracking-tight">Let's Talk</span>
    </div>
    <div class="flex-grow content-end">{$userProfile.displayName}</div>
    <div>
        <div>
            <button
                type="button"
                class="w-10 h-10 rounded-full bg-tertiary-500 hover:bg-tertiary-300 shadow-sm shadow-primary-950"
                bind:this={profileMenuButton}
                on:click={onProfileMenuClick}
            >
                {#if $userProfile.isLoggedIn}
                    <img class="rounded-full" src={$userProfile.picture} alt="user avatar" />
                {:else}
                    <div class="text-center text-1xl font-semibold">
                        <span class="align-middle">G</span>
                    </div>
                {/if}
            </button>
        </div>
        <div
            class="absolute right-2 top-14 z-10 m-2 w-56 origin-top-right rounded-md text-primary-900 bg-tertiary-300 shadow-lg shadow-primary-950 ring-1 ring-black ring-opacity-5 focus:outline-none"
        >
            <div bind:this={profileContextMenu} class="py-1" class:hidden={isProfileMenuOpen === false}>
                {#if $userProfile.isLoggedIn}
                    <a href={$userProfile.logoutUrl} class="block px-4 py-2 hover:bg-tertiary-500" id="menu-item-0"
                        >Sign out</a
                    >
                {:else}
                    <a href="/bff/login" class="block px-4 py-2 hover:bg-tertiary-500" id="menu-item-0">Sign in</a>
                {/if}
            </div>
        </div>
    </div>
</nav>

<main class="flex h-full w-full">
    <div class="basis-1/6">left side bar</div>
    <div class="flex-grow">
        <slot />
    </div>
    <div class="basis-1/6">right side bar</div>
</main> -->

<!-- <div class="flex flex-col w-full h-full">
    <div class="flex flex-row bg-secondary-600 h-16 shadow-lg">
        <div
            id="menu-button"
            aria-expanded="true"
            aria-haspopup="true"
            class="m-2 w-12 h-12 rounded-full bg-tertiary-500 hover:bg-tertiary-300 shadow-sm shadow-primary-950"
        >
            {#if $userProfile.isLoggedIn}
                <img src={$userProfile.picture} alt="user avatar" />
            {:else}
                <div class="text-center text-3xl">
                    <span class="align-middle">G</span>
                </div>
            {/if}
        </div>
    </div>
    <div class="flex flex-row flex-auto">
        <div class="basis-1/5 w-full h-full">
            <span class="text-primary-300">sdafs</span>
        </div>

        <main class="w-full h-full">
            <slot />
        </main>
        <div class="w-full h-full basis-1/5">02</div>
    </div>
</div> -->

<!-- <Navbar let:toggle>
    <span />
    <div class="flex items-end md:order-2">
        <Avatar id="avatar-menu" src={$userProfile.picture} on:click={toggle} />
        <NavHamburger class1="w-full md:flex md:w-auto md:order-2" />
    </div>
    <Dropdown placement="bottom" triggeredBy="#avatar-menu">
        {#if $userProfile.isLoggedIn}
            <DropdownHeader>
                <span class="block text-sm"> {$userProfile.displayName} </span>
                <span class="block truncate text-sm font-medium">
                    {$userProfile.subjectId}
                </span>
            </DropdownHeader>
            <DropdownDivider />
            <DropdownItem href={$userProfile.logoutUrl}>Sign out</DropdownItem>
        {:else}
            <DropdownItem href="/bff/login">Sign in</DropdownItem>
        {/if}
    </Dropdown>
</Navbar>
<Drawer
    transitionType="fly"
    {backdrop}
    {transitionParams}
    bind:hidden={drawerHidden}
    bind:activateClickOutside
    width="w-64"
    class="pb-32"
    id="sidebar"
>
    <div class="flex items-center">
        <CloseButton
            on:click={() => (drawerHidden = true)}
            class="mb-4 dark:text-white lg:hidden"
        />
    </div>
    <Sidebar asideClass="w-54">
        <SidebarWrapper
            divClass="overflow-y-auto py-4 px-3 rounded dark:bg-gray-800"
        >
            <SidebarGroup>
                <SidebarItem
                    label="Let's Talk"
                    href="/"
                    on:click={toggleSide}
                    active={activeUrl === `/`}
                />
                {#each $rooms as room}
                    <SidebarItem
                        label={room.name}
                        href="/room/{room.id}"
                        {spanClass}
                        on:click={toggleSide}
                        active={activeUrl === '/profile'}
                    />
                {/each}
            </SidebarGroup>
        </SidebarWrapper>
    </Sidebar>
</Drawer> -->
