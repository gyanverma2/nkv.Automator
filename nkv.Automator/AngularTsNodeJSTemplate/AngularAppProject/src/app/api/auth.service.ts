import { Injectable } from '@angular/core';
import axios, { AxiosError, AxiosResponse } from 'axios';
@Injectable({
    providedIn: 'root'
})
export class AuthService {

    constructor() { }

    setToken(key, value) {
        localStorage.setItem(key, value);
    }

    generateToken(username: string, password: string): Promise<any> {
        return new Promise(async (resolve, reject) => {
            try {
                let item = {
                    "username": username,
                    "password": password
                };
                const config = {
                    headers: {
                        'Access-Control-Allow-Origin': '*'
                    }
                };
                const response = await axios.post("/token", JSON.stringify(item), config);
                if (response) {
                    const res = response.data;
                    if (res.code === 1) {
                        let token = res.document.access_token;
                        let expiryDate = res.document.expires_in;
                        this.setToken('token', token);
                        this.setToken('token_exp', expiryDate);
                        resolve(res);
                    } else {
                        resolve(res);
                    }
                } else {
                    resolve({ code: 0 });
                }
            } catch (error: any) {
                if (axios.isAxiosError(error)) {
                    resolve(error);
                } else {
                    resolve({ code: 0, message: 'Some error occured' });
                }
            }
        });
    }
    getHeaders() {
        let token = 'Bearer ' + localStorage.getItem('token');
        return {
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'application/json',
            'Authorization': token
        };
    }
    
    getHeadersUpload() {
        let token = 'Bearer ' + localStorage.getItem('token');
        return {
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'multipart/form-data',
            'Authorization': token
        };
    }

}
