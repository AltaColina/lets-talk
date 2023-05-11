<script lang="ts">
      import { userProfile } from "../../services/store";
</script>

<svelte:head>
	<title>Let's Talk - Profile</title>
	<meta name="description" content="Let's Talk Svelte app" />
</svelte:head>

{#if $userProfile.isLoggedIn}
<div class="relative self-center flex w-96 flex-col rounded-xl bg-white bg-clip-border text-gray-700 shadow-md">
    <div class="relative mx-4 mt-4 h-80 overflow-hidden rounded-xl bg-white bg-clip-border text-gray-700 shadow-lg">
      <img class="w-full" src="{$userProfile.picture}" alt="profile-avatar" />
    </div>
    <div class="p-6 text-center">
      <h4 class="mb-2 block font-sans text-2xl font-semibold leading-snug tracking-normal text-blue-gray-900 antialiased">
         {$userProfile.displayName}
      </h4>
      <p class="block bg-gray-500 bg-clip-text font-sans text-base font-medium leading-relaxed text-transparent antialiased">
            {$userProfile.subjectId}
      </p>
    </div>
    <div class="flex justify-center gap-7 p-6 pt-2">
      <a
        href="#facebook"
        class="block bg-gradient-to-tr from-blue-600 to-blue-400 bg-clip-text font-sans text-xl font-normal leading-relaxed text-transparent antialiased">
        <i class="fab fa-facebook" aria-hidden="true"></i>
      </a>
      <a
        href="#twitter"
        class="block bg-gradient-to-tr from-light-blue-600 to-light-blue-400 bg-clip-text font-sans text-xl font-normal leading-relaxed text-transparent antialiased">
        <i class="fab fa-twitter" aria-hidden="true"></i>
      </a>
      <a
        href="#instagram"
        class="block bg-gradient-to-tr from-purple-600 to-purple-400 bg-clip-text font-sans text-xl font-normal leading-relaxed text-transparent antialiased">
        <i class="fab fa-instagram" aria-hidden="true"></i>
      </a>
    </div>
  </div>
  {/if}