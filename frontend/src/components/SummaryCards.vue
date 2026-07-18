<script setup lang="ts">
import type { NationalSummary } from '../types/api'
import StatCard from './StatCard.vue'

defineProps<{ national: NationalSummary }>()
</script>

<template>
  <section class="summary-cards">
    <StatCard :value="national.totalSolicitorsFound" label="Solicitors found" />
    <StatCard :value="national.totalLocationsSearched" label="Locations searched" />
    <StatCard :value="`${national.averageRating.toFixed(1)}★`" label="Average rating" />
    <StatCard :value="`${national.accreditedPercentage}%`" :label="`Accredited (${national.accreditedCount})`" />
  </section>
</template>

<style scoped>
.summary-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
  gap: 0.75rem;
  margin: 1.5rem 0;
}
</style>
