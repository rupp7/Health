using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLI.Health.Domain;

namespace CLI.Health.Application
{
    public interface IPatientRepository
    {
        Task AddAsync(Patient p);
        Task<Patient?> GetAsync(Guid id);
        Task<IReadOnlyList<Patient>> ListAsync();
        Task UpdateAsync(Patient p);
        Task DeleteAsync(Guid id);
    }

    public interface IPhysicianRepository
    {
        Task AddAsync(Physician d);
        Task<Physician?> GetAsync(Guid id);
        Task<IReadOnlyList<Physician>> ListAsync();
        Task UpdateAsync(Physician d);
        Task DeleteAsync(Guid id);
    }

    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment a);
        Task<IReadOnlyList<Appointment>> GetForPhysicianOnDateAsync(Guid physicianId, DateOnly date);
        Task<IReadOnlyList<Appointment>> ListAsync();
        Task DeleteAsync(Guid id);
    }
}
