using Microsoft.AspNetCore.Mvc;
using Health.Api.Models;
using Health.Api.Database;

namespace Health.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly PatientFilebase _filebase = PatientFilebase.Current;

    [HttpGet]
    public ActionResult<IEnumerable<PatientDto>> GetAll() => Ok(_filebase.GetAllPatients());

    [HttpGet("{id:guid}")]
    public ActionResult<PatientDto> Get(Guid id)
    {
        var p = _filebase.GetPatient(id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpGet("search")]
    public ActionResult<IEnumerable<PatientDto>> Search([FromQuery] string? q)
    {
        var all = _filebase.GetAllPatients();
        if (string.IsNullOrWhiteSpace(q)) return Ok(all);
        var lower = q.Trim().ToLowerInvariant();
        var results = all.Where(x =>
            ($"{x.FirstName} {x.LastName}".ToLowerInvariant().Contains(lower)) ||
            (x.Address ?? string.Empty).ToLowerInvariant().Contains(lower));
        return Ok(results);
    }

    [HttpPost]
    public ActionResult<PatientDto> Create([FromBody] PatientDto dto)
    {
        dto.Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id;
        var created = _filebase.AddOrUpdate(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public ActionResult Update(Guid id, [FromBody] PatientDto dto)
    {
        var existing = _filebase.GetPatient(id);
        if (existing == null) return NotFound();
        dto.Id = id;
        _filebase.AddOrUpdate(dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public ActionResult Delete(Guid id)
    {
        _filebase.DeletePatient(id);
        return NoContent();
    }
}
