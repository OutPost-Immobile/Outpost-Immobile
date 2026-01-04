import {API_URL, ROUTE_STREAM_URL} from "../Consts.ts";
import type {FullRouteData, RouteSegment} from "../Types.ts";
import {streamJsonArray} from "../Helpers/StreamHelper.ts";

export async function fetchRouteStream(
    routeId: number,
    onChunk: (segment: RouteSegment) => void
): Promise<FullRouteData> {
    
    const endpoint = ROUTE_STREAM_URL.replace("{routeId}", routeId.toString());
    const fullUrl = `${API_URL}${endpoint}`;
    
    const token = localStorage.getItem("token");

    const response = await fetch(fullUrl, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        throw new Error(`API Error: ${response.status} ${response.statusText}`);
    }

    const collectedSegments: RouteSegment[] = [];
    let totalDistance = 0;
    
    for await (const segment of streamJsonArray<RouteSegment>(response)) {
        if (totalDistance === 0 && segment.total_Dist > 0) {
            totalDistance = segment.total_Dist;
        }

        onChunk(segment);
        collectedSegments.push(segment);
    }

    return { totalDistance, segments: collectedSegments };
}