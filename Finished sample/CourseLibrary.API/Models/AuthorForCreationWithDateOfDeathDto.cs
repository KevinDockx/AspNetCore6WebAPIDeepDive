namespace CourseLibrary.API.Models;

public class AuthorForCreationWithDateOfDeathDto : AuthorForCreationDto
{
    public DateTimeOffset? DateOfDeath { get; set; }
}