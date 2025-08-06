namespace EmpManage.DTOs
{
    public class SortingPaginationDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }    // Name or Email
        public string? SortBy { get; set; }        // "name" or "email"
        public bool IsAscending { get; set; } = true;

    }
}
