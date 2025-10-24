import type {
    CreateAccountSchema,
    TotpSchema,
} from '@/components/register/RegisterForm'
import { api } from '../axios'
import type { User } from '@/types/account'
import type { AxiosError } from 'axios'
import { useMutation, useQuery } from '@tanstack/react-query'
import { toast } from 'sonner'
import type { LoginSchema, TwoFactorSchema } from '@/components/login/LoginForm'

export function useRegister() {
    async function request(data: CreateAccountSchema): Promise<void> {
        await api.post('/auth/register', data)
    }

    return useMutation({
        mutationFn: request,
        onError: (e: AxiosError<{ detail?: string }>) =>
            toast.warning(
                e.response?.data.detail ??
                    'An error occured while creating your account.'
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
                        'An error occured while creating your account.'
                )
            }
        },
    })
}

export function useLoginMfa() {
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
                    'An error occured while creating your account.'
            )
        },
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
                    'An error occured while creating your account.'
            ),
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
