import { TestBed }                                  from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient, withInterceptors }      from '@angular/common/http';
import { provideRouter }                                from '@angular/router';
import { HttpClient }                                from '@angular/common/http';
import { AuthService }                               from '../services/auth.service';
import { jwtInterceptor }                            from './jwt.interceptor';

describe('jwtInterceptor', () => {
  let http:     HttpClient;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([jwtInterceptor])),
        provideHttpClientTesting(),
        provideRouter([]),
        AuthService
      ]
    });

    http     = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => { httpMock.verify(); localStorage.clear(); });

  it('should NOT add Authorization header when user is logged out', () => {
    http.get('/api/products').subscribe();
    const req = httpMock.expectOne('/api/products');

    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush([]);
  });

  it('should add Bearer token when user is logged in', () => {
    localStorage.setItem('token', 'test-jwt-123');

    http.get('/api/cart').subscribe();
    const req = httpMock.expectOne('/api/cart');

    expect(req.request.headers.has('Authorization')).toBeTrue();
    expect(req.request.headers.get('Authorization')).toBe('Bearer test-jwt-123');
    req.flush({});
  });
});
