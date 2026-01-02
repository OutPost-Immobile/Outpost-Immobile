import { Navigate, Outlet, useLocation } from 'react-router';
import { useAuth } from './AuthProvider.tsx';

export const ProtectedRoute = () => {
    const { isAuthenticated } = useAuth();
    const location = useLocation();

    if (!isAuthenticated) {
        return <Navigate to="/Login" state={{ from: location }} replace />;
    }

    return <Outlet />;
};