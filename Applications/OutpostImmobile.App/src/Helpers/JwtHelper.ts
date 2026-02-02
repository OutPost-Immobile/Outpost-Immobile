interface JwtPayload {
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"?: string;
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"?: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string;
    exp?: number;
    iss?: string;
    aud?: string;
}

export interface UserInfo {
    userId: string | null;
    email: string | null;
    role: string | null;
}

export const parseJwt = (token: string): JwtPayload | null => {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(jsonPayload);
    } catch {
        return null;
    }
};

export const getUserInfoFromToken = (): UserInfo => {
    const token = localStorage.getItem('token');
    
    if (!token) {
        return { userId: null, email: null, role: null };
    }
    
    const payload = parseJwt(token);
    
    if (!payload) {
        return { userId: null, email: null, role: null };
    }
    
    return {
        userId: payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ?? null,
        email: payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] ?? null,
        role: payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? null,
    };
};

export const isCourier = (): boolean => {
    const { role } = getUserInfoFromToken();
    return role === "Courier";
};
