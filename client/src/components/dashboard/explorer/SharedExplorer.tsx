import type { Folder } from '@/types/folders'
import { useState } from 'react'
import { ExplorerContext } from './ExplorerContext'
import { useKeys } from '@/hooks/use-keys'
import { useFolder } from '@/api/folders/folders'
import { ExplorerHeader } from './ExplorerHeader'
import { ItemsGrid } from '@/components/items/ItemsGrid'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { AlertCircle, ArrowLeftSquare } from 'lucide-react'
import { Link } from 'react-router'
import { Button } from '@/components/ui/button'
import { useSharedLink } from '@/api/share/share'

interface SharedExplorerProps {
    sharedLinkId: string
}

export function SharedExplorer({ sharedLinkId }: SharedExplorerProps) {
    const [currentFolderId, setCurrentFolderId] = useState<string | null>(null)
    const [folderTree, setFolderTree] = useState<Folder[]>([])

    function pushFolder(folder: Folder) {
        setFolderTree((prev) => {
            if (prev.some((f) => f.id === folder.id)) return prev
            return [...prev, folder]
        })
    }

    const isInFolderPage = currentFolderId != null

    const sharedItems = useSharedLink({
        shareId: sharedLinkId,
        enabled: !isInFolderPage,
    })

    console.log(sharedItems.data)
    console.log(sharedItems.error)

    const folderQuery = useFolder({
        enabled: isInFolderPage,
        folderId: currentFolderId ?? '',
        shareId: sharedLinkId,
    })

    if (sharedItems.isError || folderQuery.isError) {
        return (
            <Alert className="border-blue-500/50 bg-blue-500/10 flex items-start gap-2 mt-5">
                <div className="text-blue-400">
                    <AlertCircle className="h-4 w-4" />
                </div>
                <AlertDescription className="text-blue-300 text-sm">
                    An error occured while fetching the content shared with you,
                    try again later or verify if the shared link is still valid.
                    {isInFolderPage && (
                        <Button
                            variant={'primary'}
                            size={'sm'}
                            className="mt-1"
                            onClick={() =>
                                setCurrentFolderId(
                                    folderTree[folderTree.length - 1].id
                                )
                            }
                        >
                            Go back <ArrowLeftSquare />
                        </Button>
                    )}
                </AlertDescription>
            </Alert>
        )
    }

    return (
        <ExplorerContext.Provider
            value={{
                shareId: sharedLinkId,
                folderTree,
                pushFolder,
                currentFolderId,
                setCurrentFolderId,
                items: folderQuery.data?.children ?? sharedItems.data ?? [],
            }}
        >
            <span className="text-slate-100">{window.location.hash}</span>
            <ExplorerHeader />
            <ItemsGrid />
        </ExplorerContext.Provider>
    )
}
