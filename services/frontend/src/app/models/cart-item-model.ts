import ProductModel from './product-model';

export default interface CartItemModel {
  userId: string;
  productId: number;
  storeLocationId: number;
  quantity: number;
  product: ProductModel | null;
}
