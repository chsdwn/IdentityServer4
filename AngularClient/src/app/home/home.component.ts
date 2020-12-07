import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  status = '';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.userManager.getUser().then((user) => {
      if (user) {
        console.log(user);
        this.status = 'Hoşgeldiniz';
      } else this.status = 'Giriş yapınız';
    });
  }

  login(): void {
    this.authService.userManager.signinRedirect();
  }

  logout(): void {
    this.authService.userManager.signoutRedirect();
  }
}
