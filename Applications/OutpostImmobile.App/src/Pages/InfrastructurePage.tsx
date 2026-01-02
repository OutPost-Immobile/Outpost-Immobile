import {Typography} from "@mui/material";
import {$api} from "../Api/Api.ts";
import { INFRASTRUCTURE_URL } from "../Consts.ts";

export const InfrastructurePage = () => {
    
    const { data } = $api.useQuery("get", INFRASTRUCTURE_URL);
    
    const message = data?.data?.message;

    return (
        <>
            <div>
                <Typography variant="h4">{message}</Typography>
                <Typography variant="h4">AAAAAAAAAAAAAAAaa</Typography>
            </div>
        </>
    )
}