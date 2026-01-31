'use client';

import { useEffect, useState } from 'react';

interface VideoPlayerProps {
    url: string;
}

export default function VideoPlayer({ url }: VideoPlayerProps) {
    const [hasMounted, setHasMounted] = useState(false);

    useEffect(() => {
        setHasMounted(true);
    }, []);

    if (!hasMounted) {
        return (
            <div className="aspect-video bg-gradient-to-br from-[var(--glass-bg)] to-[var(--glass-bg-light)] animate-pulse flex items-center justify-center">
                <div className="animate-spin w-8 h-8 border-3 border-[var(--primary)] border-t-transparent rounded-full"></div>
            </div>
        );
    }

    const videoSrc = url.startsWith('http')
        ? url
        : `${process.env.NEXT_PUBLIC_API_URL}${url}`;

    return (
        <div className="relative aspect-video bg-black overflow-hidden group">
            <video
                src={videoSrc}
                controls
                className="w-full h-full object-contain"
                controlsList="nodownload"
                playsInline
            >
                Your browser does not support the video tag.
            </video>
            {/* Custom Play Overlay (hidden when playing) */}
            <div className="absolute inset-0 bg-black/20 opacity-0 group-hover:opacity-100 transition-opacity pointer-events-none flex items-center justify-center">
                {/* Subtle gradient overlay on hover */}
            </div>
        </div>
    );
}
