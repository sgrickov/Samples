<script setup lang="ts">
import { useSearchRun } from '../composables/useSearchRun'

const { isSearching, error, run } = useSearchRun()
</script>

<template>
  <div class="search-trigger">
    <button type="button" :disabled="isSearching" @click="run">
      {{ isSearching ? 'Searching…' : 'Run Search' }}
    </button>
    <p v-if="error" class="error">{{ error }}</p>
  </div>
</template>

<style scoped>
.search-trigger {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.search-trigger button {
  padding: 0.5rem 1.25rem;
  font-weight: 600;
  background: #1e3a8a;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.search-trigger button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error {
  color: #b91c1c;
  margin: 0;
}
</style>
