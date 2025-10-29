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
import { useAccountDeletion, useAccountMutation } from '@/api/account/users'
import { ConfirmationDialog } from '../ui/confirmation-dialog'

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

    const { mutate: updateAccount, isPending: isUpdating } =
        useAccountMutation()
    const { mutate: deleteAccount, isPending: isDeleting } =
        useAccountDeletion()

    const form = useForm<ProfileFormSchema>({
        resolver: zodResolver(profileFormSchema),
        defaultValues: {
            id: account?.id,
        },
    })

    function handleSubmit(data: ProfileFormSchema) {
        updateAccount(data, { onSuccess: () => form.reset() })
    }

    function onDeletionConfirmation() {
        deleteAccount()
    }

    const isPending = isUpdating || isDeleting
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
                <div className="flex gap-3 col-span-full">
                    <ConfirmationDialog onConfirm={onDeletionConfirmation}>
                        <Button
                            variant={'destructive'}
                            className="bg-red-800"
                            disabled={isPending}
                        >
                            Delete Account
                        </Button>
                    </ConfirmationDialog>
                    <Button
                        variant={'primary'}
                        className="flex-1"
                        disabled={isPending}
                    >
                        Submit
                    </Button>
                </div>
            </form>
        </Form>
    )
}
