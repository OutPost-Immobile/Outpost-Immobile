import {createBrowserRouter, createRoutesFromElements, Route} from "react-router";
import {Layout} from "./Layout.tsx";
import ReportWebVitals from "./ReportWebVitals.ts";
import {InfrastructurePage} from "../Pages/InfrastructurePage.tsx";
import {LandingPage} from "../Pages/LandingPage.tsx";
import {LoginPage} from "../Pages/LoginPage.tsx";
import { MaczkopatPage } from "../Pages/MaczkopatPage.tsx";
import {ProtectedRoute} from "../Auth/ProtectedRoute.tsx";
import {RoutesPage} from "../Pages/RoutesPage.tsx";
import {UpdateParcelStatusPage} from "../Pages/UpdateParcelStatusPage.tsx";

const nonAuthRoutes = (
    <>
        <Route element={<Layout />}>
            <Route path="/Infrastructure" element={<InfrastructurePage />}/>
            <Route index element={<LandingPage />}/>
            <Route path="/Login" element={<LoginPage />}/>
        </Route>
    </>
)
const authRoutes = (
    <>
        <Route element={<ProtectedRoute />}>
            <Route element={<Layout />}>
                <Route path="/Maczkopat" element={<MaczkopatPage/>}/>
                <Route path="/Routes" element={<RoutesPage/>}/>
                <Route path="/Parcels/UpdateStatus" element={<UpdateParcelStatusPage />}/>
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