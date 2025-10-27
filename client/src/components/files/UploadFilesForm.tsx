import z from 'zod'
import { Form } from '../ui/form'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { DropZone, DropZoneContent } from '../ui/drop-zone'

const uploadFilesSchema = z.object({
    parentId: z.string().min(1).optional(),
    files: z.array(z.instanceof(File)),
})

export type UploadFilesSchema = z.infer<typeof uploadFilesSchema>

interface Props {
    parentId?: string
}
export function UploadFilesForm({ parentId }: Props) {
    const form = useForm<UploadFilesSchema>({
        resolver: zodResolver(uploadFilesSchema),
        defaultValues: {
            parentId,
        },
    })

    function handleSubmit(data: UploadFilesSchema) {
        console.log(data)
    }

    function handleDrop(files: File[]) {
        console.log(files)
    }
    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(handleSubmit)}>
                <DropZone onDrop={handleDrop}>
                    <DropZoneContent />
                </DropZone>
            </form>
        </Form>
    )
}
