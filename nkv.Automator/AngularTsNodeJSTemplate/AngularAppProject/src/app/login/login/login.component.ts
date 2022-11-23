import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/api/auth.service';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  data: any;
  constructor(private router: Router, private authService: AuthService,private toastr: ToastrService) { 
    this.data = {};
  }

  ngOnInit() {

  }
  onLogin() {
    this.authService.generateToken(this.data.username,this.data.password).then((token:any) => {
      if (token.code === 1) {
        localStorage.setItem('isLoggedin', 'true');
        this.toastr.success('Login Successful!');
        this.router.navigate(['/dashboard']);
      }else{
        this.toastr.error('Invalid Login');
      }
    })
  }
}
