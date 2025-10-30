import { Toaster } from '@/components/ui/sonner'
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import App from './App.tsx'
import './index.css'
import { MediaQueryProvider } from './hooks/use-media-query.tsx'

const queryClient = new QueryClient()

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <QueryClientProvider client={queryClient}>
            <MediaQueryProvider>
                <App />
                <Toaster />
            </MediaQueryProvider>
        </QueryClientProvider>
    </StrictMode>
)
