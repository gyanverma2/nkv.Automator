import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { UploadService } from 'src/app/service/upload.service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})
export class UploadComponent implements OnInit {

  fileName:string;
  fileImageName:string;
  loading:boolean;
  constructor(private uploadService: UploadService, private toastr: ToastrService) {
  }

  ngOnInit() {
    this.fileName='';
    this.fileImageName='';
    this.loading=false;
  }

  @ViewChild('fileInput') fileInput: ElementRef;
  fileAttr = 'Choose File';
  fileImage='Choose image';

  uploadFileEvt(imgFile: any) {
    this.loading=true;
    if (imgFile.target.files && imgFile.target.files[0]) {
      let formData = new FormData();
      formData.append("file",imgFile.target.files[0]);
      this.uploadService.uploadFile(formData).then((res: any) => {
        if (res.code === 1) {
            this.toastr.success("File uploaded successfully!");
            this.fileName = res.document.message;
          this.loading=false;
        } else {
          this.toastr.error(res.message);
          this.loading=false;
        }
      })
      this.fileInput.nativeElement.value = "";
    } else {
      this.fileAttr = 'Choose File';
      this.loading=false;
    }
  }
  uploadImageEvt(imgFile: any) {
    this.loading=true;
    if (imgFile.target.files && imgFile.target.files[0]) {
      let formData = new FormData();
        formData.append("file",imgFile.target.files[0]);
      this.uploadService.uploadImage(formData).then((res: any) => {
        if (res.code === 1) {
            this.toastr.success("Image uploaded successfully!");
            this.fileImageName = res.document.message;
          this.loading=false;
        } else {
          this.toastr.error(res.message);
          this.loading=false;
        }
      })
      this.fileInput.nativeElement.value = "";
    } else {
      this.fileAttr = 'Choose File';
      this.loading=false;
    }
  }

}