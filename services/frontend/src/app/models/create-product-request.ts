export default interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  manufacturerId: number;
  categoryId: number;
  files: File[];
}
