import { RegistrationStep } from '@/pages/RegisterPage'
import type { ReactElement } from 'react'
import { z } from 'zod'
import { useRegistrationContext } from './RegistrationContext'
import { zodResolver } from '@hookform/resolvers/zod'
import { motion } from 'motion/react'
import { CardDescription, CardTitle } from '../ui/card'
import { useForm } from 'react-hook-form'
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
import { AlertCircle, CheckCircle2 } from 'lucide-react'
import { Alert, AlertDescription } from '../ui/alert'
import { Encryption } from '@/lib/encryption'
import {
    useLogin,
    useMfaQrCode,
    useRegister,
    useSetupMfa,
} from '@/api/account/auth'
import { useUpdateVaultSecret } from '@/api/account/users'
import { Spinner } from '../ui/spinner'
import { useAuth } from '@/hooks/use-auth'
import { Encoding } from '@/lib/encoding'
import { useKeys } from '@/hooks/use-keys'

const createAccountSchema = z
    .object({
        email: z.email(),
        password: z
            .string()
            .min(8, 'Password must be at least 8 characters')
            .regex(/[0-9]/, 'Password must contain at least one number')
            .regex(
                /[a-z]/,
                'Password must contain at least one lowercase letter'
            )
            .regex(
                /[A-Z]/,
                'Password must contain at least one uppercase letter'
            )
            .regex(
                /[^a-zA-Z0-9]/,
                'Password must contain at least one special character'
            ),
        passwordConfirmation: z.string(),
    })
    .refine((data) => data.password === data.passwordConfirmation, {
        message: "Passwords don't match",
        path: ['passwordConfirmation'],
    })

const vaultSecretSchema = z.object({
    secret: z.string().min(12, 'Vault secret must be at least 12 characters'),
})

const totpSchema = z.object({
    code: z.string().min(6).max(6),
})

export type CreateAccountSchema = z.infer<typeof createAccountSchema>
export type VaultSecretSchema = z.infer<typeof vaultSecretSchema>
export type TotpSchema = z.infer<typeof totpSchema>

export function RegisterForm() {
    const { currentStep } = useRegistrationContext()

    const forms: Record<RegistrationStep, ReactElement> = {
        [RegistrationStep.ACCOUNT]: <AccountForm />,
        [RegistrationStep.VAULT_SECRET]: <VaultSecretForm />,
        [RegistrationStep.MFA]: <SetupMfaForm />,
    }

    return forms[currentStep]
}

export function AccountForm() {
    const { setCurrentStep } = useRegistrationContext()
    const { setAccount } = useAuth()
    const {
        mutate: register,
        isPending: isRegistering,
        isError: registrationFailed,
    } = useRegister()

    const {
        mutate: login,
        isPending: isLoggingIn,
        isError: loginFailed,
    } = useLogin()

    const form = useForm<CreateAccountSchema>({
        resolver: zodResolver(createAccountSchema),
    })

    function handleSubmit(data: CreateAccountSchema) {
        register(data, {
            onSuccess: () => {
                login(
                    { email: data.email, password: data.password },
                    {
                        onSuccess: (data) => {
                            setAccount(data)
                            setCurrentStep(RegistrationStep.VAULT_SECRET)
                        },
                    }
                )
            },
        })
    }

    return (
        <motion.div
            initial={{ opacity: 0 }}
            animate={{ x: 0, opacity: 1 }}
            transition={{ duration: 0.3, ease: 'easeInOut' }}
        >
            <CardTitle className="text-white">Account Information</CardTitle>
            <CardDescription className="text-slate-400">
                Create your account with basic information
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
                    <FormField
                        control={form.control}
                        name="passwordConfirmation"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel className="text-slate-200">
                                    Confirm Password
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
                        disabled={isRegistering || isLoggingIn}
                    >
                        Continue
                    </Button>
                </form>
            </Form>
            <div className="mt-6 text-center text-sm text-slate-400">
                Already have an account? &nbsp;
                <Link
                    to={'/login'}
                    className="text-blue-400 hover:text-blue-300"
                >
                    Sign in
                </Link>
            </div>
        </motion.div>
    )
}

