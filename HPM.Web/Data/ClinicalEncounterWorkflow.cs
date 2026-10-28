using HPM.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HPM.Web.Data;

public static class ClinicalEncounterWorkflow
{
    public static bool CanTransition(string current, string next) => (current, next) switch
    {
        (EncounterStatuses.Waiting, EncounterStatuses.InConsultation) => true,
        (EncounterStatuses.Waiting, EncounterStatuses.Cancelled) => true,
        (EncounterStatuses.Waiting, EncounterStatuses.NoShow) => true,
        (EncounterStatuses.InConsultation, EncounterStatuses.Completed) => true,
        (EncounterStatuses.InConsultation, EncounterStatuses.Cancelled) => true,
        _ => false
    };

    public static string FormatTempPatientId(int appointmentId) =>
        appointmentId >= 1_000_000 ? $"TMP{appointmentId}" : $"TMP-{appointmentId:000000}";

    public static bool IsTempPatientId(string? patientId) =>
        !string.IsNullOrWhiteSpace(patientId) && patientId.Trim().StartsWith("TMP", StringComparison.OrdinalIgnoreCase);

    public static async Task<PatientEncounter> CheckInAppointmentAsync(
        HpmDbContext db, DoctorAppointment appointment, int userId, CancellationToken cancellationToken = default)
    {
        var existing = await db.PatientEncounters
            .SingleOrDefaultAsync(x => x.AppointmentID == appointment.ID, cancellationToken);
        if (existing is not null) return existing;
        if (appointment.ApptDate.Date != DateTime.Today)
            throw new InvalidOperationException("Only today's appointment can be checked in.");
        if (appointment.Status is "Cancelled" or "NoShow" or "Completed")
            throw new InvalidOperationException($"A {appointment.Status} appointment cannot be checked in.");

        if (string.IsNullOrWhiteSpace(appointment.PatientID))
        {
            appointment.PatientID = FormatTempPatientId(appointment.ID);
        }

        await SqlServerConcurrency.AcquireTransactionLockAsync(
            db, $"HPM:EncounterQueue:{appointment.DoctorID}:{appointment.ApptDate:yyyyMMdd}");
        var nextQueue = (await db.PatientEncounters
            .Where(x => x.DoctorID == appointment.DoctorID && x.VisitDate == appointment.ApptDate.Date)
            .MaxAsync(x => (int?)x.QueueNo, cancellationToken) ?? 0) + 1;

        decimal? parsedWeightKg = null;
        if (!string.IsNullOrWhiteSpace(appointment.Weight))
        {
            var cleanWeight = appointment.Weight.Replace("kg", "", StringComparison.OrdinalIgnoreCase).Trim();
            if (decimal.TryParse(cleanWeight, out var wVal) && wVal > 0)
            {
                parsedWeightKg = wVal;
            }
        }

        var encounter = new PatientEncounter
        {
            PatientID = NullIfWhiteSpace(appointment.PatientID),
            PatientName = appointment.Name.Trim(),
            Age = NullIfWhiteSpace(appointment.Age),
            Sex = NullIfWhiteSpace(appointment.Sex),
            WeightKg = parsedWeightKg,
            DoctorID = appointment.DoctorID,
            AppointmentID = appointment.ID,
            QueueNo = nextQueue,
            VisitDate = appointment.ApptDate.Date,
            CheckedInAt = DateTime.Now,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            CreatedBy = userId
        };
        appointment.Status = "CheckedIn";
        db.PatientEncounters.Add(encounter);
        return encounter;
    }

    public static async Task<PatientEncounter> AttachTicketAsync(
        HpmDbContext db, OPDTicket ticket, int userId, CancellationToken cancellationToken = default)
    {
        var existing = await db.PatientEncounters
            .SingleOrDefaultAsync(x => x.TicketID == ticket.ID, cancellationToken);
        if (existing is not null)
        {
            if (existing.AppointmentID.HasValue && existing.DoctorID != ticket.DoctorID)
                throw new InvalidOperationException("The doctor cannot be changed after the ticket is linked to a checked-in appointment.");
            ApplyTicket(existing, ticket);
            return existing;
        }

        var visitDate = ticket.TransDate.Date;
        PatientEncounter? encounter = null;
        if (!string.IsNullOrWhiteSpace(ticket.RegNo))
        {
            encounter = await db.PatientEncounters
                .Where(x => x.PatientID == ticket.RegNo && x.DoctorID == ticket.DoctorID
                    && x.VisitDate == visitDate && x.TicketID == null
                    && (x.Status == EncounterStatuses.Waiting || x.Status == EncounterStatuses.InConsultation))
                .OrderByDescending(x => x.CheckedInAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (encounter is null)
        {
            await SqlServerConcurrency.AcquireTransactionLockAsync(
                db, $"HPM:EncounterQueue:{ticket.DoctorID}:{visitDate:yyyyMMdd}");
            var nextQueue = (await db.PatientEncounters
                .Where(x => x.DoctorID == ticket.DoctorID && x.VisitDate == visitDate)
                .MaxAsync(x => (int?)x.QueueNo, cancellationToken) ?? 0) + 1;
            encounter = new PatientEncounter
            {
                QueueNo = nextQueue,
                VisitDate = visitDate,
                CheckedInAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };
            db.PatientEncounters.Add(encounter);
        }

        ApplyTicket(encounter, ticket);
        if (encounter.AppointmentID is null && !string.IsNullOrWhiteSpace(ticket.RegNo))
        {
            var appointment = await db.DoctorAppointments
                .Where(x => x.PatientID == ticket.RegNo && x.DoctorID == ticket.DoctorID && x.ApptDate == visitDate)
                .OrderByDescending(x => x.ID)
                .FirstOrDefaultAsync(cancellationToken);
            if (appointment is not null && !await db.PatientEncounters.AnyAsync(x => x.AppointmentID == appointment.ID, cancellationToken))
            {
                encounter.AppointmentID = appointment.ID;
                appointment.Status = "CheckedIn";
            }
        }
        return encounter;
    }

    private static void ApplyTicket(PatientEncounter encounter, OPDTicket ticket)
    {
        encounter.TicketID = ticket.ID;
        encounter.PatientID = NullIfWhiteSpace(ticket.RegNo);
        encounter.PatientName = ticket.Name.Trim();
        encounter.Age = NullIfWhiteSpace(ticket.Age);
        encounter.Sex = NullIfWhiteSpace(ticket.Sex);
        encounter.DoctorID = ticket.DoctorID;
        encounter.DepartmentID = ticket.DeptID;
        encounter.ChiefComplaint ??= NullIfWhiteSpace(ticket.Comments);
        encounter.UpdatedAt = DateTime.Now;
    }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
