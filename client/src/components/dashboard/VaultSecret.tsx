import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import z from 'zod'
import { Card, CardDescription, CardTitle } from '../ui/card'
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from '../ui/form'
import { Alert, AlertDescription } from '../ui/alert'
import { Button } from '../ui/button'
import { AlertCircle } from 'lucide-react'
import { Input } from '../ui/input'
import { useKeys } from '@/hooks/use-keys'
import { useAuth } from '@/hooks/use-auth'
import { Encryption } from '@/lib/encryption'
import { useEffect, useState } from 'react'
import { ItemCardSkeleton } from '../items/ItemCardSkeleton'

const vaultSecretSchema = z.object({
    secret: z.string().min(1),
})

export type VaultSecretSchema = z.infer<typeof vaultSecretSchema>

export function VaultSecret() {
    const [pending, setPending] = useState(false)

    const { rootKey, setRootKey } = useKeys()
    const { account } = useAuth()

    const form = useForm<VaultSecretSchema>({
        resolver: zodResolver(vaultSecretSchema),
    })

    async function handleSubmit(data: VaultSecretSchema) {
        const { secret } = data
        setPending(true)

        const decrypted = await Encryption.decryptVaultKey({
            base64Key: account!.vaultKey,
            secret,
        })

        if (decrypted == null) {
            form.setError('secret', { message: 'Incorrect secret.' })
        } else {
            setRootKey(decrypted)
        }

        setPending(false)
    }

    if (rootKey != null) {
        return <></>
    }

    return (
        <div className="relative w-full">
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 blur-xs opacity-35">
                {Array.from({ length: 12 }).map((_, i) => (
                    <ItemCardSkeleton key={i} />
                ))}
            </div>
            <Card className="bg-slate-800/50 border-slate-600 p-6 gap-1 w-full md:w-2/3 absolute left-1/2 -translate-x-1/2 top-0 ">
                <CardTitle className="text-white">
                    Enter your vault secret
                </CardTitle>
                <CardDescription className="text-slate-400">
                    Enter your vault secret to unlock (decrypt) your vault.
                </CardDescription>
                <Form {...form}>
                    <form
                        onSubmit={form.handleSubmit(handleSubmit)}
                        className="space-y-4 mt-5"
                    >
                        <div className="p-4 bg-slate-700/50 rounded-lg border border-slate-600">
                            <FormField
                                control={form.control}
                                name="secret"
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel className="text-slate-200">
                                            Secret Key:
                                        </FormLabel>
                                        <FormControl>
                                            <Input {...field} />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <Alert className="border-blue-500/50 bg-blue-500/10 flex items-start gap-2 mt-5">
                                <div className="text-blue-400">
                                    <AlertCircle className="h-4 w-4" />
                                </div>
                                <AlertDescription className="text-blue-300 text-sm">
                                    We don't store your keys anywhere, only YOU
                                    know them. Therefore, you must enter your
                                    secret every time you access our app again.
                                    We also don't store them in places like
                                    cookies, local storage, or local DB in the
                                    browser due to vulnerabilities.
                                </AlertDescription>
                            </Alert>
                        </div>

                        <Button
                            type="submit"
                            variant={'primary'}
                            className="w-full"
                            disabled={pending}
                        >
                            Verify
                        </Button>
                    </form>
                </Form>
            </Card>
        </div>
    )
}
