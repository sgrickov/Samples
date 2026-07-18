<script setup lang="ts">
import type { SearchReport } from '../types/api'
import SummaryCards from './SummaryCards.vue'
import LocationSummaryCard from './LocationSummaryCard.vue'
import TopRatedTable from './TopRatedTable.vue'
import NewSolicitorsPanel from './NewSolicitorsPanel.vue'
import ResultsTable from './ResultsTable.vue'

defineProps<{ report: SearchReport }>()
</script>

<template>
  <div class="report-view">
    <SummaryCards :national="report.national" />

    <NewSolicitorsPanel :locations="report.locations" />

    <TopRatedTable title="Nationally top-rated" :listings="report.national.topRated" />

    <section class="locations-grid">
      <LocationSummaryCard v-for="location in report.locations" :key="location.locationName" :location="location" />
    </section>

    <section v-for="location in report.locations" :key="`table-${location.locationName}`" class="location-results">
      <h3>{{ location.locationName }} — all results</h3>
      <ResultsTable :listings="location.allListings" />
    </section>
  </div>
</template>

<style scoped>
.locations-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 0.75rem;
  margin: 1.5rem 0;
}

.location-results {
  margin: 1.5rem 0;
  overflow-x: auto;
}

.location-results h3 {
  margin: 0 0 0.5rem;
}
</style>
