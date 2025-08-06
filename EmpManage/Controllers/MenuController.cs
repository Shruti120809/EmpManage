using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            if (!ModelState.IsValid)
                return new ResponseDTO<Menu>(
                    400,
                    ResponseHelper.ValidationError(ModelState),
                    null);

            var createMenu = await _unitOfWork.Menu.CreateMenuAsync(menuDTO);
            if (createMenu == null)
                return new ResponseDTO<Menu>(
                    500,
                    ResponseHelper.InternalError("Menu"),
                    null);

            return new ResponseDTO<Menu>(
                200,
                ResponseHelper.Success("created", "Menu"),
                createMenu);
        }

        [HttpGet("GetMenus")]
        public async Task<ResponseDTO<List<Menu>>> GetAllMenu()
        {
            var menu = await _unitOfWork.Menu.GetMenuAsync();
            if (menu == null || !menu.Any())
                return new ResponseDTO<List<Menu>>(
                    404,
                    ResponseHelper.NotFound("Menu"),
                    null);

            return new ResponseDTO<List<Menu>>(
                200,
                ResponseHelper.Success("fetched", "Menu"),
                menu);
        }

        [HttpGet("GetMenu/{id}")]
        public async Task<ResponseDTO<Menu>> GetMenuById(int id)
        {
            var menu = await _unitOfWork.Menu.GetMenuByIdAsync(id);
            if (menu == null)
                return new ResponseDTO<Menu>(
                    404,
                    ResponseHelper.NotFound("Menu"),
                    null);

            return new ResponseDTO<Menu>(
                200,
                ResponseHelper.Success("fetched", "Menu"),
                menu);
        }

        [HttpPut("UpdateMenu/{id}")]
        public async Task<ResponseDTO<Menu>> UpdateMenu([FromBody] MenuDTO dto, int id)
        {
            var menu = await _unitOfWork.Menu.UpdateMenuAsync(dto, id);
            if (menu == null)
                return new ResponseDTO<Menu>(
                    404,
                    ResponseHelper.NotFound("Menu"),
                    null);

            return new ResponseDTO<Menu>(
                200,
                ResponseHelper.Success("updated", "Menu"),
                menu);
        }

        [HttpDelete("DeleteMenu/{id}")]
        public async Task<ResponseDTO<bool>> DeleteMenu(int id)
        {
            var menu = await _unitOfWork.Menu.GetMenuByIdAsync(id);
            if (menu == null)
            {
                return new ResponseDTO<bool>(
                    404,
                    ResponseHelper.NotFound("Menu"),
                    false
                );
            }

            var result = await _unitOfWork.Menu.DeleteMenuAsync(id);
            return new ResponseDTO<bool>(
                200,
                ResponseHelper.Success("deleted", "Menu"),
                true
            );
        }

    }
}
