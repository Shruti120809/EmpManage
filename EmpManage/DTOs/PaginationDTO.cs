namespace EmpManage.DTOs
{
    public class PaginationDTO<T>
    {
        public List<T>? Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HadPrev => PageIndex > 1;
        public bool HadNext => PageIndex  < TotalPages;

        public PaginationDTO() { }

        public PaginationDTO(List<T>? items, int pageIndex, int totalPages)
        {
            Items = items;
            PageIndex = pageIndex;
            TotalPages = totalPages;
        }
    }
}