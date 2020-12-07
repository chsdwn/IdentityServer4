import { Injectable } from '@angular/core';
import * as oidc from 'oidc-client';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  userManager: oidc.UserManager;

  constructor() {
    const config = {
      authority: 'https://localhost:4001',
      client_id: 'js-client',
      redirect_uri: 'http://localhost:4200/callback',
      response_type: 'code',
      scope: 'openid profile email api1.read',
      post_logout_redirect_uri: 'http://localhost:4200',
    };

    this.userManager = new oidc.UserManager(config);
  }
}
