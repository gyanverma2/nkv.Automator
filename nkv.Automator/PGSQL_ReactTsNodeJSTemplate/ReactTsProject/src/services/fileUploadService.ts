import { APIService } from "services"

export const uploadImageService = (data) => {
    return APIService.api().post(`/upload`, data);
}
export const uploadFileService = (data) => {
    return APIService.api().post(`/upload`, data);
}