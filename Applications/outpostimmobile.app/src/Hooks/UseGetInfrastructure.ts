import {useQuery} from "@tanstack/react-query";
import type {PingDto, TypedResponse} from "../Models/Types.ts";
import {ApiClient} from "../Helpers/ApiClient.ts";
import { INFRASTRUCTURE_URL } from "../Consts.ts";

export const UseGetInfrastructure = () =>
    useQuery({
        queryKey: ['infrastructure'],
        queryFn: async () => {
            const response = await ApiClient.get<TypedResponse<PingDto>>(INFRASTRUCTURE_URL);
            return response.data;
        }
    });