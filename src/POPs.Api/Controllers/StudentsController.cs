using Microsoft.AspNetCore.Mvc;
using POPs.Api.Services;
using POPs.Shared.Dtos;

namespace POPs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly StudentService _studentService;

    public StudentsController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StudentDto>>> GetAll([FromQuery] string? search)
        => Ok(await _studentService.GetAllAsync(search));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetById(int id)
    {
        var student = await _studentService.FindStudentAsync(id);
        return student == null ? NotFound() : Ok(student);
    }

    [HttpGet("by-group/{groupId:int}")]
    public async Task<ActionResult<List<StudentDto>>> GetByGroup(int groupId)
        => Ok(await _studentService.GetStudentsByGroupAsync(groupId));

    [HttpPost]
    public async Task<ActionResult<StudentDto>> Add([FromBody] StudentCreateRequest request)
    {
        try
        {
            var created = await _studentService.AddStudentAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StudentDto>> Edit(int id, [FromBody] StudentUpdateRequest request)
    {
        request.Id = id;
        try
        {
            var updated = await _studentService.EditStudentAsync(request);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _studentService.DeleteStudentAsync(id) ? NoContent() : NotFound();

    [HttpPost("{id:int}/expel")]
    public async Task<ActionResult<StudentDto>> Expel(int id, [FromBody] ExpelRequest request)
    {
        var result = await _studentService.ExpelStudentAsync(id, request.Reason);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>Прецедент «Внесение персональных данных» (студент).</summary>
    [HttpPut("{id:int}/personal")]
    public async Task<ActionResult<StudentDto>> UpdatePersonal(int id, [FromBody] PersonalDataRequest request)
    {
        var result = await _studentService.UpdatePersonalDataAsync(id, request);
        return result == null ? NotFound() : Ok(result);
    }
}
