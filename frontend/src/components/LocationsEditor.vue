<script setup lang="ts">
import { useLocationsDraft } from '../composables/useLocationsDraft'

const { draft, newLocation, isSaving, addLocation, removeLocation, resetToDefaults, save } = useLocationsDraft()
</script>

<template>
  <section class="locations-editor">
    <h2>Locations</h2>

    <ul class="chip-list">
      <li v-for="name in draft" :key="name" class="chip">
        {{ name }}
        <button type="button" class="chip-remove" :aria-label="`Remove ${name}`" @click="removeLocation(name)">
          ×
        </button>
      </li>
    </ul>

    <form class="add-form" @submit.prevent="addLocation">
      <input v-model="newLocation" type="text" placeholder="Add a town, city or county" />
      <button type="submit">Add</button>
    </form>

    <div class="actions">
      <button type="button" @click="resetToDefaults">Reset to defaults</button>
      <button type="button" :disabled="isSaving || draft.length === 0" @click="save">
        {{ isSaving ? 'Saving…' : 'Save' }}
      </button>
    </div>
  </section>
</template>

<style scoped>
.locations-editor {
  border: 1px solid #d0d5dd;
  border-radius: 8px;
  padding: 1rem 1.25rem;
}

.chip-list {
  list-style: none;
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  padding: 0;
  margin: 0.75rem 0;
}

.chip {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  background: #eef2ff;
  color: #1e3a8a;
  border-radius: 999px;
  padding: 0.25rem 0.5rem 0.25rem 0.75rem;
  font-size: 0.9rem;
}

.chip-remove {
  border: none;
  background: transparent;
  color: inherit;
  cursor: pointer;
  font-size: 1rem;
  line-height: 1;
  padding: 0 0.15rem;
}

.add-form {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.add-form input {
  flex: 1;
  padding: 0.4rem 0.6rem;
}

.actions {
  display: flex;
  gap: 0.5rem;
}
</style>
