<script setup lang="ts">
import type { SolicitorListingView } from '../types/api'
import RatingBadge from './RatingBadge.vue'

defineProps<{ title: string; listings: SolicitorListingView[] }>()
</script>

<template>
  <section class="top-rated">
    <h3>{{ title }}</h3>
    <table v-if="listings.length > 0" class="data-table">
      <thead>
        <tr>
          <th>Name</th>
          <th>Location</th>
          <th>Rating</th>
          <th>Phone</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="listing in listings" :key="`${listing.locationName}-${listing.name}`">
          <td>{{ listing.name }}</td>
          <td>{{ listing.locationName }}</td>
          <td><RatingBadge :rating="listing.rating" :review-count="listing.reviewCount" /></td>
          <td>{{ listing.phone ?? '—' }}</td>
        </tr>
      </tbody>
    </table>
    <p v-else>No data yet.</p>
  </section>
</template>

<style scoped>
.top-rated {
  margin: 1.5rem 0;
}
</style>
