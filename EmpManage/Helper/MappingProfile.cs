using AutoMapper;
using EmpManage.DTOs;
using EmpManage.Models;

namespace EmpManage.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDTO>()
             .ForMember(dest => dest.Roles, opt =>
                 opt.MapFrom(src => src.EmpRoles != null
                     ? src.EmpRoles
                         .Where(er => er.Role != null)
                         .Select(er => er.Role!.Name)
                         .Distinct()
                         .ToList()
                     : new List<string>()))
             .ForMember(dest => dest.Menus, opt =>
                 opt.MapFrom(src => src.EmpRoles != null
                     ? src.EmpRoles
                         .Where(er => er.Role != null && er.Role.RoleMenuPermissions != null)
                         .SelectMany(er => er.Role!.RoleMenuPermissions!
                             .Where(rmp => rmp.Menu != null)
                             .Select(rmp => new MenuPermissionDTO
                             {
                                 MenuId = rmp.Menu!.Id,
                                 MenuName = rmp.Menu!.Name
                             }))
                         .GroupBy(m => m.MenuId)   // prevent duplicates
                         .Select(g => g.First())
                         .ToList()
                     : new List<MenuPermissionDTO>()));

            CreateMap<Employee, DeleteDTO>()
                .ForMember(dest => dest.UpdatedAt, opt =>
                    opt.MapFrom(src => src.UpdatedAt.HasValue
                        ? src.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        : null));

            CreateMap<RegisterDTO, Employee>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLower()))
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            CreateMap<Employee, LoginResponseDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
