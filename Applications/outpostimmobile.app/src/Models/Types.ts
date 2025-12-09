export type TypedResponse<T> = {
    data: T,
    errors: string | null,
    httpStatusCode: number
}

export type PingDto = {
   message: string
}