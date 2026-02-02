import { $api } from "../Api/Api.ts";
import { ENUM_TRANSLATIONS_URL, GET_METHOD } from "../Consts.ts";

export const useEnumTranslations = (enumName: string) => {
    return $api.useQuery(
        GET_METHOD,
        ENUM_TRANSLATIONS_URL,
        {
            params: {
                path: { enumName },
            },
        },
        {
            staleTime: 1000 * 60 * 60, 
            gcTime: 1000 * 60 * 60, 
        }
    );
};
