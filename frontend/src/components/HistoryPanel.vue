<script setup lang="ts">
import { useHistorySelection } from '../composables/useHistorySelection'

const { history, selectedRunId, selectRun, viewLatest, deleteRun, clearHistory } = useHistorySelection()

function confirmDeleteRun(searchRunId: number) {
  if (window.confirm(`Delete run #${searchRunId}? This cannot be undone.`)) {
    deleteRun(searchRunId)
  }
}

function confirmClearHistory() {
  if (window.confirm('Delete ALL search history? This cannot be undone.')) {
    clearHistory()
  }
}
</script>

<template>
  <section v-if="history.length > 0" class="history-panel">
    <div class="header">
      <h2>History</h2>
      <div class="header-actions">
        <button v-if="selectedRunId !== null" type="button" @click="viewLatest">View latest</button>
        <button type="button" class="clear-all" @click="confirmClearHistory">Clear all</button>
      </div>
    </div>
    <ul>
      <li v-for="item in history" :key="item.searchRunId" class="history-row">
        <button
          type="button"
          class="history-item"
          :class="{ active: item.searchRunId === selectedRunId }"
          @click="selectRun(item.searchRunId)"
        >
          Run #{{ item.searchRunId }} — {{ new Date(item.startedAtUtc).toLocaleString() }} —
          {{ item.totalFound }} found across {{ item.locationsSearched }} locations
        </button>
        <button
          type="button"
          class="delete-btn"
          :aria-label="`Delete run ${item.searchRunId}`"
          @click="confirmDeleteRun(item.searchRunId)"
        >
          ×
        </button>
      </li>
    </ul>
  </section>
</template>

<style scoped>
.history-panel {
  margin: 1.5rem 0;
}

.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.header h2 {
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 0.5rem;
}

.clear-all {
  color: #b91c1c;
}

ul {
  list-style: none;
  padding: 0;
  margin: 0.5rem 0 0;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.history-row {
  display: flex;
  gap: 0.4rem;
}

.history-item {
  flex: 1;
  text-align: left;
  background: transparent;
  border: 1px solid var(--border);
  border-radius: 6px;
  padding: 0.4rem 0.6rem;
  cursor: pointer;
  font-size: 0.85rem;
  color: var(--text);
}

.history-item.active {
  border-color: #1e3a8a;
  background: #eef2ff;
  color: #1e3a8a;
}

.delete-btn {
  background: transparent;
  border: 1px solid var(--border);
  border-radius: 6px;
  width: 2rem;
  cursor: pointer;
  color: #b91c1c;
  font-size: 1rem;
  line-height: 1;
}
</style>
