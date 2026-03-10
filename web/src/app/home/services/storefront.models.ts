export interface Product {
  id: string;
  title: string;
  slug: string;
  categorySlug?: string;
  categoryName?: string;
  imageUrl: string;
  priceFrom: number;
  description?: string;
}

export interface Review {
  rating: number;
  title: string;
  message: string;
  reviewer: string;
  date: string;
}
