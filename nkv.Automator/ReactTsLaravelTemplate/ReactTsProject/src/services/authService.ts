
import { APIService } from "services";

export const tokenAPICALL = async (user, pass) => {
    const response = await APIService.apiNoAuth().post(`/token/generate.php`, {
        "username": user,
        "password": pass
    }, {
        headers: {
            'Content-Type': 'application/json; charset=UTF-8'
        }
    })

    if (response && response.status === 200 && response.data) {
        const data = response.data.data;
        const now = new Date().getTime();
        const expiryDate = now + data.expires_in;
        return { jwtToken: data.access_token, expiryDate: expiryDate, userProfile: data.user };
    } else {
        return null;
    }
}