import { useState } from "react";

export function usePagination(initialPage = 1) {
    const [page, setPage] = useState<number>(initialPage);
    const [totalPages, setTotalPages] = useState<number | null>(null);

    const handleChange = (event: React.ChangeEvent<unknown>, value: number) => {
        setPage(value);
    };

    return {
        page,
        handleChange,
        setPage,
        setTotalPages,
        totalPages,
    };
}

// const [isLoading, setIsLoading] = useState(false);

// const startLoading = () => setIsLoading(true);
// const finishLoading = () => setIsLoading(false);

// return {
//     isLoading,
//     startLoading,
//     finishLoading,
// };
