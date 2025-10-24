import { useCurrentUser } from '@/api/account/users'
import { useAuth } from '@/hooks/use-auth'
import { LoadingPage } from '@/pages/LoadingPage'
import { Encryption } from '@/services/encryption'
import { useEffect } from 'react'
import { Outlet } from 'react-router'

export function PersistAuth() {
    const { account, setAccount } = useAuth()
    const { data, isLoading, isSuccess } = useCurrentUser()

    useEffect(() => {
        if (isSuccess) {
            setAccount(data)
        }
    }, [isSuccess])

    if (isLoading) {
        return <LoadingPage />
    }

    console.log(account)

    return <Outlet />
}
