import { ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useAppStore } from '../stores/app'

/** Lets the user browse past search runs, switch the displayed report between them, and delete runs. */
export function useHistorySelection() {
  const store = useAppStore()
  const { history } = storeToRefs(store)
  const selectedRunId = ref<number | null>(null)

  async function selectRun(searchRunId: number) {
    selectedRunId.value = searchRunId
    await store.loadReportByRunId(searchRunId)
  }

  async function viewLatest() {
    selectedRunId.value = null
    await store.loadLatestReport()
  }

  async function deleteRun(searchRunId: number) {
    await store.deleteRun(searchRunId)
    if (selectedRunId.value === searchRunId) {
      selectedRunId.value = null
    }
  }

  async function clearHistory() {
    await store.clearHistory()
    selectedRunId.value = null
  }

  return { history, selectedRunId, selectRun, viewLatest, deleteRun, clearHistory }
}
