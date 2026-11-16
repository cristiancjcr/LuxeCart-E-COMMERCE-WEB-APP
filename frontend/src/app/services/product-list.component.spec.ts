import { ComponentFixture, TestBed, fakeAsync, flush, tick } from '@angular/core/testing';
import { provideHttpClient }                            from '@angular/common/http';
import { provideHttpClientTesting }                     from '@angular/common/http/testing';
import { provideRouter, Router }                                from '@angular/router';
import { of, throwError }                               from 'rxjs';
import { ProductListComponent }                         from '../components/product-list/product-list.component';
import { ProductService }                               from './product-order.service';
import { CartService }                                  from './cart.service';
import { AuthService }                                  from './auth.service';
import { ToastService }                                 from './toast.service';
import { Product }                                      from '../models/models';

describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let fixture:   ComponentFixture<ProductListComponent>;
  let mockProductSvc: jasmine.SpyObj<ProductService>;
  let mockCartSvc:    jasmine.SpyObj<CartService>;
  let mockAuthSvc:    jasmine.SpyObj<AuthService>;
  let toast:          ToastService;
  let router:         Router;

  const MOCK_PRODUCTS: Product[] = [
    { id: 1, name: 'Blue Top',   description: 'Desc', price: 500, category: "Women's Tops",   imageUrl: 'img.jpg', stock: 50 },
    { id: 2, name: 'Men Tshirt', description: 'Desc', price: 400, category: "Men's T-shirts", imageUrl: 'img.jpg', stock: 100 }
  ];

  // The API returns a paged envelope, not a plain array
  const paged = (items: Product[]) => ({
    items, totalItems: items.length, page: 1, pageSize: 12,
    totalPages: 1, hasNext: false, hasPrevious: false
  });

  beforeEach(async () => {
    mockProductSvc = jasmine.createSpyObj('ProductService', ['getAllPaged', 'getCategories']);
    mockCartSvc    = jasmine.createSpyObj('CartService',    ['addToCart', 'loadCart']);
    mockAuthSvc    = jasmine.createSpyObj('AuthService',    ['isLoggedIn']);

    mockProductSvc.getAllPaged.and.returnValue(of(paged(MOCK_PRODUCTS)));
    mockProductSvc.getCategories.and.returnValue(of(["Women's Tops", "Men's T-shirts"]));
    mockAuthSvc.isLoggedIn.and.returnValue(true);

    await TestBed.configureTestingModule({
      imports: [ProductListComponent],
      providers: [
        provideHttpClient(), provideHttpClientTesting(), provideRouter([]),
        { provide: ProductService, useValue: mockProductSvc },
        { provide: CartService,    useValue: mockCartSvc    },
        { provide: AuthService,    useValue: mockAuthSvc    }
      ]
    }).compileComponents();

    toast     = TestBed.inject(ToastService);
    router    = TestBed.inject(Router);
    spyOn(router, 'navigate').and.resolveTo(true);
    fixture   = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch the first page of products on init', () => {
    expect(mockProductSvc.getAllPaged).toHaveBeenCalledOnceWith(undefined, undefined, 1, 12);
    expect(component.products().length).toBe(2);
  });

  it('should fetch categories on init', () => {
    expect(mockProductSvc.getCategories).toHaveBeenCalled();
    expect(component.categories().length).toBe(2);
  });

  it('should set loading to false after products load', () => {
    expect(component.loading()).toBeFalse();
  });

  it('should filter by category when selectCategory is called', fakeAsync(() => {
    mockProductSvc.getAllPaged.and.returnValue(of(paged([MOCK_PRODUCTS[0]])));
    component.selectCategory("Women's Tops");
    tick(300); // fade-out delay (220 ms) + grid fade-in (30 ms)
    expect(mockProductSvc.getAllPaged).toHaveBeenCalledWith("Women's Tops", undefined, 1, 12);
    expect(component.selectedCategory).toBe("Women's Tops");
    expect(component.products().length).toBe(1);
  }));

  it('should reset category filter when null is passed', fakeAsync(() => {
    component.selectedCategory = "Women's Tops";
    component.selectCategory(null);
    tick(300);
    expect(component.selectedCategory).toBeNull();
  }));

  it('addToCart should call CartService.addToCart when logged in', fakeAsync(() => {
    mockCartSvc.addToCart.and.returnValue(of({ items: [], total: 0 }));
    component.addToCart(MOCK_PRODUCTS[0]);
    expect(mockCartSvc.addToCart).toHaveBeenCalledWith(1, 1);
    flush(); // let the toast and "added" timers finish
  }));

  it('addToCart should NOT call CartService when logged out', () => {
    mockAuthSvc.isLoggedIn.and.returnValue(false);
    component.addToCart(MOCK_PRODUCTS[0]);
    expect(mockCartSvc.addToCart).not.toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should set addingId while add-to-cart request is in progress', fakeAsync(() => {
    mockCartSvc.addToCart.and.returnValue(of({ items: [], total: 0 }));
    component.addToCart(MOCK_PRODUCTS[0]);
    expect(component.addingId()).toBe(1);
    flush();
    expect(component.addingId()).toBeNull();
  }));

  it('should handle product service error gracefully', () => {
    spyOn(toast, 'error');
    mockProductSvc.getAllPaged.and.returnValue(throwError(() => new Error('Network error')));
    component.loadProducts();
    expect(component.loading()).toBeFalse();
    expect(toast.error).toHaveBeenCalled();
  });
});
