using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EmpManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class MenuController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("CreateMenu")]
        public async Task<ResponseDTO<Menu>> CreateMenuByAdmin([FromBody] MenuDTO menuDTO)
        {
            Log.Information("CreateMenu called with {@MenuDTO}", menuDTO);

            if (!ModelState.IsValid)
            {
                Log.Warning("CreateMenu failed due to invalid model state");
                return new ResponseDTO<Menu>(400, ResponseHelper.ValidationError(ModelState), null);
            }

            var createMenu = await _unitOfWork.Menu.CreateMenuAsync(menuDTO);
            if (createMenu == null)
            {
                Log.Error("CreateMenu failed internally for {@MenuDTO}", menuDTO);
                return new ResponseDTO<Menu>(500, ResponseHelper.InternalError("Menu"), null);
            }

            Log.Information("Menu created successfully: {MenuId}", createMenu.Id);
            return new ResponseDTO<Menu>(200, ResponseHelper.Success("created", "Menu"), createMenu);
        }

        [HttpGet("GetMenus")]
        public async Task<ResponseDTO<List<Menu>>> GetAllMenu()
        {
            Log.Information("GetMenus called");

            var menu = await _unitOfWork.Menu.GetMenuAsync();
            if (menu == null || !menu.Any())
            {
                Log.Warning("No menus found");
                return new ResponseDTO<List<Menu>>(404, ResponseHelper.NotFound("Menu"), null);
            }

            Log.Information("Fetched {Count} menus", menu.Count);
            return new ResponseDTO<List<Menu>>(200, ResponseHelper.Success("fetched", "Menu"), menu);
        }

        [HttpGet("GetMenu/{id}")]
        public async Task<ResponseDTO<Menu>> GetMenuById(int id)
        {
            Log.Information("GetMenuById called for MenuId: {Id}", id);

            var menu = await _unitOfWork.Menu.GetMenuByIdAsync(id);
            if (menu == null)
            {
                Log.Warning("Menu not found with Id: {Id}", id);
                return new ResponseDTO<Menu>(404, ResponseHelper.NotFound("Menu"), null);
            }

            Log.Information("Menu fetched successfully with Id: {Id}", id);
            return new ResponseDTO<Menu>(200, ResponseHelper.Success("fetched", "Menu"), menu);
        }

        [HttpPut("UpdateMenu/{id}")]
        public async Task<ResponseDTO<Menu>> UpdateMenu([FromBody] MenuDTO dto, int id)
        {
            Log.Information("UpdateMenu called for MenuId: {Id} with {@MenuDTO}", id, dto);

            var menu = await _unitOfWork.Menu.UpdateMenuAsync(dto, id);
            if (menu == null)
            {
                Log.Warning("Menu not found for update with Id: {Id}", id);
                return new ResponseDTO<Menu>(404, ResponseHelper.NotFound("Menu"), null);
            }

            Log.Information("Menu updated successfully with Id: {Id}", id);
            return new ResponseDTO<Menu>(200, ResponseHelper.Success("updated", "Menu"), menu);
        }

        [HttpDelete("DeleteMenu/{id}")]
        public async Task<ResponseDTO<bool>> DeleteMenu(int id)
        {
            Log.Information("DeleteMenu called for MenuId: {Id}", id);

            var menu = await _unitOfWork.Menu.GetMenuByIdAsync(id);
            if (menu == null)
            {
                Log.Warning("Menu not found for deletion with Id: {Id}", id);
                return new ResponseDTO<bool>(404, ResponseHelper.NotFound("Menu"), false);
            }

            var result = await _unitOfWork.Menu.DeleteMenuAsync(id);
            Log.Information("Menu deleted successfully with Id: {Id}", id);
            return new ResponseDTO<bool>(200, ResponseHelper.Success("deleted", "Menu"), true);
        }
    }
}
