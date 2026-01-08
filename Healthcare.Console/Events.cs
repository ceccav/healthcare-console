using System;
namespace App;

static class Events
{
    public static string RegistrationRequested(int requestId, string firstName, string lastName, string personalnumber)
    {
        long ticksUtc = DateTime.UtcNow.Ticks;
        return "PatientRegistrationRequested" + "|" + ticksUtc + "|" + requestId + "|" + firstName + "|" + lastName + "|" + personalnumber;
    }

    public static string RegistrationApproved(int requestId, int patientId)
    {
        long ticksUtc = DateTime.UtcNow.Ticks;
        return "PatientRegistrationApproved" + "|" + ticksUtc + "|" + requestId + "|" + patientId;
    }

    public static string TypeOf(string line)
    {
        int index = line.IndexOf('|');
        if (index < 0) return line;
        return line.Substring(0, index);
    }

    public static string[] Parts(string line)
    {
        // MVP: simple split, no escaping.
        return line.Split('|');
    }
}
