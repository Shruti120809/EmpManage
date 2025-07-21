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
                    opt.MapFrom(src => src.EmpRoles!
                        .Select(er => er.Role!.Name)
                        .Distinct()
                        .ToList()))
                .ForMember(dest => dest.Menus, opt =>
                    opt.MapFrom(src => src.EmpRoles!
                        .SelectMany(er => er.Role!.RoleMenuPermissions!)
                        .Select(rp => rp.Menu!.Name)
                        .Distinct()
                        .ToList()));

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
