// Mirrors InfoTrack.Solicitors.Core.Models (System.Text.Json serializes with camelCase by default).

export interface SolicitorListingView {
  name: string
  locationName: string
  address: string
  phone: string | null
  rating: number
  reviewCount: number
  isAccredited: boolean
  websiteUrl: string | null
  isNewSinceLastRun: boolean
}

export interface NationalSummary {
  totalLocationsSearched: number
  totalSolicitorsFound: number
  averageRating: number
  accreditedCount: number
  accreditedPercentage: number
  topRated: SolicitorListingView[]
}

export interface LocationReport {
  locationName: string
  totalFound: number
  averageRating: number
  accreditedCount: number
  topRated: SolicitorListingView[]
  newSinceLastRun: SolicitorListingView[]
  allListings: SolicitorListingView[]
}

export interface SearchReport {
  searchRunId: number
  startedAtUtc: string
  completedAtUtc: string
  national: NationalSummary
  locations: LocationReport[]
}

export interface LocationSearchSummary {
  locationName: string
  resultCount: number
  newSinceLastRunCount: number
}

export interface SearchRunSummary {
  searchRunId: number
  startedAtUtc: string
  completedAtUtc: string
  locations: LocationSearchSummary[]
}

export interface SearchRunHistoryItem {
  searchRunId: number
  startedAtUtc: string
  completedAtUtc: string | null
  locationsSearched: number
  totalFound: number
}
