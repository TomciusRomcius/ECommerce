export default interface UpdateProductRequest {
  productId: number;
  name: string;
  description: string;
  price: number;
  manufacturerId: number;
  categoryId: number;
}
