using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers;

[ApiController]
[Route("api/authorcollections")]
public class AuthorCollectionsController : ControllerBase
{
    private readonly ICourseLibraryRepository _courseLibraryRepository;
    private readonly IMapper _mapper;

    public AuthorCollectionsController(
        ICourseLibraryRepository courseLibraryRepository,
        IMapper mapper)
    {
        _courseLibraryRepository = courseLibraryRepository ??
            throw new ArgumentNullException(nameof(courseLibraryRepository));
        _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
    }

    // 1,2,3
    // key1=value1,key2=value2

    [HttpGet("({authorIds})", Name = "GetAuthorCollection")]
    public async Task<ActionResult<IEnumerable<AuthorForCreationDto>>> 
        GetAuthorCollection(
                [ModelBinder(BinderType = typeof(ArrayModelBinder))]
                [FromRoute] IEnumerable<Guid> authorIds)
    {
        var authorEntities = await _courseLibraryRepository
            .GetAuthorsAsync(authorIds);

        // do we have all requested authors?
        if (authorIds.Count() != authorEntities.Count())
        {
            return NotFound();
        }

        // map
        var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
        return Ok(authorsToReturn);
    }


    [HttpPost]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> CreateAuthorCollection(
        IEnumerable<AuthorForCreationDto> authorCollection)
    {
        var authorEntities = _mapper.Map<IEnumerable<Author>>(authorCollection);
        foreach (var author in authorEntities)
        {
            _courseLibraryRepository.AddAuthor(author);
        }
        await _courseLibraryRepository.SaveAsync();

        var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(
            authorEntities);
        var authorIdsAsString = string.Join(",", 
            authorCollectionToReturn.Select(a => a.Id));

        return CreatedAtRoute("GetAuthorCollection",
          new { authorIds = authorIdsAsString },
          authorCollectionToReturn);
    }
}
