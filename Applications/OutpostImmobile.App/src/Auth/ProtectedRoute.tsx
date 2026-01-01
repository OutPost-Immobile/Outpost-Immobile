import { Navigate, Outlet, useLocation } from 'react-router';
import { useAuth } from './AuthProvider.tsx';

export const ProtectedRoute = () => {
    const { isAuthenticated } = useAuth();
    const location = useLocation();

    if (!isAuthenticated) {
        // Redirect them to the /Login page, but save the current location they were
        // trying to go to. This allows us to send them back after they login.
        return <Navigate to="/Login" state={{ from: location }} replace />;
    }

    return <Outlet />;
};