import { storeToRefs } from 'pinia'
import { useAppStore } from '../stores/app'

/** Triggers a search run and exposes its in-flight/error state, without components touching the store directly. */
export function useSearchRun() {
  const store = useAppStore()
  const { isSearching, error } = storeToRefs(store)

  function run() {
    return store.runSearchNow()
  }

  return { isSearching, error, run }
}
