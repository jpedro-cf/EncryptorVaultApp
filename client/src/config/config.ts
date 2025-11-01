const mb256 = 256 * 1024 * 1024
export const config = {
    TOTAL_STORAGE: mb256,
    APP_URL: import.meta.env.VITE_CLIENT_URL ?? 'http://localhost:5173',
    API_URL: import.meta.env.VITE_SERVER_URL ?? 'http://localhost:5190/api',
}
