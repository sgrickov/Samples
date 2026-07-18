import { ref, watch } from 'vue'
import { useAppStore } from '../stores/app'

const DEFAULT_LOCATIONS = ['London', 'Birmingham', 'Leeds', 'Manchester', 'Sheffield', 'Bradford', 'Liverpool', 'Bristol']

/**
 * Local editable draft of the location list, synced from the store's saved list and
 * only pushed back via `save()` — lets the user add/remove several before persisting.
 */
export function useLocationsDraft() {
  const store = useAppStore()
  const draft = ref<string[]>([])
  const newLocation = ref('')
  const isSaving = ref(false)

  watch(
    () => store.locations,
    (locations) => {
      draft.value = [...locations]
    },
    { immediate: true },
  )

  function addLocation() {
    const name = newLocation.value.trim()
    if (name.length === 0) {
      return
    }
    if (!draft.value.some((existing) => existing.toLowerCase() === name.toLowerCase())) {
      draft.value.push(name)
    }
    newLocation.value = ''
  }

  function removeLocation(name: string) {
    draft.value = draft.value.filter((existing) => existing !== name)
  }

  function resetToDefaults() {
    draft.value = [...DEFAULT_LOCATIONS]
  }

  async function save() {
    isSaving.value = true
    try {
      await store.saveLocations(draft.value)
    } finally {
      isSaving.value = false
    }
  }

  return { draft, newLocation, isSaving, addLocation, removeLocation, resetToDefaults, save }
}
