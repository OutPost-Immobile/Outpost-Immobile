import axios from 'axios';
import { API_URL } from '../Consts';

export const ApiClient = axios.create({
    baseURL: API_URL,
});

ApiClient.interceptors.response.use((response) => {
    return response;
}, (error) => {
    if (error.response && error.response.status < 500) {
        return error.response;
    }

    return Promise.reject(new Error("Could not connect to the server. Please try again later."));
})