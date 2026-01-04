export interface RouteSegment {
    seq: number;
    geoJson: string;      
    segment_Dist: number;  
    total_Dist: number;    
}

export interface LatLng {
    lat: number;
    lng: number;
}

export interface FullRouteData {
    totalDistance: number;
    segments: RouteSegment[];
}