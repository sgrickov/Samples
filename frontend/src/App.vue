<script setup lang="ts">
import LocationsEditor from './components/LocationsEditor.vue'
import SearchTrigger from './components/SearchTrigger.vue'
import HistoryPanel from './components/HistoryPanel.vue'
import ReportView from './components/ReportView.vue'
import { useAppBootstrap } from './composables/useAppBootstrap'

const { report, isLoadingReport } = useAppBootstrap()
</script>

<template>
  <main class="app">
    <h1>InfoTrack Solicitors Report</h1>

    <div class="toolbar">
      <LocationsEditor />
      <SearchTrigger />
    </div>

    <HistoryPanel />

    <p v-if="isLoadingReport">Loading…</p>
    <p v-else-if="!report">No search has been run yet. Click "Run Search" above to get started.</p>
    <ReportView v-else :report="report" />
  </main>
</template>

<style scoped>
.app {
  max-width: 1000px;
  margin: 0 auto;
  padding: 2rem 1rem 4rem;
}

.toolbar {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin: 1.5rem 0 2rem;
}
</style>
