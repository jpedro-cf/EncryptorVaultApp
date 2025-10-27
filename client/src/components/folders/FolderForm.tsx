import { useForm } from 'react-hook-form'
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from '../ui/form'
import z from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { Input } from '../ui/input'
import { Button } from '../ui/button'
import { CheckCircle } from 'lucide-react'
import { useFolderMutation } from '@/api/folders/folders'
import { toast } from 'sonner'

const folderFormSchema = z.object({
    id: z.string().optional(),
    name: z.string().min(1),
    parentId: z.string().min(1).optional(),
})

export type FolderFormSchema = z.infer<typeof folderFormSchema>

interface Props {
    parentId?: string
    id?: string
    onComplete: () => void
}
export function FolderForm({ parentId, id, onComplete }: Props) {
    const form = useForm<FolderFormSchema>({
        resolver: zodResolver(folderFormSchema),
        defaultValues: {
            id,
            parentId,
        },
    })

    const { mutate, isPending } = useFolderMutation()

    function handleSubmit(data: FolderFormSchema) {
        mutate(data, {
            onSuccess: () => {
                toast.success('Folder created!')
                onComplete()
            },
        })
    }

    return (
        <Form {...form}>
            <form
                onSubmit={form.handleSubmit(handleSubmit)}
                className="space-y-3"
            >
                <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel className="text-slate-200">
                                Folder name:
                            </FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <div className="flex justify-end">
                    <Button
                        type="submit"
                        variant={'primary'}
                        className="w-[120px]"
                        disabled={isPending}
                    >
                        Save <CheckCircle />
                    </Button>
                </div>
            </form>
        </Form>
    )
}
