import axios from "axios"
export class APIService {

    static apiCustomHeader(headers: any) {
        return axios.create({
            baseURL: process.env.REACT_APP_API_BASE_URL,
            headers: headers,
        })
    }
    static api() {
        const jwtToken = localStorage.getItem("token");
        const api = axios.create({
            baseURL: process.env.REACT_APP_API_BASE_URL,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + jwtToken
            }
        });
        api.interceptors.response.use(
            (res) => {
                return res;
            },
            (err) => {
                if (err.response) {
                    if (err.response.status === 401 || err.response.status === 403) {
                        return window.location.href = '/'
                    } else {
                        return err.response.data;
                    }
                }
            }
        );
        return api;
    }
    static apiNoAuth() {
        return axios.create({
            baseURL: process.env.REACT_APP_API_BASE_URL,
            headers: {
                //'Access-Control-Allow-Origin': '*',
                'Content-Type': 'application/json',
            }
        })
    }
}