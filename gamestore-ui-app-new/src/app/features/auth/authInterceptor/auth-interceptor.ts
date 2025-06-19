import { HttpInterceptorFn } from '@angular/common/http';
import {AuthService} from '../../../shared/services/authService/auth-service';
import {inject} from '@angular/core';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService: AuthService = inject(AuthService);
  const token = authService.getToken();

  if (token) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `${token}`,
      },
    });
    return next(authReq);
  }

  return next(req);
};
