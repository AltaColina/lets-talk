<script>
    import { userProfile } from '../services/store';
</script>

<svelte:head>
    <title>Let's Talk</title>
    <meta name="description" content="Let's Talk Svelte app" />
</svelte:head>

<section class="h-full flex flex-col">
    <h1 class="m-8 text-center text-4xl h-full">
        Welcome to <span class="text-blue-500">Let's Talk</span>,
        <span class="font-bold">{$userProfile.displayName}</span>!
    </h1>
</section>
