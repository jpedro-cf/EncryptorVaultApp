import { PlusCircle } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { useExplorerContext } from './ExplorerContext'
import { useKeys } from '@/hooks/use-keys'
import {
    Breadcrumb,
    BreadcrumbEllipsis,
    BreadcrumbItem,
    BreadcrumbLink,
    BreadcrumbList,
    BreadcrumbPage,
    BreadcrumbSeparator,
} from '@/components/ui/breadcrumb'
import { Link } from 'react-router'
import { FolderDialog } from '@/components/folders/FolderDialog'
import { UploadFilesDialog } from '@/components/files/UploadFilesDialog'

export function ExplorerHeader() {
    const { rootKey } = useKeys()
    const { folderTree, shareId } = useExplorerContext()

    const currentFolder =
        folderTree.length > 0 ? folderTree[folderTree.length - 1] : null

    if (!rootKey && !shareId) {
        return <></>
    }

    const slicedTree =
        folderTree.length > 2
            ? folderTree.slice(1, -1)
            : folderTree.slice(0, -1)

    return (
        <div className="flex items-end justify-between mb-8">
            <div>
                {currentFolder && (
                    <Breadcrumb className="mb-3">
                        <BreadcrumbList>
                            <BreadcrumbItem>
                                <BreadcrumbLink asChild>
                                    <Link to="/">Home</Link>
                                </BreadcrumbLink>
                            </BreadcrumbItem>
                            {folderTree.length >= 3 && (
                                <>
                                    <BreadcrumbSeparator />
                                    <BreadcrumbItem className="text-slate-100">
                                        <BreadcrumbEllipsis />
                                    </BreadcrumbItem>
                                </>
                            )}
                            <BreadcrumbSeparator />
                            {slicedTree.map((folder) => (
                                <>
                                    <BreadcrumbItem key={folder.id}>
                                        <BreadcrumbLink asChild>
                                            {!shareId ? (
                                                <Link
                                                    to={`/folders/${folder.id}`}
                                                >
                                                    {folder.name}
                                                </Link>
                                            ) : (
                                                <span>{folder.name}</span>
                                            )}
                                        </BreadcrumbLink>
                                    </BreadcrumbItem>
                                    <BreadcrumbSeparator />
                                </>
                            ))}
                            <BreadcrumbItem>
                                <BreadcrumbPage>
                                    {currentFolder.name}
                                </BreadcrumbPage>
                            </BreadcrumbItem>
                        </BreadcrumbList>
                    </Breadcrumb>
                )}
                <h1 className="text-3xl font-bold text-white mb-1">
                    {currentFolder ? currentFolder.name : 'Explore'}
                </h1>
                <p className="text-slate-400">
                    {shareId
                        ? 'Explore the files and folders shared with you'
                        : 'Manage your encrypted files and folders'}
                </p>
            </div>
            {!shareId && (
                <div className="flex items-center gap-3">
                    <FolderDialog>
                        <Button variant={'default'}>
                            {currentFolder ? 'Add subfolder' : 'Add folder'}
                            <PlusCircle />
                        </Button>
                    </FolderDialog>
                    <UploadFilesDialog>
                        <Button variant={'primary'}>
                            Upload File
                            <PlusCircle />
                        </Button>
                    </UploadFilesDialog>
                </div>
            )}
        </div>
    )
}
