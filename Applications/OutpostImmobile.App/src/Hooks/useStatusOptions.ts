import { $api } from "../Api/Api.ts";
import { ENUM_TRANSLATIONS_URL, GET_METHOD } from "../Consts.ts";

interface StatusOption {
    value: number;
    label: string;
}

// Default fallback options
const DEFAULT_STATUS_OPTIONS: StatusOption[] = [
    { value: 0, label: "Nadana" },
    { value: 1, label: "W drodze" },
    { value: 2, label: "W maczkopcie" },
    { value: 3, label: "Odebrana" },
    { value: 4, label: "Zwrócona" },
];

export const useStatusOptions = () => {
    const { data, isLoading, isError, error } = $api.useQuery(
        GET_METHOD,
        ENUM_TRANSLATIONS_URL,
        {
            params: {
                path: { enumName: "ParcelStatus" },
            },
        },
        {
            staleTime: 1000 * 60 * 60,
            gcTime: 1000 * 60 * 60,
        }
    );

    const apiData = data?.data;
    const options: StatusOption[] = apiData && Array.isArray(apiData) && apiData.length > 0
        ? apiData.map((item) => ({
            value: item.value,
            label: item.label,
        }))
        : DEFAULT_STATUS_OPTIONS;

    return {
        options,
        isLoading,
        isError,
        error,
    };
};
