import {createBrowserRouter, createRoutesFromElements, Route} from "react-router";
import {Layout} from "./Layout.tsx";
import ReportWebVitals from "./ReportWebVitals.ts";
import {InfrastructurePage} from "../Pages/InfrastructurePage.tsx";

const nonAuthRoutes = (
    <>
        <Route element={<Layout />}>
            <Route path="/Infrastructure" element={<InfrastructurePage />}/>
            <Route index element={<InfrastructurePage />}/>
        </Route>
    </>
)

export const routings = createBrowserRouter(
    createRoutesFromElements(
        <>
            {nonAuthRoutes}
            {/*{authRoutes}*/}
        </>
    ))

ReportWebVitals();

export default routings;