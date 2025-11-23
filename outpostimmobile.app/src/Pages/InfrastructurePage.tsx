import {UseGetInfrastructure} from "../Hooks/UseGetInfrastructure.ts";
import {Typography} from "@mui/material";

export const InfrastructurePage = () => {

    console.log("Wysyłam");

    const { data } = UseGetInfrastructure();

    console.log(data?.data);
    console.log(data);

    return (
        <>
            <div>
                <Typography variant="h4">{data?.data.message}</Typography>
                <Typography variant="h4">AAAAAAAAAAAAAAAaa</Typography>
            </div>
        </>
    )
}