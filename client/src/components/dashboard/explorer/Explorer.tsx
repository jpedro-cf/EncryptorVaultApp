import { useEffect, useState } from 'react'
import { ExplorerContext } from './ExplorerContext'
import { VaultSecret } from '@/components/dashboard/VaultSecret'
import { useAuth } from '@/hooks/use-auth'
import { useItems } from '@/api/items/items'
import { ItemCardSkeleton } from '@/components/items/ItemCardSkeleton'
import { ItemsGrid } from '@/components/items/ItemsGrid'
import { useKeys } from '@/hooks/use-keys'
import { ExplorerHeader } from '@/components/dashboard/explorer/ExplorerHeader'
import { useFolder } from '@/api/folders/folders'
import type { Folder } from '@/types/folders'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { AlertCircle, ArrowLeftSquare } from 'lucide-react'
import { Link } from 'react-router'
import { Button } from '@/components/ui/button'

interface Props {
    folderId: string | null
}
export function Explorer({ folderId }: Props) {
    const [currentFolderId, setCurrentFolderId] = useState<string | null>(
        folderId
    )
    const [folderTree, setFolderTree] = useState<Folder[]>([])

    const { account } = useAuth()
    const { rootKey } = useKeys()

    function pushFolder(folder: Folder) {
        setFolderTree((prev) => {
            const index = prev.findIndex((f) => f.id === folder.id)

            if (index !== -1) {
                return prev.slice(0, index + 1)
            }

            return [...prev, folder]
        })
    }

    const isInRootPage = account != null && rootKey != null && !folderId
    const isInFolderPage =
        !isInRootPage && (folderTree.length > 0 || folderId != null)

    const rootItems = useItems({
        enabled: isInRootPage,
    })

    const folderQuery = useFolder({
        enabled: isInFolderPage && rootKey != null,
        folderId: currentFolderId ?? '',
        shareId: null,
    })

    useEffect(() => {
        if (folderQuery.data) {
            pushFolder(folderQuery.data)
        }
    }, [folderQuery.data])

    useEffect(() => {
        if (folderId) {
            setCurrentFolderId(folderId)
        }
    }, [folderId])

    if (rootItems.isLoading || folderQuery.isLoading) {
        return (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
                {[1, 2, 3, 4].map((i) => (
                    <ItemCardSkeleton key={i} />
                ))}
            </div>
        )
    }

    if (rootItems.isError || folderQuery.isError) {
        return (
            <Alert className="border-blue-500/50 bg-blue-500/10 flex items-start gap-2 mt-5">
                <div className="text-blue-400">
                    <AlertCircle className="h-4 w-4" />
                </div>
                <AlertDescription className="text-blue-300 text-sm">
                    An error occured while fetching your content, try again
                    later or contact us.
                    {isInFolderPage && (
                        <Link
                            to={`${
                                folderTree.length > 0
                                    ? '/folders/' +
                                      folderTree[folderTree.length - 1].id
                                    : '/'
                            } `}
                        >
                            <Button
                                variant={'primary'}
                                size={'sm'}
                                className="mt-1"
                            >
                                Go back <ArrowLeftSquare />
                            </Button>
                        </Link>
                    )}
                </AlertDescription>
            </Alert>
        )
    }

    return (
        <ExplorerContext.Provider
            value={{
                shareId: null,
                folderTree,
                pushFolder,
                currentFolderId,
                setCurrentFolderId,
                items: folderQuery.data?.children ?? rootItems.data ?? [],
            }}
        >
            <ExplorerHeader />
            {rootKey != null && <ItemsGrid />}
            <VaultSecret />
        </ExplorerContext.Provider>
    )
}
