using CLI.Health.Application;
using CLI.Health.Domain;
using CLI.Health.Infrastructure;

Console.WriteLine("Welcome to Health Management CLi");

var patientRepo = new InMemoryPatientRepository();
var physicianRepo = new InMemoryPhysicianRepository();
var apptRepo = new InMemoryAppointmentRepository();

var patientSvc = new PatientService(patientRepo);
var physicianSvc = new PhysicianService(physicianRepo);
var apptSvc = new AppointmentService(apptRepo, patientRepo, physicianRepo);

while (true)
{
    Console.WriteLine("\n1) Create Patient");
    Console.WriteLine("2) Create Physician");
    Console.WriteLine("3) Add Medical Note to Patient");
    Console.WriteLine("4) Schedule Appointment");
    Console.WriteLine("5) List Patients");
    Console.WriteLine("6) List Physicians");
    Console.WriteLine("0) Exit");
    Console.Write("Select: ");
    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                Console.Write("First name: "); var pf = Console.ReadLine()!;
                Console.Write("Last name: "); var pl = Console.ReadLine()!;
                Console.Write("Birthdate (MM/dd/yyyy): "); var dob = DateOnly.Parse(Console.ReadLine()!);
                Console.Write("Address: "); var addr = Console.ReadLine()!;
                var race = (Race)ReadInt("Race (0=Unknown,1=White,2=Black,3=Asian,4=NativeAmerican,5=PacificIslander,6=Other,7=PreferNotToSay): ");
                var gen = (Gender)ReadInt("Gender (0=Unknown,1=Male,2=Female,3=NonBinary,4=Other): ");
                await patientSvc.CreateAsync(pf, pl, dob, addr, race, gen);
                Console.WriteLine("Patient created.");
                break;

            case "2":
                Console.Write("First name: "); var df = Console.ReadLine()!;
                Console.Write("Last name: "); var dl = Console.ReadLine()!;
                Console.Write("License #: "); var lic = Console.ReadLine()!;
                Console.Write("Graduation date (MM/dd/yyyy): "); var grad = DateOnly.Parse(Console.ReadLine()!);
                Console.Write("Specializations (comma separated text): ");
                var specs = Console.ReadLine()!.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                await physicianSvc.CreateAsync(df, dl, lic, grad, specs);
                Console.WriteLine("Physician created.");
                break;

            case "3":
                var patients = await patientSvc.ListAsync();
                if (patients.Count == 0) { Console.WriteLine("No patients."); break; }
                int pIdx = ReadChoice("Patients", patients.Select(p => $"{p.LastName}, {p.FirstName}").ToList());
                var patient = patients[pIdx];

                Console.Write("Diagnoses: "); var dx = Console.ReadLine();
                Console.Write("Prescriptions: "); var rx = Console.ReadLine();
                Console.Write("Free text: "); var noteText = Console.ReadLine();
                patient.AddNote(new MedicalNote { PatientId = patient.Id, Diagnoses = dx, Prescriptions = rx, FreeText = noteText });
                Console.WriteLine("Note added.");
                break;

            case "4":
                var pats = await patientSvc.ListAsync();
                var docs = await physicianSvc.ListAsync();
                if (pats.Count == 0 || docs.Count == 0) { Console.WriteLine("Need at least one patient and one physician."); break; }

                int pI = ReadChoice("Patients", pats.Select(p => $"{p.LastName}, {p.FirstName}").ToList());
                int dI = ReadChoice("Physicians", docs.Select(d => $"Dr. {d.LastName} ({d.LicenseNumber})").ToList());

                var date = ReadDate("Date (MM/dd/yyyy): ");
                var start = ReadTime("Start (HH:mm 24h): ");
                var mins = ReadInt("Duration minutes: ");

                var appt = await apptSvc.ScheduleAsync(pats[pI].Id, docs[dI].Id, date, start, TimeSpan.FromMinutes(mins));
                Console.WriteLine($"Appointment booked {appt.Start}–{appt.End} on {appt.Date:MM/dd/yyyy}.");
                break;

            case "5":
                foreach (var p in await patientSvc.ListAsync())
                    Console.WriteLine($"{p.Id} | {p.LastName}, {p.FirstName} | DOB {p.BirthDate:MM/dd/yyyy} | {p.Address} | Race {p.Race} | Gender {p.Gender} | Notes {p.Notes.Count}");
                break;

            case "6":
                foreach (var d in await physicianSvc.ListAsync())
                    Console.WriteLine($"{d.Id} | Dr. {d.LastName}, {d.FirstName} | Lic {d.LicenseNumber} | Grad {d.GraduationDate:MM/dd/yyyy} | Specs: {string.Join(", ", d.Specializations)}");
                break;

            case "0": return;
            default: Console.WriteLine("Invalid choice."); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static int ReadInt(string prompt)
{
    Console.Write(prompt);
    return int.Parse(Console.ReadLine()!);
}

static DateOnly ReadDate(string prompt)
{
    Console.Write(prompt);
    return DateOnly.Parse(Console.ReadLine()!);
}

static TimeOnly ReadTime(string prompt)
{
    Console.Write(prompt);
    return TimeOnly.Parse(Console.ReadLine()!);
}

static int ReadChoice(string title, IList<string> items)
{
    Console.WriteLine($"\n{title}:");
    for (int i = 0; i < items.Count; i++)
        Console.WriteLine($"{i + 1}) {items[i]}");   // 1-based display
    Console.Write("Select #: ");
    int choice = int.Parse(Console.ReadLine()!);
    return choice - 1; // convert back to 0-based index
}