using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLI.Health.Domain;

namespace CLI.Health.Application
{
    public sealed class PatientService
    {
        private readonly IPatientRepository _repo;
        public PatientService(IPatientRepository repo) => _repo = repo;

        public Task CreateAsync(string first, string last, DateOnly dob, string address, Race race, Gender gender)
            => _repo.AddAsync(new Patient { FirstName = first, LastName = last, BirthDate = dob, Address = address, Race = race, Gender = gender });

        public Task<IReadOnlyList<Patient>> ListAsync() => _repo.ListAsync();

        public Task<Patient?> GetAsync(Guid id) => _repo.GetAsync(id);

        public Task UpdateAsync(Patient p) => _repo.UpdateAsync(p);

        public Task DeleteAsync(Guid id) => _repo.DeleteAsync(id);
    }

    public sealed class PhysicianService
    {
        private readonly IPhysicianRepository _repo;
        public PhysicianService(IPhysicianRepository repo) => _repo = repo;

        public Task CreateAsync(string first, string last, string license, DateOnly grad, IEnumerable<string> specs)
            => _repo.AddAsync(new Physician { FirstName = first, LastName = last, LicenseNumber = license, GraduationDate = grad, Specializations = specs.ToList() });

        public Task<IReadOnlyList<Physician>> ListAsync() => _repo.ListAsync();

        public Task<Physician?> GetAsync(Guid id) => _repo.GetAsync(id);

        public Task UpdateAsync(Physician p) => _repo.UpdateAsync(p);

        public Task DeleteAsync(Guid id) => _repo.DeleteAsync(id);
    }

    public sealed class AppointmentService
    {
        private static readonly TimeOnly Open = new(8, 0);
        private static readonly TimeOnly Close = new(17, 0);

        private readonly IAppointmentRepository _appointments;
        private readonly IPatientRepository _patients;
        private readonly IPhysicianRepository _physicians;

        public AppointmentService(IAppointmentRepository appts, IPatientRepository pats, IPhysicianRepository docs)
        { _appointments = appts; _patients = pats; _physicians = docs; }

        public async Task<Appointment> ScheduleAsync(Guid patientId, Guid physicianId, DateOnly date, TimeOnly start, TimeSpan duration)
        {
            // must exist
            if (await _patients.GetAsync(patientId) is null) throw new InvalidOperationException("Patient not found.");
            if (await _physicians.GetAsync(physicianId) is null) throw new InvalidOperationException("Physician not found.");

            // weekday only
            var dow = date.ToDateTime(TimeOnly.MinValue).DayOfWeek;
            if (dow is DayOfWeek.Saturday or DayOfWeek.Sunday)
                throw new InvalidOperationException("Appointments allowed Monday–Friday only.");

            // within hours & positive duration
            var end = start.Add(duration);
            if (start < Open || end > Close || duration <= TimeSpan.Zero)
                throw new InvalidOperationException("Must be within 08:00–17:00 and positive duration.");

            // no double-booking for physician
            var sameDay = await _appointments.GetForPhysicianOnDateAsync(physicianId, date);
            bool overlap = sameDay.Any(a => a.Start < end && start < a.End);
            if (overlap) throw new InvalidOperationException("Physician is already booked during this time.");

            var appt = new Appointment { PatientId = patientId, PhysicianId = physicianId, Date = date, Start = start, Duration = duration };
            await _appointments.AddAsync(appt);
            return appt;
        }

        public Task<IReadOnlyList<Appointment>> ListAsync() => _appointments.ListAsync();

        public Task DeleteAsync(Guid id) => _appointments.DeleteAsync(id);
    }
}
