import z from 'zod'
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from '@/components/ui/form'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useAuth } from '@/hooks/use-auth'
import { Input } from '@/components/ui/input'
import { Button } from '../ui/button'
import { useAccountMutation } from '@/api/account/users'

const profileFormSchema = z.object({
    id: z.string().min(1),
    email: z.email('Insert a valid email.').optional(),
    newPassword: z
        .string()
        .optional()
        .refine(
            (val) =>
                !val ||
                (val.length >= 8 &&
                    /[0-9]/.test(val) &&
                    /[a-z]/.test(val) &&
                    /[A-Z]/.test(val) &&
                    /[^a-zA-Z0-9]/.test(val)),
            {
                message:
                    'Password must have at least 8 characters, one number, one lowercase, one uppercase, and one special character.',
            }
        ),
    currentPassword: z.string('Insert your current password'),
})

export type ProfileFormSchema = z.infer<typeof profileFormSchema>

export function ProfileForm() {
    const { account } = useAuth()

    const { mutate, isPending } = useAccountMutation()

    const form = useForm<ProfileFormSchema>({
        resolver: zodResolver(profileFormSchema),
        defaultValues: {
            id: account?.id,
        },
    })

    function handleSubmit(data: ProfileFormSchema) {
        mutate(data, { onSuccess: () => form.reset() })
    }
    return (
        <Form {...form}>
            <form
                onSubmit={form.handleSubmit(handleSubmit)}
                className="grid grid-cols-2 gap-3"
            >
                <FormField
                    control={form.control}
                    name="email"
                    render={({ field }) => (
                        <FormItem className="col-span-full">
                            <FormLabel className="text-slate-200">
                                Email
                            </FormLabel>
                            <FormControl>
                                <Input
                                    placeholder="you@example.com"
                                    type="email"
                                    defaultValue={account?.email}
                                    {...field}
                                />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="newPassword"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel className="text-slate-200">
                                New Password
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
                    name="currentPassword"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel className="text-slate-200">
                                Current Password
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
                    variant={'primary'}
                    className="col-span-full"
                    disabled={isPending}
                >
                    Submit
                </Button>
            </form>
        </Form>
    )
}
