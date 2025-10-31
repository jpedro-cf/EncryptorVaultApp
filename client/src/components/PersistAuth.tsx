import { useCurrentUser } from '@/api/account/users'
import { LoadingPage } from '@/pages/LoadingPage'
import { useEffect, useState } from 'react'
import { Outlet } from 'react-router'

export function PersistAuth() {
    const [loading, setLoading] = useState(true)
    const { data, isFetched } = useCurrentUser()

    useEffect(() => {
        if (isFetched) {
            setLoading(false)
        }
    }, [data, isFetched])

    return loading ? <LoadingPage /> : <Outlet />
}
