import { Routes } from '@angular/router';
import { HomePage } from './views/home/home-page';
import { productResolver } from './views/home/home-page.resolver';
import { loginRedirectGuard } from './guards/login-redirect.guard';
import { AuthCallback } from './views/auth/auth-callback';
import { ProductPage } from './views/product/product-page';
import { productDetailResolver } from './views/product/product-detail.resolver';
import { CartPage } from './views/cart/cartPage';
import { CheckoutPage } from './views/checkout/checkout-page';
import { StoresPage } from './views/stores/stores-page';
import { storeLocationsResolver } from './views/stores/store-locations.resolver';
import { AdminPage } from './views/admin/admin-page';
import { CreateManufacturerPage } from './views/admin/manufacturers/components/create-manufacturer/create-manufacturer-page';
import { CreateCategoryPage } from './views/create-category/create-category-page';
import { CreateProductPage } from './views/create-product/create-product-page';
import { CreateStorePage } from './views/create-store/create-store-page';
import { Manufacturers } from './views/admin/manufacturers/manufacturers';
import { manufacturersResolver } from './views/admin/manufacturers/manufacturers.resolver';
import { Categories } from './views/admin/categories/categories';
import { categoriesResolver } from './views/admin/categories/categories.resolver';
import { Products } from './views/admin/products/products';
import { productsResolver } from './views/admin/products/products.resolver';
import { Stores } from './views/admin/stores/stores';
import { Store } from './views/admin/store/store';
import { storeLocationResolver } from './views/admin/store/store-location.resolver';
import { storeProductsResolver } from './views/admin/store/store-products.resolver';
import { CheckoutSuccessPage as CheckoutSuccessPage } from './views/checkout-success/checkout-success';

export const routes: Routes = [
  {
    path: 'home',
    component: HomePage,
    resolve: { products: productResolver },
    runGuardsAndResolvers: 'paramsOrQueryParamsChange',
  },
  {
    path: 'stores',
    component: StoresPage,
    resolve: { storeLocations: storeLocationsResolver },
  },
  {
    path: 'product/:productId',
    component: ProductPage,
    resolve: { product: productDetailResolver },
  },
  {
    path: 'cart',
    component: CartPage,
  },
  {
    path: 'checkout',
    component: CheckoutPage,
  },
  {
    path: 'checkout-success',
    component: CheckoutSuccessPage
  },
  {
    path: 'admin',
    component: AdminPage,
    children: [
      { path: '', redirectTo: 'manufacturers', pathMatch: 'full' },
      {
        path: 'store/:id',
        component: Store,
        resolve: {
          storeLocation: storeLocationResolver,
          products: storeProductsResolver,
        },
        runGuardsAndResolvers: 'always',
      },
      { path: 'manufacturers/create', component: CreateManufacturerPage },
      {
        path: 'manufacturers',
        component: Manufacturers,
        resolve: { manufacturers: manufacturersResolver },
        runGuardsAndResolvers: 'paramsOrQueryParamsChange',
      },
      { path: 'categories/create', component: CreateCategoryPage },
      {
        path: 'categories',
        component: Categories,
        resolve: { categories: categoriesResolver },
        runGuardsAndResolvers: 'paramsOrQueryParamsChange',
      },
      { path: 'products/create', component: CreateProductPage },
      {
        path: 'products',
        component: Products,
        resolve: { products: productsResolver },
        runGuardsAndResolvers: 'paramsOrQueryParamsChange',
      },
      { path: 'stores/create', component: CreateStorePage },
      {
        path: 'stores',
        component: Stores,
        resolve: { storeLocations: storeLocationsResolver },
        runGuardsAndResolvers: 'paramsOrQueryParamsChange',
      },
    ],
  },
  {
    path: 'auth/callback',
    component: AuthCallback,
  },
  {
    path: 'login',
    canActivate: [loginRedirectGuard],
    component: HomePage,
  },
  {
    path: '**',
    redirectTo: '/home'
  }
];
