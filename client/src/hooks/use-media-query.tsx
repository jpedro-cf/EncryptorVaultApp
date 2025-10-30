import {
    createContext,
    useContext,
    useEffect,
    useState,
    type PropsWithChildren,
} from 'react'

interface MediaQueryContext {
    isMobile: boolean
}
const MediaQueryContext = createContext<MediaQueryContext | null>(null)

export function useMediaQuery() {
    const context = useContext(MediaQueryContext)
    if (!context) {
        throw new Error('MediaQueryContext must be used within a provider')
    }
    return context
}
export function MediaQueryProvider({ children }: PropsWithChildren) {
    const [width, setWidth] = useState<number>(window.innerWidth)

    function handleWindowSizeChange() {
        setWidth(window.innerWidth)
    }
    useEffect(() => {
        window.addEventListener('resize', handleWindowSizeChange)
        return () => {
            window.removeEventListener('resize', handleWindowSizeChange)
        }
    }, [])

    const isMobile = width <= 768

    return (
        <MediaQueryContext.Provider value={{ isMobile }}>
            {children}
        </MediaQueryContext.Provider>
    )
}
