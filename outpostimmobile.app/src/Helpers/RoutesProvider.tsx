import {createBrowserRouter, createRoutesFromElements, Route} from "react-router";
import {Layout} from "./Layout.tsx";
import ReportWebVitals from "./ReportWebVitals.ts";
import {InfrastructurePage} from "../Pages/InfrastructurePage.tsx";
import {LandingPage} from "../Pages/LandingPage.tsx";

const nonAuthRoutes = (
    <>
        <Route element={<Layout />}>
            <Route path="/Infrastructure" element={<InfrastructurePage />}/>
            <Route index element={<LandingPage />}/>
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