import {createBrowserRouter, createRoutesFromElements, Route} from "react-router";
import {Layout} from "./Layout.tsx";
import ReportWebVitals from "./ReportWebVitals.ts";
import {InfrastructurePage} from "../Pages/InfrastructurePage.tsx";
import {LandingPage} from "../Pages/LandingPage.tsx";
import {LoginPage} from "../Pages/LoginPage.tsx";
import { MaczkopatPage } from "../Pages/MaczkopatPage.tsx";
import {ProtectedRoute} from "../Auth/ProtectedRoute.tsx";

const nonAuthRoutes = (
    <>
        <Route element={<Layout />}>
            <Route path="/Infrastructure" element={<InfrastructurePage />}/>
            <Route index element={<LandingPage />}/>
        </Route>
        <Route element={<Layout />}>
            <Route path="/Login" element={<LoginPage />}/>
        </Route>
    </>
)
const authRoutes = (
    <>
        <Route element={<ProtectedRoute />}>
            <Route element={<Layout />}>
                <Route path="/Maczkopat" element={<MaczkopatPage/>}/>
            </Route>
        </Route>
    </>
)

export const routings = createBrowserRouter(
    createRoutesFromElements(
        <>
            {nonAuthRoutes}
            {authRoutes}
        </>
    ))

ReportWebVitals();

export default routings;