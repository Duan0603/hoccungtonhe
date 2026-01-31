'use client';

import { useState, useEffect } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';

export default function CourseFilter() {
    const router = useRouter();
    const searchParams = useSearchParams();

    const [filters, setFilters] = useState({
        search: searchParams.get('search') || '',
        subject: searchParams.get('subject') || '',
        grade: searchParams.get('grade') || '',
        minPrice: searchParams.get('minPrice') || '',
        maxPrice: searchParams.get('maxPrice') || '',
    });

    useEffect(() => {
        const timer = setTimeout(() => {
            applyFilters();
        }, 500);
        return () => clearTimeout(timer);
    }, [filters]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFilters((prev) => ({ ...prev, [name]: value }));
    };

    const applyFilters = () => {
        const params = new URLSearchParams();
        if (filters.search) params.set('search', filters.search);
        if (filters.subject) params.set('subject', filters.subject);
        if (filters.grade) params.set('grade', filters.grade);
        if (filters.minPrice) params.set('minPrice', filters.minPrice);
        if (filters.maxPrice) params.set('maxPrice', filters.maxPrice);
        params.set('page', '1');
        router.push(`/courses?${params.toString()}`);
    };

    return (
        <div className="space-y-5">
            <h3 className="text-lg font-bold text-[var(--text-primary)] flex items-center gap-2">
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
                </svg>
                Bộ lọc
            </h3>

            {/* Search */}
            <div>
                <div className="relative">
                    <input
                        type="text"
                        name="search"
                        value={filters.search}
                        onChange={handleChange}
                        placeholder="Tìm kiếm..."
                        className="input-glass pl-10"
                    />
                    <svg className="w-5 h-5 text-[var(--text-muted)] absolute left-4 top-1/2 -translate-y-1/2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                    </svg>
                </div>
            </div>

            {/* Subject */}
            <div>
                <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Môn học</label>
                <select
                    name="subject"
                    value={filters.subject}
                    onChange={handleChange}
                    className="input-glass"
                >
                    <option value="">Tất cả môn</option>
                    <option value="Toán">Toán</option>
                    <option value="Lý">Vật Lý</option>
                    <option value="Hóa">Hóa Học</option>
                    <option value="Anh">Tiếng Anh</option>
                    <option value="Sinh">Sinh Học</option>
                    <option value="Văn">Ngữ Văn</option>
                </select>
            </div>

            {/* Grade */}
            <div>
                <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Lớp</label>
                <select
                    name="grade"
                    value={filters.grade}
                    onChange={handleChange}
                    className="input-glass"
                >
                    <option value="">Tất cả lớp</option>
                    <option value="12">Lớp 12</option>
                    <option value="11">Lớp 11</option>
                    <option value="10">Lớp 10</option>
                </select>
            </div>

            {/* Price Range */}
            <div>
                <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Khoảng giá</label>
                <div className="flex items-center gap-2">
                    <input
                        type="number"
                        name="minPrice"
                        value={filters.minPrice}
                        onChange={handleChange}
                        placeholder="Min"
                        className="input-glass text-sm"
                    />
                    <span className="text-[var(--text-muted)]">→</span>
                    <input
                        type="number"
                        name="maxPrice"
                        value={filters.maxPrice}
                        onChange={handleChange}
                        placeholder="Max"
                        className="input-glass text-sm"
                    />
                </div>
            </div>

            {/* Clear Button */}
            <button
                onClick={() => setFilters({ search: '', subject: '', grade: '', minPrice: '', maxPrice: '' })}
                className="w-full py-2.5 text-sm text-[var(--text-secondary)] hover:text-[var(--primary)] font-medium transition-all-smooth border border-[var(--glass-border)] rounded-xl hover:bg-[var(--glass-bg-light)]"
            >
                Xóa bộ lọc
            </button>
        </div>
    );
}
