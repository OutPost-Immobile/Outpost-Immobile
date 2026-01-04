export async function* streamJsonArray<T>(response: Response): AsyncGenerator<T, void, unknown> {
    if (!response.body) throw new Error("Response body is null");

    const reader = response.body.getReader();
    const decoder = new TextDecoder("utf-8");
    let buffer = "";

    while (true) {
        const { done, value } = await reader.read();
        if (done) break;

        buffer += decoder.decode(value, { stream: true });

        let startIndex = 0;
        let depth = 0;
        let inString = false;
        let isEscaped = false;

        for (let i = 0; i < buffer.length; i++) {
            const char = buffer[i];

            if (isEscaped) {
                isEscaped = false;
                continue;
            }
            if (char === '\\') {
                isEscaped = true;
                continue;
            }

            if (char === '"') {
                inString = !inString;
                continue;
            }

            if (!inString) {
                if (char === '{') {
                    depth++;
                } else if (char === '}') {
                    depth--;

                    if (depth === 0) {
                        const chunk = buffer.slice(startIndex, i + 1);

                        const cleanChunk = chunk.replace(/^[,\s\[]+/, "");

                        if (cleanChunk.trim()) {
                            try {
                                const parsed: T = JSON.parse(cleanChunk);
                                yield parsed;
                            } catch (e) {
                                console.warn("Skipped malformed chunk", e);
                            }
                        }

                        startIndex = i + 1;
                    }
                }
            }
        }

        // Remove the processed portion from the buffer
        if (startIndex > 0) {
            buffer = buffer.slice(startIndex);
        }
    }
}