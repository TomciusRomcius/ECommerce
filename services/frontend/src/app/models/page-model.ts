export default interface PageModel<T> {
  data: T[];
  totalCount: number;
  pageSize: number;
  hasNextPage: boolean;
  hasPrevPage: boolean;
}

export function emptyPage<T>(): PageModel<T> {
  return {
    data: [],
    totalCount: 0,
    pageSize: 0,
    hasPrevPage: false,
    hasNextPage: false,
  };
}
