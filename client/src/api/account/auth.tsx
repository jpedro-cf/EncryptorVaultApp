import type { CreateAccountSchema } from '@/components/register/RegisterForm'
import { api } from '../axios'
import type { User } from '@/types/account'
import type { AxiosError } from 'axios'
import { useMutation, useQuery } from '@tanstack/react-query'
import { toast } from 'sonner'
import type { LoginSchema, TwoFactorSchema } from '@/components/login/LoginForm'
import { useAuth } from '@/hooks/use-auth'
import { useKeys } from '@/hooks/use-keys'
import type { TotpSchema } from '@/components/account/MfaForm'

export function useRegister() {
    async function request(data: CreateAccountSchema): Promise<void> {
        await api.post('/auth/register', data)
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string }>) =>
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while performing this operation.'
            ),
    })
}

export function useLogin() {
    async function request(data: LoginSchema): Promise<User> {
        const res = await api.post('/auth/login', data)
        const { user } = res.data
        return user
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string; title?: string }>) => {
            if (e.response?.data.title != 'TwoFactorAuthenticationRequired') {
                toast.warning(
                    e.response?.data.detail ??
                        'An error occured while performing this operation.'
                )
            }
        },
    })
}

export function useLoginMfa() {
    const { setAccount } = useAuth()
    async function request(data: TwoFactorSchema): Promise<User> {
        const res = await api.post('/auth/mfa/login', data)
        const { user } = res.data
        return user
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string }>) => {
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while performing this operation.'
            )
        },
        onSuccess: (data) => setAccount(data),
    })
}

export function useSetupMfa() {
    async function request(data: TotpSchema): Promise<void> {
        await api.post('/auth/mfa', data)
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string }>) =>
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while performing this operation.'
            ),
        onSuccess: () => {
            toast.success('Success!')
        },
    })
}

export function useLogout() {
    const { setAccount } = useAuth()
    const { setRootKey } = useKeys()
    async function request(): Promise<void> {
        await api.post('/auth/logout')
        setRootKey(null)
    }

    return useMutation({
        mutationFn: request,
        onSuccess: () => setAccount(null),
        onError: (e: AxiosError<{ detail?: string }>) => {
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while performing this operation.'
            )
        },
    })
}

interface MfaQrCodeResponse {
    key: string
    qrCodeBase64: string
}
export function useMfaQrCode() {
    async function request(): Promise<MfaQrCodeResponse> {
        const res = await api.get('/auth/mfa')
        return res.data
    }

    return useQuery({
        queryFn: request,
        queryKey: ['qr_code'],
        retry: 2,
        refetchOnWindowFocus: false,
    })
}
