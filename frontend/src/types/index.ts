export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  userId: string;
  username: string;
  email: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: Record<string, string[]>;
}

export interface ProfileGroup {
  id: string;
  name: string;
  description?: string;
  templatePath?: string;
  sortOrder: number;
  isActive: boolean;
  createdAt: string;
}

export interface TokenBalance {
  userId: string;
  currentToken: number;
  updatedAt: string;
}

export interface TokenPackage {
  id: string;
  name: string;
  tokenAmount: number;
  pricePerToken: number;
  totalPrice: number;
  sortOrder: number;
}

export interface TokenTransaction {
  id: string;
  type: string;
  amount: number;
  description?: string;
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
