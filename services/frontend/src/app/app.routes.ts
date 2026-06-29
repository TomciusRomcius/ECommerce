import { Routes } from '@angular/router';
import { HomeComponent } from './views/home/home.component';
import { productResolver } from './views/home/home-page.resolver';
import { loginRedirectGuard } from './guards/login-redirect.guard';
import { AuthCallbackComponent } from './views/auth/auth-callback.component';
import { ProductComponent } from './views/product/product.component';
import { productDetailResolver } from './views/product/product-detail.resolver';
import { CartComponent } from './views/cart/cart.component';
import { CheckoutComponent } from './views/checkout/checkout.component';
import { StoresComponent } from './views/stores/stores.component';
import { storeLocationsResolver } from './views/stores/store-locations.resolver';
import { AdminComponent } from './views/admin/admin.component';
import { CreateManufacturerComponent } from './views/admin/manufacturers/components/create-manufacturer/create-manufacturer.component';
import { CreateCategoryComponent } from './views/create-category/create-category.component';
import { CreateProductComponent } from './views/create-product/create-product.component';
import { CreateStoreComponent } from './views/create-store/create-store.component';
import { ManufacturersComponent } from './views/admin/manufacturers/manufacturers.component';
import { manufacturersResolver } from './views/admin/manufacturers/manufacturers.resolver';
import { CategoriesComponent } from './views/admin/categories/categories.component';
import { categoriesResolver } from './views/admin/categories/categories.resolver';
import { ProductsComponent } from './views/admin/products/products.component';
import { productsResolver } from './views/admin/products/products.resolver';
import { StoresComponent as AdminStoresComponent } from './views/admin/stores/stores.component';
import { StoreComponent } from './views/admin/store/store.component';
import { storeLocationResolver } from './views/admin/store/store-location.resolver';
import { storeProductsResolver } from './views/admin/store/store-products.resolver';
import { CheckoutSuccessComponent } from './views/checkout-success/checkout-success.component';

export const routes: Routes = [
  {
    path: 'home',
    component: HomeComponent,
    resolve: { products: productResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  {
    path: 'stores',
    component: StoresComponent,
    resolve: { storeLocations: storeLocationsResolver },
  },
  {
    path: 'product/:productId',
    component: ProductComponent,
    resolve: { product: productDetailResolver },
  },
  {
    path: 'cart',
    component: CartComponent,
  },
  {
    path: 'checkout',
    component: CheckoutComponent,
  },
  {
    path: 'checkout-success',
    component: CheckoutSuccessComponent
  },
  {
    path: 'admin',
    component: AdminComponent,
    children: [
      { path: '', redirectTo: 'manufacturers', pathMatch: 'full' },
      {
        path: 'store/:id',
        component: StoreComponent,
        resolve: {
          storeLocation: storeLocationResolver,
          products: storeProductsResolver,
        },
        runGuardsAndResolvers: 'always',
      },
      { path: 'manufacturers/create', component: CreateManufacturerComponent },
      {
        path: 'manufacturers',
        component: ManufacturersComponent,
        resolve: { manufacturers: manufacturersResolver },
        runGuardsAndResolvers: 'paramsOrQueryParamsChange',
      },
      { path: 'categories/create', component: CreateCategoryComponent },
      {
        path: 'categories',
        component: CategoriesComponent,
        resolve: { categories: categoriesResolver },
        runGuardsAndResolvers: 'paramsOrQueryParamsChange',
      },
      { path: 'products/create', component: CreateProductComponent },
      {
        path: 'products',
        component: ProductsComponent,
        resolve: { products: productsResolver },
        runGuardsAndResolvers: 'paramsOrQueryParamsChange',
      },
      { path: 'stores/create', component: CreateStoreComponent },
      {
        path: 'stores',
        component: AdminStoresComponent,
        resolve: { storeLocations: storeLocationsResolver },
        runGuardsAndResolvers: 'paramsOrQueryParamsChange',
      },
    ],
  },
  {
    path: 'auth/callback',
    component: AuthCallbackComponent,
  },
  {
    path: 'login',
    canActivate: [loginRedirectGuard],
    component: HomeComponent,
  },
  {
    path: '**',
    redirectTo: '/home'
  }
];
