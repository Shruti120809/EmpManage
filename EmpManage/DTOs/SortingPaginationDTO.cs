namespace EmpManage.DTOs
{
    public class SortingPaginationDTO
    {
        public string? Sorting {  get; set; }
        public string? Search {  get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IsAsc = true;
    }
}
