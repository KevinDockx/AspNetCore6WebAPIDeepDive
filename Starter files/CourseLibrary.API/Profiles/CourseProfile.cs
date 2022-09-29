using AutoMapper;

namespace CourseLibrary.API.Profiles;
public class CoursesProfile : Profile
{
    public CoursesProfile()
    {
        CreateMap<Entities.Course, Models.CourseDto>();
        CreateMap<Models.CourseForCreationDto, Entities.Course>();
    }
}