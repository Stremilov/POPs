using Microsoft.AspNetCore.Mvc;
using POPs.Api.Services;
using POPs.Shared.Dtos;

namespace POPs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly CourseService _courseService;

    public CoursesController(CourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseDto>>> GetAll()
        => Ok(await _courseService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseDto>> GetById(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        return course == null ? NotFound() : Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create([FromBody] CourseCreateRequest request)
    {
        var created = await _courseService.CreateCourseAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CourseDto>> Edit(int id, [FromBody] CourseUpdateRequest request)
    {
        request.Id = id;
        var updated = await _courseService.EditCourseAsync(request);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpPost("attach-group")]
    public async Task<IActionResult> AddGroupToCourse([FromBody] AddGroupToCourseRequest request)
    {
        try
        {
            await _courseService.AddGroupToCourseAsync(request.GroupId, request.CourseId);
            return Ok(new { message = "Группа привязана к курсу." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/promote")]
    public async Task<IActionResult> PromoteStudents(int id)
    {
        try
        {
            var message = await _courseService.PromoteStudentsAsync(id);
            return Ok(new { message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
