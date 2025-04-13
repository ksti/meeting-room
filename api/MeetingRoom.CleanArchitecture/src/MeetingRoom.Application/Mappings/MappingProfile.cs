using AutoMapper;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Domain.Aggregates.MeetingAggregate;
using MeetingRoom.Domain.Aggregates.RoomAggregate;
using MeetingRoom.Domain.Aggregates.UserAggregate;
using MeetingRoom.Domain.Enums;

namespace MeetingRoom.Application.Mappings;

/// <summary>
/// AutoMapper映射配置
/// 定义领域对象和DTO之间的映射规则
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 用户映射
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.GetDisplayName()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.GetDisplayName()))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name != null ? src.Name.FirstName : null))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name != null ? src.Name.LastName : null));
        
        // 会议室映射
        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.GetDisplayName()))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable()));
        
        // 会议映射
        CreateMap<Meeting, MeetingDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.GetDisplayName()))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.TimeRange.Start))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.TimeRange.End))
            .ForMember(dest => dest.Attendees, opt => opt.Ignore()); // 参与者需要单独处理
        
        // 用户到参与者的映射
        CreateMap<User, AttendeeDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => 
                src.Name != null ? src.Name.FullName : null))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => 
                src.Email != null ? src.Email.Value : null));
    }
}
