import axios from 'axios'

export const api = axios.create({
    baseURL: 'http://localhost:5190/api',
    withCredentials: true,
})
