<script lang="ts">
    import { onMount } from 'svelte';
    import { http } from '../../http';
    import { greeting } from '../../store';

    onMount(async () => {
        await http.get('/api/greet')
            .then(response => response.text())
            .then(text => greeting.set(text));
        })
</script>

<h1>{$greeting}</h1>