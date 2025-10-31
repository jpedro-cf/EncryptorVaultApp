const mb256 = 256 * 1024 * 1024
export const config = {
    TOTAL_STORAGE: mb256,
    APP_URL: import.meta.env.VITE_APP_URL ?? 'http://localhost:5173',
}
