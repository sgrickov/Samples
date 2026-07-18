import { computed, ref } from 'vue'

export type SortDirection = 'asc' | 'desc'

/**
 * Generic click-to-sort helper: given a source list, a default column, and an accessor per
 * sortable column key, returns the sorted list plus the state/toggle needed to drive clickable
 * table headers. Reusable across any list-of-objects table, not just one component.
 */
export function useSortableList<T>(
  items: () => T[],
  defaultKey: string,
  accessors: Record<string, (item: T) => string | number>,
) {
  const sortKey = ref(defaultKey)
  const sortDirection = ref<SortDirection>('asc')

  function toggleSort(key: string) {
    if (sortKey.value === key) {
      sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
    } else {
      sortKey.value = key
      sortDirection.value = 'asc'
    }
  }

  const sorted = computed(() => {
    const accessor = accessors[sortKey.value]
    const list = [...items()]
    if (!accessor) {
      return list
    }

    const sign = sortDirection.value === 'asc' ? 1 : -1
    return list.sort((a, b) => {
      const aValue = accessor(a)
      const bValue = accessor(b)
      if (aValue < bValue) return -1 * sign
      if (aValue > bValue) return 1 * sign
      return 0
    })
  })

  return { sortKey, sortDirection, toggleSort, sorted }
}
