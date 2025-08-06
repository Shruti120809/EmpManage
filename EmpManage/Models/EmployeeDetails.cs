using Microsoft.EntityFrameworkCore;

[Keyless]
public class EmployeeDetails
{
    public int EmployeeId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? RoleName { get; set; }
    public string? MenuJson { get; set; }    // used in grouping

    public int TotalRecords { get; set; }    // Required for pagination
}
