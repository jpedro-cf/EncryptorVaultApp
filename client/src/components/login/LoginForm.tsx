import { LoginStep } from '@/pages/LoginPage'
import { useEffect, type ReactElement } from 'react'
import { useLoginContext } from './LoginContext'
import { motion } from 'motion/react'
import { CardDescription, CardTitle } from '../ui/card'
import z from 'zod'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from '../ui/form'
import { Input } from '../ui/input'
import { Button } from '../ui/button'
import { Link, useNavigate } from 'react-router'
import { useLogin, useLoginMfa } from '@/api/account/auth'
import { useAuth } from '@/hooks/use-auth'
import { useKeys } from '@/hooks/use-keys'
import { useQueryClient } from '@tanstack/react-query'

const loginSchema = z.object({
    email: z.email(),
    password: z.string(),
})

const twoFactorSchema = z.object({
    code: z.string().min(6).max(6),
})

export type LoginSchema = z.infer<typeof loginSchema>
export type TwoFactorSchema = z.infer<typeof twoFactorSchema>

export function LoginForm() {
    const { currentStep } = useLoginContext()

    const forms: Record<LoginStep, ReactElement> = {
        [LoginStep.DEFAULT]: <DefaultForm />,
        [LoginStep.TWO_FACTOR]: <TwoFactorForm />,
    }

    return forms[currentStep]
}

function DefaultForm() {
    const navigate = useNavigate()
    const queryClient = useQueryClient()

    const { setAccount } = useAuth()
    const { clear: clearKeys } = useKeys()
    const { setCurrentStep } = useLoginContext()

    const { mutate: login, isPending, isError } = useLogin()

    const form = useForm<LoginSchema>({
        resolver: zodResolver(loginSchema),
    })

    function handleSubmit(data: LoginSchema) {
        login(data, {
            onError: (e) => {
                if (
                    e.response?.data.title == 'TwoFactorAuthenticationRequired'
                ) {
                    setCurrentStep(LoginStep.TWO_FACTOR)
                }
            },
            onSuccess: (data) => {
                clearKeys()
                setAccount(data)
                queryClient.clear()
                navigate('/', { replace: true })
            },
        })
    }

    return (
        <motion.div
            className="mt-3"
            initial={{ opacity: 0 }}
            animate={{ x: 0, opacity: 1 }}
            transition={{ duration: 0.3, ease: 'easeInOut' }}
        >
            <CardTitle className="text-white">Login</CardTitle>
            <CardDescription className="text-slate-400">
                Enter your account credentials to continue
            </CardDescription>
            <Form {...form}>
                <form
                    onSubmit={form.handleSubmit(handleSubmit)}
                    className="space-y-4 mt-5"
                >
                    <FormField
                        control={form.control}
                        name="email"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel className="text-slate-200">
                                    Email
                                </FormLabel>
                                <FormControl>
                                    <Input
                                        placeholder="you@example.com"
                                        type="email"
                                        {...field}
                                    />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                    <FormField
                        control={form.control}
                        name="password"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel className="text-slate-200">
                                    Password
                                </FormLabel>
                                <FormControl>
                                    <Input
                                        placeholder="••••••••"
                                        type="password"
                                        {...field}
                                    />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <Button
                        type="submit"
                        variant={'primary'}
                        className="w-full"
                        disabled={isPending}
                    >
                        Submit
                    </Button>
                </form>
            </Form>
            <div className="mt-6 text-center text-sm text-slate-400">
                Don't have an account? &nbsp;
                <Link
                    to={'/register'}
                    className="text-blue-400 hover:text-blue-300"
                >
                    Sign up
                </Link>
            </div>
        </motion.div>
    )
}

function TwoFactorForm() {
    const queryClient = useQueryClient()

    const navigate = useNavigate()

    const { clear: clearKeys } = useKeys()
    const { setAccount } = useAuth()

    const { mutate, isPending } = useLoginMfa()

    const form = useForm<TwoFactorSchema>({
        resolver: zodResolver(twoFactorSchema),
    })

    function handleSubmit(data: TwoFactorSchema) {
        mutate(data, {
            onSuccess: (data) => {
                clearKeys()
                setAccount(data)
                queryClient.clear()
                navigate('/', { replace: true })
            },
        })
    }

    return (
        <motion.div
            className="mt-5"
            initial={{ x: '50%', opacity: 0 }}
            animate={{ x: 0, opacity: 1 }}
            transition={{ duration: 0.3, ease: 'easeInOut' }}
        >
            <CardTitle className="text-white">
                Two Factor Authentication
            </CardTitle>
            <CardDescription className="text-slate-400">
                Open your authenticator app to get your verification code.
            </CardDescription>

            <Form {...form}>
                <form
                    onSubmit={form.handleSubmit(handleSubmit)}
                    className="mt-5"
                >
                    <div className="p-4 bg-slate-700/50 rounded-lg border border-slate-600 space-y-3">
                        <div className="flex items-end gap-3">
                            <FormField
                                control={form.control}
                                name="code"
                                render={({ field }) => (
                                    <FormItem className="w-full">
                                        <FormLabel className="text-slate-200">
                                            Verification Code
                                        </FormLabel>
                                        <FormControl>
                                            <Input
                                                placeholder="000000"
                                                maxLength={6}
                                                className="bg-slate-600 border-slate-500 text-white placeholder:text-slate-400 text-center text-2xl tracking-widest font-mono"
                                                {...field}
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <Button
                                type="submit"
                                variant={'primary'}
                                disabled={isPending}
                            >
                                Verify
                            </Button>
                        </div>
                    </div>
                </form>
            </Form>
        </motion.div>
    )
}
