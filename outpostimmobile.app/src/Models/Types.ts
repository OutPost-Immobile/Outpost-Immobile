export type TypedResponse<T> = {
    data: T,
    Errors: string | null,
    HttpStatusCode: number
}

export type PingDto = {
   message: string
}