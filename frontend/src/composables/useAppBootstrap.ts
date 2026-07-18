import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useAppStore } from '../stores/app'

/** Loads locations, the latest report, and run history as soon as the app mounts. */
export function useAppBootstrap() {
  const store = useAppStore()
  const { report, isLoadingReport } = storeToRefs(store)

  onMounted(() => {
    store.initialize()
  })

  return { report, isLoadingReport }
}
