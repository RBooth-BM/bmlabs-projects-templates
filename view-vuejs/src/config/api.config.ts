export const apiConfig = {
  baseURL: import.meta.env.VITE_API_URL || window.location.origin,
  timeout: 15000,
  withCredentials: false,
  headers: {
    'Content-Type': 'application/json',
  },
}