export function VaultSecretForm() {
    const { setRootKey } = useKeys()
    const { setCurrentStep } = useRegistrationContext()
    const { mutate, isPending, isError } = useUpdateVaultSecret()

    const form = useForm<VaultSecretSchema>({
        resolver: zodResolver(vaultSecretSchema),
        defaultValues: {
            secret: Encoding.uint8ArrayToBase64(
                Encryption.generateRandomSecret(10)
            ),
        },
    })

    function handleSubmit(data: VaultSecretSchema) {
        mutate(data, {
            onSuccess: () => {
                setRootKey(Encoding.textToUint8Array(data.secret))
                setCurrentStep(RegistrationStep.MFA)
            },
        })
    }
    return (
        <motion.div
            initial={{ x: '50%', opacity: 0 }}
            animate={{ x: 0, opacity: 1 }}
            transition={{ duration: 0.3, ease: 'easeInOut' }}
        >
            <CardTitle className="text-white">Setup vault secret</CardTitle>
            <CardDescription className="text-slate-400">
                Configure your app secret key for encryption
            </CardDescription>
            <Form {...form}>
                <form
                    onSubmit={form.handleSubmit(handleSubmit)}
                    className="space-y-4 mt-5"
                >
                    <div className="p-4 bg-slate-700/50 rounded-lg border border-slate-600">
                        <p className="text-sm text-slate-300 mb-3">
                            Your secret key is used to encrypt your data. Store
                            it safely.
                        </p>
                        <FormField
                            control={form.control}
                            name="secret"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel className="text-slate-200">
                                        Secret Key
                                    </FormLabel>
                                    <FormControl>
                                        <Input {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                    </div>
                    <Alert className="border-blue-500/50 bg-blue-500/10 flex items-start gap-2">
                        <div className="text-blue-400">
                            <AlertCircle className="h-4 w-4" />
                        </div>
                        <AlertDescription className="text-blue-300 text-sm">
                            Save this key in a secure location. You'll need it
                            to decrypt your vault.
                        </AlertDescription>
                    </Alert>
                    <Button
                        type="submit"
                        variant={'primary'}
                        className="w-full"
                        disabled={isPending}
                    >
                        Continue
                    </Button>
                </form>
            </Form>
        </motion.div>
    )
}

export function SetupMfaForm() {
    const navigate = useNavigate()

    const { data, isLoading } = useMfaQrCode()
    const { mutate, isPending, isError } = useSetupMfa()

    const form = useForm<TotpSchema>({
        resolver: zodResolver(totpSchema),
    })

    function handleSubmit(data: TotpSchema) {
        mutate(data, { onSuccess: () => navigate('/', { replace: true }) })
    }

    return (
        <motion.div
            initial={{ x: '50%', opacity: 0 }}
            animate={{ x: 0, opacity: 1 }}
            transition={{ duration: 0.3, ease: 'easeInOut' }}
        >
            <CardTitle className="text-white">
                Security settings (OPTIONAL)
            </CardTitle>
            <CardDescription className="text-slate-400">
                Enable optional two-factor authentication.
            </CardDescription>
            <Form {...form}>
                <form
                    onSubmit={form.handleSubmit(handleSubmit)}
                    className="space-y-4 mt-5"
                >
                    <div className="space-y-4">
                        <div className="p-4 bg-slate-700/50 rounded-lg border border-slate-600 space-y-3">
                            <p className="text-sm text-slate-300">
                                Scan this QR code with your authenticator app
                                and verify the code in the input below:
                            </p>
                            <div className="bg-white p-4 rounded-lg flex items-center justify-center">
                                <div className="w-32 h-32 bg-slate-200 rounded flex items-center justify-center text-xs text-slate-600 overflow-hidden">
                                    {isLoading ? (
                                        <Spinner size={24} color="#000" />
                                    ) : (
                                        <img
                                            src={data?.qrCodeBase64}
                                            alt="QR Code"
                                            className="w-full h-full object-cover"
                                        />
                                    )}
                                </div>
                            </div>
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
                                <Button type="submit" variant={'secondary'}>
                                    Confirm
                                </Button>
                            </div>
                        </div>
                    </div>

                    <Alert className="border-green-500/50 bg-green-500/10 flex items-start gap-2">
                        <div className="text-green-400">
                            <CheckCircle2 className="h-4 w-4" />
                        </div>
                        <AlertDescription className="text-green-300 text-sm">
                            Your account is ready to be created! You can choose
                            to skip this setup and set it up later.
                        </AlertDescription>
                    </Alert>

                    <Button
                        type="button"
                        variant={'primary'}
                        className="w-full"
                        onClick={() => navigate('/', { replace: true })}
                    >
                        Skip
                    </Button>
                </form>
            </Form>
        </motion.div>
    )
}
