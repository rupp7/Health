using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLI.Health.Application;
using CLI.Health.Domain;

namespace CLI.Health.Infrastructure
{
    public sealed class InMemoryPatientRepository : IPatientRepository
    {
        private readonly List<Patient> _items = new();
        public Task AddAsync(Patient p) { _items.Add(p); return Task.CompletedTask; }
        public Task<Patient?> GetAsync(Guid id) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
        public Task<IReadOnlyList<Patient>> ListAsync() => Task.FromResult((IReadOnlyList<Patient>)_items);
        public Task UpdateAsync(Patient p)
        {
            var idx = _items.FindIndex(x => x.Id == p.Id);
            if (idx >= 0) _items[idx] = p;
            return Task.CompletedTask;
        }
        public Task DeleteAsync(Guid id) { _items.RemoveAll(x => x.Id == id); return Task.CompletedTask; }
    }

    public sealed class InMemoryPhysicianRepository : IPhysicianRepository
    {
        private readonly List<Physician> _items = new();
        public Task AddAsync(Physician d) { _items.Add(d); return Task.CompletedTask; }
        public Task<Physician?> GetAsync(Guid id) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
        public Task<IReadOnlyList<Physician>> ListAsync() => Task.FromResult((IReadOnlyList<Physician>)_items);
        public Task UpdateAsync(Physician d)
        {
            var idx = _items.FindIndex(x => x.Id == d.Id);
            if (idx >= 0) _items[idx] = d;
            return Task.CompletedTask;
        }
        public Task DeleteAsync(Guid id) { _items.RemoveAll(x => x.Id == id); return Task.CompletedTask; }
    }

    public sealed class InMemoryAppointmentRepository : IAppointmentRepository
    {
        private readonly List<Appointment> _items = new();
        public Task AddAsync(Appointment a) { _items.Add(a); return Task.CompletedTask; }
        public Task<IReadOnlyList<Appointment>> GetForPhysicianOnDateAsync(Guid physicianId, DateOnly date)
            => Task.FromResult((IReadOnlyList<Appointment>)_items.Where(x => x.PhysicianId == physicianId && x.Date == date).ToList());

        public Task<IReadOnlyList<Appointment>> ListAsync() => Task.FromResult((IReadOnlyList<Appointment>)_items);

        public Task DeleteAsync(Guid id) { _items.RemoveAll(x => x.Id == id); return Task.CompletedTask; }
    }
}
