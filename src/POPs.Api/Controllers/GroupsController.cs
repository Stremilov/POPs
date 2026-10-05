using Microsoft.AspNetCore.Mvc;
using POPs.Api.Services;
using POPs.Shared.Dtos;

namespace POPs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly GroupService _groupService;

    public GroupsController(GroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GroupDto>>> GetAll()
        => Ok(await _groupService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GroupDto>> GetById(int id)
    {
        var group = await _groupService.GetByIdAsync(id);
        return group == null ? NotFound() : Ok(group);
    }

    [HttpPost]
    public async Task<ActionResult<GroupDto>> Create([FromBody] GroupCreateRequest request)
    {
        try
        {
            var created = await _groupService.CreateGroupAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<GroupDto>> Edit(int id, [FromBody] GroupUpdateRequest request)
    {
        request.Id = id;
        try
        {
            var updated = await _groupService.EditGroupAsync(request);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            return await _groupService.DeleteGroupAsync(id) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/curator")]
    public async Task<ActionResult<GroupDto>> AssignCurator(int id, [FromBody] AssignCuratorRequest request)
    {
        var result = await _groupService.AssignCuratorAsync(id, request.Curator);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> AddStudentToGroup([FromBody] AddStudentToGroupRequest request)
    {
        try
        {
            await _groupService.AddStudentToGroupAsync(request.StudentId, request.GroupId);
            return Ok(new { message = "Студент зачислен в группу." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
