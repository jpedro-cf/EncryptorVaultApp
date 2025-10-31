import { useEffect, useState } from 'react'
import { Alert, AlertDescription } from '../ui/alert'
import { AlertCircle, DownloadCloud } from 'lucide-react'
import { Button } from '../ui/button'
import { FileDisplaySkeleton } from './FileDisplaySkeleton'

export interface DisplayProps {
    id: string
    fileContent: ArrayBuffer
    contentType: string
}

export function TextDisplay({ fileContent, contentType, id: _ }: DisplayProps) {
    const [lines, setLines] = useState<string[]>([])

    function handle() {
        const reader = new FileReader()

        reader.onload = (e) => {
            const text = (e?.target?.result ?? '') as string
            setLines(text.split('\n'))
        }

        reader.readAsText(new Blob([fileContent], { type: contentType }))
    }
    useEffect(() => {
        handle()
    }, [fileContent])

    return (
        <div className="bg-slate-800 text-slate-200 rounded-md p-5 font-semibold">
            {!lines && <FileDisplaySkeleton />}
            {lines.map((line, index) => (
                <p key={index} className="whitespace-pre-wrap">
                    {line}
                </p>
            ))}
        </div>
    )
}

export function AudioDisplay({
    fileContent,
    contentType,
    id: _,
}: DisplayProps) {
    const [audioUrl, setAudioUrl] = useState<string | null>(null)

    function handle() {
        const audioBlob = new Blob([fileContent], { type: contentType })
        const url = URL.createObjectURL(audioBlob)
        setAudioUrl(url)

        return () => {
            URL.revokeObjectURL(url)
        }
    }
    useEffect(() => {
        handle()
    }, [fileContent])

    return (
        <div className="bg-slate-900 text-slate-200 rounded-md p-5 font-semibold">
            {!audioUrl && <FileDisplaySkeleton />}
            {audioUrl && <audio controls src={audioUrl} />}
        </div>
    )
}

export function VideoDisplay({
    fileContent,
    contentType,
    id: _,
}: DisplayProps) {
    const [videoUrl, setVideoUrl] = useState<string | null>(null)

    function handle() {
        const audioBlob = new Blob([fileContent], { type: contentType })
        const url = URL.createObjectURL(audioBlob)
        setVideoUrl(url)

        return () => {
            URL.revokeObjectURL(url)
        }
    }
    useEffect(() => {
        handle()
    }, [fileContent])

    return (
        <div className="bg-slate-900 text-slate-200 rounded-md font-semibold w-[90vw] sm:w-auto">
            {!videoUrl && (
                <div className="p-5">
                    <FileDisplaySkeleton />
                </div>
            )}
            {videoUrl && <video controls src={videoUrl} />}
        </div>
    )
}

export function ImageDisplay({
    fileContent,
    contentType,
    id: _,
}: DisplayProps) {
    const [imageUrl, setImageUrl] = useState<string | null>(null)

    function handle() {
        const audioBlob = new Blob([fileContent], { type: contentType })
        const url = URL.createObjectURL(audioBlob)
        setImageUrl(url)

        return () => {
            URL.revokeObjectURL(url)
        }
    }
    useEffect(() => {
        handle()
    }, [fileContent])

    return (
        <div className="bg-slate-900 text-slate-200 rounded-md font-semibold w-[90vw] sm:w-auto">
            {!imageUrl && (
                <div className="p-5">
                    <FileDisplaySkeleton />
                </div>
            )}
            {imageUrl && <img src={imageUrl} />}
        </div>
    )
}

export function ApplicationDisplay({
    fileContent,
    contentType,
    id: _,
}: DisplayProps) {
    const [url, setUrl] = useState<string | null>(null)

    function handle() {
        const audioBlob = new Blob([fileContent], { type: contentType })
        const url = URL.createObjectURL(audioBlob)
        setUrl(url)

        return () => {
            URL.revokeObjectURL(url)
        }
    }
    useEffect(() => {
        handle()
    }, [fileContent])

    return (
        <div className="bg-slate-900 text-slate-200 rounded-md font-semibold p-5">
            {!url && (
                <div className="p-5">
                    <FileDisplaySkeleton />
                </div>
            )}
            {url && (
                <Alert className="border-blue-500/50 bg-blue-500/10 flex items-start gap-2">
                    <div className="text-blue-400">
                        <AlertCircle className="h-4 w-4" />
                    </div>
                    <AlertDescription className="text-blue-300 text-sm">
                        Files of type "application" can't be visualized.
                        <a href={url} download={url} className="block mt-3">
                            <Button variant={'primary'}>
                                <DownloadCloud /> Download
                            </Button>
                        </a>
                    </AlertDescription>
                </Alert>
            )}
        </div>
    )
}
