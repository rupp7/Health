using Microsoft.AspNetCore.Mvc;
using Health.Api.Models;
using System.Collections.Concurrent;

namespace Health.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private static readonly ConcurrentDictionary<Guid, PatientDto> _store = new();

    [HttpGet]
    public ActionResult<IEnumerable<PatientDto>> GetAll() => Ok(_store.Values);

    [HttpGet("{id:guid}")]
    public ActionResult<PatientDto> Get(Guid id)
    {
        if (!_store.TryGetValue(id, out var p)) return NotFound();
        return Ok(p);
    }

    [HttpGet("search")]
    public ActionResult<IEnumerable<PatientDto>> Search([FromQuery] string? q)
    {
        if (string.IsNullOrWhiteSpace(q)) return Ok(_store.Values);
        var lower = q.Trim().ToLowerInvariant();
        var results = _store.Values.Where(x =>
            ($"{x.FirstName} {x.LastName}".ToLowerInvariant().Contains(lower)) ||
            (x.Address ?? string.Empty).ToLowerInvariant().Contains(lower));
        return Ok(results);
    }

    [HttpPost]
    public ActionResult<PatientDto> Create([FromBody] PatientDto dto)
    {
        dto.Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id;
        _store[dto.Id] = dto;
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public ActionResult Update(Guid id, [FromBody] PatientDto dto)
    {
        if (!_store.ContainsKey(id)) return NotFound();
        dto.Id = id;
        _store[id] = dto;
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public ActionResult Delete(Guid id)
    {
        _store.TryRemove(id, out _);
        return NoContent();
    }
}
