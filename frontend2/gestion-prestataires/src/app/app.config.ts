import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withRouterConfig } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

import { routes } from './app.routes';
import { authInterceptor } from './interceptors/auth.interceptor';
import { silentErrorInterceptor } from './interceptors/silent-error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideCharts(withDefaultRegisterables()),
    provideRouter(
      routes,
      withRouterConfig({ onSameUrlNavigation: 'reload' })
    ),
    provideHttpClient(withInterceptors([authInterceptor, silentErrorInterceptor]))
  ]
};