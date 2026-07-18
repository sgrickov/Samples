import { defineStore } from 'pinia'
import {
  ApiError,
  deleteAllSearchRuns,
  deleteSearchRun,
  getHistory,
  getLatestReport,
  getLocations,
  getReport,
  runSearch,
  updateLocations,
} from '../api/client'
import type { SearchReport, SearchRunHistoryItem } from '../types/api'

function toErrorMessage(error: unknown): string {
  if (error instanceof ApiError || error instanceof Error) {
    return error.message
  }
  return 'Something went wrong'
}

export const useAppStore = defineStore('app', {
  state: () => ({
    locations: [] as string[],
    report: null as SearchReport | null,
    history: [] as SearchRunHistoryItem[],
    isLoadingLocations: false,
    isSearching: false,
    isLoadingReport: false,
    error: null as string | null,
  }),
  actions: {
    async initialize() {
      await Promise.all([this.loadLocations(), this.loadLatestReport(), this.loadHistory()])
    },

    async loadLocations() {
      this.isLoadingLocations = true
      try {
        this.locations = await getLocations()
      } catch (error) {
        this.error = toErrorMessage(error)
      } finally {
        this.isLoadingLocations = false
      }
    },

    async saveLocations(locations: string[]) {
      this.error = null
      this.locations = await updateLocations(locations).catch((error) => {
        this.error = toErrorMessage(error)
        throw error
      })
    },

    async runSearchNow() {
      this.isSearching = true
      this.error = null
      try {
        await runSearch()
        await Promise.all([this.loadLatestReport(), this.loadHistory()])
      } catch (error) {
        this.error = toErrorMessage(error)
      } finally {
        this.isSearching = false
      }
    },

    async loadLatestReport() {
      this.isLoadingReport = true
      try {
        this.report = await getLatestReport()
      } catch (error) {
        this.error = toErrorMessage(error)
      } finally {
        this.isLoadingReport = false
      }
    },

    async loadReportByRunId(searchRunId: number) {
      this.isLoadingReport = true
      try {
        this.report = await getReport(searchRunId)
      } catch (error) {
        this.error = toErrorMessage(error)
      } finally {
        this.isLoadingReport = false
      }
    },

    async loadHistory() {
      try {
        this.history = await getHistory()
      } catch (error) {
        this.error = toErrorMessage(error)
      }
    },

    /** Deletes one run and refreshes history; falls back to the latest report if the deleted run was on screen. */
    async deleteRun(searchRunId: number) {
      this.error = null
      try {
        await deleteSearchRun(searchRunId)
        await this.loadHistory()
        if (this.report?.searchRunId === searchRunId) {
          await this.loadLatestReport()
        }
      } catch (error) {
        this.error = toErrorMessage(error)
      }
    },

    async clearHistory() {
      this.error = null
      try {
        await deleteAllSearchRuns()
        this.history = []
        this.report = null
      } catch (error) {
        this.error = toErrorMessage(error)
      }
    },
  },
})
