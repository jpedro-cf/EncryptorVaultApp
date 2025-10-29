import { useCurrentUser } from '@/api/account/users'
import { useAuth } from '@/hooks/use-auth'
import { LoadingPage } from '@/pages/LoadingPage'
import { useEffect, useState } from 'react'
import { Outlet } from 'react-router'

export function PersistAuth() {
    const [loading, setLoading] = useState(true)
    const { setAccount, setStorageUsage } = useAuth()
    const { data, isSuccess, isFetched } = useCurrentUser()

    useEffect(() => {
        if (!isFetched) {
            return
        }
        if (isSuccess) {
            setAccount(data.user)
            setStorageUsage(data.storageUsage)
        }
        setLoading(false)
    }, [data, isFetched])

    return loading ? <LoadingPage /> : <Outlet />
}
