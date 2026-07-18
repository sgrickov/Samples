<script setup lang="ts">
import type { LocationReport } from '../types/api'
import NewBadge from './NewBadge.vue'

defineProps<{ location: LocationReport }>()
</script>

<template>
  <div class="location-card">
    <div class="header">
      <h3>{{ location.locationName }}</h3>
      <NewBadge v-if="location.newSinceLastRun.length > 0" :label="`${location.newSinceLastRun.length} new`" />
    </div>
    <p class="stat"><strong>{{ location.totalFound }}</strong> found</p>
    <p class="stat">{{ location.averageRating.toFixed(1) }}★ average rating</p>
    <p class="stat">{{ location.accreditedCount }} accredited</p>
  </div>
</template>

<style scoped>
.location-card {
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 0.75rem 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
}

.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.header h3 {
  margin: 0;
  font-size: 1.05rem;
}

.stat {
  font-size: 0.9rem;
  color: var(--text-muted);
}
</style>
