using AutoMapper;
using EmpManage.DTOs;
using EmpManage.Models;

namespace EmpManage.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.Roles, opt =>
                    opt.MapFrom(src => src.EmpRoles!.Select(er => er.Role!.Name).ToList()));

            CreateMap<RegisterDTO, Employee>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.EmpRoles, opt => opt.Ignore());


        }



    }
}
