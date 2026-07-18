<script setup lang="ts">
import { computed } from 'vue'
import type { LocationReport } from '../types/api'
import RatingBadge from './RatingBadge.vue'

const props = defineProps<{ locations: LocationReport[] }>()

const newListings = computed(() => props.locations.flatMap((location) => location.newSinceLastRun))
</script>

<template>
  <section v-if="newListings.length > 0" class="new-solicitors">
    <h3>New since last search ({{ newListings.length }})</h3>
    <ul>
      <li v-for="listing in newListings" :key="`${listing.locationName}-${listing.name}`">
        <strong>{{ listing.name }}</strong> — {{ listing.locationName }}
        (<RatingBadge :rating="listing.rating" :review-count="listing.reviewCount" />)
      </li>
    </ul>
  </section>
</template>

<style scoped>
.new-solicitors {
  margin: 1.5rem 0;
  border: 1px solid #86efac;
  background: #f0fdf4;
  border-radius: 8px;
  padding: 0.75rem 1rem;
}

.new-solicitors h3 {
  margin: 0 0 0.5rem;
  color: #166534;
}

.new-solicitors ul {
  margin: 0;
  padding-left: 1.25rem;
  font-size: 0.9rem;
}
</style>
