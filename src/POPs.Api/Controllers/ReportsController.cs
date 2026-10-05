using Microsoft.AspNetCore.Mvc;
using POPs.Api.Services;
using POPs.Shared.Dtos;

namespace POPs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("contingent")]
    public async Task<ActionResult<List<ContingentItemDto>>> GetContingent()
        => Ok(await _reportService.GetContingentAsync());

    [HttpGet("overview")]
    public async Task<ActionResult<OverviewStatisticsDto>> GetOverview()
        => Ok(await _reportService.GetOverviewAsync());

    [HttpGet("by-status")]
    public async Task<ActionResult<object>> CountByStatus([FromQuery] string status)
        => Ok(new { status, count = await _reportService.CountByStatusAsync(status) });

    [HttpGet("by-group/{groupId:int}")]
    public async Task<ActionResult<object>> CountByGroup(int groupId)
        => Ok(new { groupId, count = await _reportService.CountByGroupAsync(groupId) });

    [HttpGet("by-course/{courseId:int}")]
    public async Task<ActionResult<object>> CountByCourse(int courseId)
        => Ok(new { courseId, count = await _reportService.CountByCourseAsync(courseId) });

    /// <summary>Отчёт по группе (диаграмма активности).</summary>
    [HttpGet("group/{groupId:int}")]
    public async Task<ActionResult<GroupReportDto>> GetGroupReport(int groupId)
    {
        var report = await _reportService.GetGroupStatisticsAsync(groupId);
        return report == null ? NotFound() : Ok(report);
    }

    [HttpGet("course/{courseId:int}")]
    public async Task<ActionResult<CourseStatisticsDto>> GetCourseReport(int courseId)
    {
        var report = await _reportService.GetCourseStatisticsAsync(courseId);
        return report == null ? NotFound() : Ok(report);
    }
}
