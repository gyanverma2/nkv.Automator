import { APIService } from "services"

export const uploadImageService = (data) => {
    return APIService.api().post(`/upload/image`, data);
}
export const uploadFileService = (data) => {
    return APIService.api().post(`/upload/file`, data);
}