import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({
  variable: "--font-inter",
  subsets: ["latin", "vietnamese"],
  display: "swap",
});

export const metadata: Metadata = {
  title: "hoccungtonhe - Ôn thi THPT Quốc Gia",
  description: "Nền tảng ôn thi THPT Quốc Gia với AI chấm bài thông minh. Học online hiệu quả với video bài giảng chất lượng cao.",
  keywords: ["ôn thi THPT", "THPT Quốc Gia", "học online", "luyện thi đại học", "hoccungtonhe"],
  authors: [{ name: "hoccungtonhe" }],
  openGraph: {
    title: "hoccungtonhe - Ôn thi THPT Quốc Gia",
    description: "Nền tảng ôn thi THPT với AI chấm bài thông minh",
    type: "website",
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="vi" suppressHydrationWarning>
      <body
        className={`${inter.variable} font-sans antialiased bg-[var(--background)] text-[var(--text-primary)]`}
        suppressHydrationWarning
      >
        {/* Global Mesh Background */}
        <div className="fixed inset-0 overflow-hidden pointer-events-none -z-10">
          <div className="mesh-bg" />
        </div>
        {children}
      </body>
    </html>
  );
}
