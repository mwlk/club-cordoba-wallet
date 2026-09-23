import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { successInterceptor } from './success.interceptor';

describe('successInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let snackBarOpenSpy: jasmine.Spy;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([successInterceptor])),
        provideHttpClientTesting(),
        { provide: MatSnackBar, useValue: { open: () => {} } }
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    snackBarOpenSpy = spyOn(TestBed.inject(MatSnackBar), 'open');
  });

  afterEach(() => httpMock.verify());

  it('muestra un toast con el mensaje real en un POST 2xx con success:true', () => {
    http.post('/api/credentials', {}).subscribe();

    httpMock.expectOne('/api/credentials').flush({ success: true, message: 'Credencial emitida correctamente', data: {} });

    expect(snackBarOpenSpy).toHaveBeenCalledWith(
      'Credencial emitida correctamente',
      'Cerrar',
      jasmine.objectContaining({ duration: 4000 })
    );
  });

  it('no muestra toast en un GET aunque sea 2xx con success:true', () => {
    http.get('/api/credentials').subscribe();

    httpMock.expectOne('/api/credentials').flush({ success: true, message: 'no debería importar', data: [] });

    expect(snackBarOpenSpy).not.toHaveBeenCalled();
  });

  it('no muestra toast en un POST 2xx con success:false', () => {
    http.post('/api/credentials', {}).subscribe();

    httpMock.expectOne('/api/credentials').flush({ success: false, message: 'algo', data: null });

    expect(snackBarOpenSpy).not.toHaveBeenCalled();
  });
});
