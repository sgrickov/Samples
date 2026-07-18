import type { SearchReport, SearchRunHistoryItem, SearchRunSummary } from '../types/api'

const BASE_URL = import.meta.env.VITE_API_BASE_URL as string

export class ApiError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, {
    ...init,
    headers: { 'Content-Type': 'application/json', ...init?.headers },
  })

  if (!response.ok) {
    const body = await response.text().catch(() => '')
    throw new ApiError(response.status, body || `Request to ${path} failed with ${response.status}`)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}

/** As `request`, but a 404 resolves to `null` instead of throwing — used where "not found yet" is a normal state. */
async function requestOrNull<T>(path: string): Promise<T | null> {
  const response = await fetch(`${BASE_URL}${path}`)
  if (response.status === 404) {
    return null
  }
  if (!response.ok) {
    throw new ApiError(response.status, `Request to ${path} failed with ${response.status}`)
  }
  return (await response.json()) as T
}

export function getLocations(): Promise<string[]> {
  return request<string[]>('/api/locations')
}

export function updateLocations(locations: string[]): Promise<string[]> {
  return request<string[]>('/api/locations', {
    method: 'PUT',
    body: JSON.stringify({ locations }),
  })
}

export function runSearch(locations?: string[]): Promise<SearchRunSummary> {
  return request<SearchRunSummary>('/api/search', {
    method: 'POST',
    body: JSON.stringify({ locations: locations ?? null }),
  })
}

export function getLatestReport(): Promise<SearchReport | null> {
  return requestOrNull<SearchReport>('/api/reports/latest')
}

export function getReport(searchRunId: number): Promise<SearchReport | null> {
  return requestOrNull<SearchReport>(`/api/reports/${searchRunId}`)
}

export function getHistory(): Promise<SearchRunHistoryItem[]> {
  return request<SearchRunHistoryItem[]>('/api/reports/history')
}

export function deleteSearchRun(searchRunId: number): Promise<void> {
  return request<void>(`/api/reports/${searchRunId}`, { method: 'DELETE' })
}

export function deleteAllSearchRuns(): Promise<void> {
  return request<void>('/api/reports', { method: 'DELETE' })
}
