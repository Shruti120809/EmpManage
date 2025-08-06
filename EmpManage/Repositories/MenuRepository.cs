using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EmpManage.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Menu> CreateMenuAsync(MenuDTO dto)
        {
            var param = new[]
            {
                new SqlParameter("@Name",dto.Name),
                new SqlParameter("@Route", dto.Route),
                new SqlParameter("@Section",dto.Section),
                new SqlParameter("@Icon",dto.Icon),
                new SqlParameter("@InOrder",dto.InOrder),
                new SqlParameter("@IsActive", dto.IsActive),
                new SqlParameter("@IsDelete", false)

            };

            var data = await _context.Menus
                .FromSqlRaw("Exec sp_CreateMenu @Name, @Route, @Section, @Icon, @Inorder, @IsActive, @IsDelete", param)
                .AsNoTracking()
                .ToListAsync();

            var menudata = data.Select(e => new MenuDTO
            {
                Name = e.Name,
                Route = e.Route,
                Section = e.Section,
                Icon = e.Icon,
                InOrder = e.InOrder
            });

            await _context.SaveChangesAsync();

            return data.FirstOrDefault();
        }

        public async Task<List<Menu>> GetMenuAsync()
        {
            var data = await _context.Menus
                .FromSqlRaw("Exec sp_GetMenu")
                .AsNoTracking()
                .ToListAsync();

            return data;
        }

        public async Task<Menu> GetMenuByIdAsync(int id)
        {
            var para = new[]
            {
                new SqlParameter("@Id", id)
            };
            var data = await _context.Menus
                .FromSqlRaw("Exec sp_GetMenuById @Id", para)
                .AsNoTracking()
                .ToListAsync();

            return data.FirstOrDefault();
        }

        public async Task<Menu> UpdateMenuAsync(MenuDTO dto, int id)
        {
            var para = new[]
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@Name", dto.Name),
                new SqlParameter("@Route", dto.Route),
                new SqlParameter("@Section", dto.Section),
                new SqlParameter("@Icon", dto.Icon),
                new SqlParameter("@InOrder", dto.InOrder),
            };

            var data = await _context.Menus
                .FromSqlRaw("Exec sp_UpdateMenu @Id, @Name, @Route, @Section, @Icon, @InOrder",para)
                .AsNoTracking()
                .ToListAsync();

            return data.FirstOrDefault();
        }

        public async Task<bool> DeleteMenuAsync(int id)
        {
            var para = new SqlParameter("@Id", id);
            var result = await _context.Database.ExecuteSqlRawAsync(
                "Exec sp_SoftDeleteMenu @Id", para);

            return result > 0;
        }


        public async Task<bool> HasMenuAsync(int id)
        {
            var param = new SqlParameter("@Id", id);

            var result = await _context.Menus
                .FromSqlRaw("SELECT * FROM Menus WHERE Id = @Id AND IsDeleted = 0", param)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return result != null;
        }


    }
}
