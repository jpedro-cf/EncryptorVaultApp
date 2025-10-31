import { useMfaQrCode, useSetupMfa } from '@/api/account/auth'
import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import z from 'zod'
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from '../ui/form'
import { Spinner } from '../ui/spinner'
import { Button } from '../ui/button'
import { Input } from '../ui/input'

const totpSchema = z.object({
    code: z.string().min(6).max(6),
})

export type TotpSchema = z.infer<typeof totpSchema>

interface Props {
    onSuccess: () => void
}
export function MfaForm({ onSuccess }: Props) {
    const { data, isSuccess } = useMfaQrCode()
    const { mutate, isPending } = useSetupMfa()

    const form = useForm<TotpSchema>({
        resolver: zodResolver(totpSchema),
    })

    function handleSubmit(data: TotpSchema) {
        mutate(data, { onSuccess: () => onSuccess() })
    }

    return (
        <Form {...form}>
            <form
                onSubmit={form.handleSubmit(handleSubmit)}
                className="space-y-4 mt-5"
            >
                <div className="space-y-4">
                    <div className="p-4 bg-slate-700/50 rounded-lg border border-slate-600 space-y-3 text-slate-300">
                        <p className="text-sm">
                            Scan this QR code with your Authenticator App and
                            verify the code in the input below:
                        </p>
                        <div className="bg-white p-4 rounded-lg flex items-center justify-center">
                            <div className="w-32 h-32 bg-slate-200 rounded flex items-center justify-center text-xs text-slate-600 overflow-hidden">
                                {!isSuccess ? (
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
                        {isSuccess && (
                            <p className="text-sm">
                                Or use this key inside the authenticator app:{' '}
                                <span className="tracking-widest font-mono text-xs text-slate-400">
                                    {data?.key}
                                </span>
                            </p>
                        )}
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
                                variant={'secondary'}
                                disabled={isPending}
                            >
                                Confirm
                            </Button>
                        </div>
                    </div>
                </div>
            </form>
        </Form>
    )
}
