export async function* streamJsonArray<T>(response: Response): AsyncGenerator<T, void, unknown> {
    if (!response.body) throw new Error("Response body is null");

    const reader = response.body.getReader();
    const decoder = new TextDecoder("utf-8");
    let buffer = "";

    while (true) {
        const { done, value } = await reader.read();
        if (done) break;

        buffer += decoder.decode(value, { stream: true });
        
        let boundary = buffer.indexOf("}");

        while (boundary !== -1) {
            const chunk = buffer.slice(0, boundary + 1);
            buffer = buffer.slice(boundary + 1);
            
            const cleanChunk = chunk.replace(/^[,\s\[]+/, "");

            if (cleanChunk.trim().startsWith("{")) {
                try {
                    const parsed: T = JSON.parse(cleanChunk);
                    yield parsed;
                } catch (e) {
                    console.warn("Skipped malformed chunk", e);
                }
            }

            boundary = buffer.indexOf("}");
        }
    }
}