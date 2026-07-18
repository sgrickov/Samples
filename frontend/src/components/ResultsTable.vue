<script setup lang="ts">
import type { SolicitorListingView } from '../types/api'
import RatingBadge from './RatingBadge.vue'
import NewBadge from './NewBadge.vue'
import { useSortableList } from '../composables/useSortableList'

const props = defineProps<{ listings: SolicitorListingView[] }>()

const { sortKey, sortDirection, toggleSort, sorted } = useSortableList<SolicitorListingView>(
  () => props.listings,
  'name',
  {
    name: (listing) => listing.name.toLowerCase(),
    address: (listing) => listing.address.toLowerCase(),
    phone: (listing) => listing.phone ?? '',
    rating: (listing) => listing.rating,
    accredited: (listing) => (listing.isAccredited ? 1 : 0),
  },
)

function ariaSort(key: string): 'ascending' | 'descending' | undefined {
  if (sortKey.value !== key) return undefined
  return sortDirection.value === 'asc' ? 'ascending' : 'descending'
}

function indicator(key: string): string {
  if (sortKey.value !== key) return ''
  return sortDirection.value === 'asc' ? ' ▲' : ' ▼'
}
</script>

<template>
  <table v-if="sorted.length > 0" class="data-table">
    <thead>
      <tr>
        <th class="sortable" :aria-sort="ariaSort('name')" @click="toggleSort('name')">Name{{ indicator('name') }}</th>
        <th class="sortable" :aria-sort="ariaSort('address')" @click="toggleSort('address')">
          Address{{ indicator('address') }}
        </th>
        <th class="sortable" :aria-sort="ariaSort('phone')" @click="toggleSort('phone')">
          Phone{{ indicator('phone') }}
        </th>
        <th class="sortable" :aria-sort="ariaSort('rating')" @click="toggleSort('rating')">
          Rating{{ indicator('rating') }}
        </th>
        <th class="sortable" :aria-sort="ariaSort('accredited')" @click="toggleSort('accredited')">
          Accredited{{ indicator('accredited') }}
        </th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="listing in sorted" :key="`${listing.name}-${listing.address}`">
        <td>
          {{ listing.name }}
          <NewBadge v-if="listing.isNewSinceLastRun" />
        </td>
        <td>{{ listing.address }}</td>
        <td>{{ listing.phone ?? '—' }}</td>
        <td><RatingBadge :rating="listing.rating" :review-count="listing.reviewCount" /></td>
        <td>{{ listing.isAccredited ? 'Yes' : '—' }}</td>
      </tr>
    </tbody>
  </table>
  <p v-else>No results for this location.</p>
</template>
