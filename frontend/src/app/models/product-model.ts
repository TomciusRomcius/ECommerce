import StoreModel from './store-model';

export default interface ProductModel {
  productId: number;
  name: string;
  description: string;
  price: number;
  manufacturerId: number;
  manufacturerName: string;
  categoryId: number;
  categoryName: string;
  store: StoreModel | null;
  imageUrls: string[];
}
