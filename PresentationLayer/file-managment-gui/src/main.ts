import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { provideHttpClient,withInterceptorsFromDi } from '@angular/common/http';


bootstrapApplication(App, {
  ...appConfig,
  providers: [
    ...(appConfig.providers || []),  // keep existing providers
    provideHttpClient(withInterceptorsFromDi())  // add HttpClient + DI interceptors
  ]
}).catch((err) => console.error(err));