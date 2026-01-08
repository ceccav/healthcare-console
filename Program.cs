using System;
using System.Collections.Generic;
using System.Linq;
using App;

string logPath = "data/events.log";
EventStore store = new EventStore(logPath);


State state = new State();
state.EventLines = store.LoadAll();
state.Rebuild();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Healthcare Console MVP");
    Console.WriteLine("1) Request registration");
    Console.WriteLine("2) Approve registration (admin)");
    Console.WriteLine("3) List pending requests");
    Console.WriteLine("4) List patients");
    Console.WriteLine("0) Exit");
    Console.Write("> ");

    string choice = Console.ReadLine() ?? "";

    if (choice == "0")
        break;

    try
    {
        if (choice == "1")
        {
            string line = CreateRegistrationRequestedLine(state);
            store.Append(line);

            state.EventLines.Add(line);
            state.Rebuild();

            Console.WriteLine("Request stored.");
            Console.WriteLine("Pending reguests: " + state.PendingRequests.Count);
            Console.WriteLine("Patients: " + state.Patients.Count);
        }
        else if (choice == "2")
        {
            string line = CreateRegistrationApprovedLine(state);
            store.Append(line);

            state.EventLines.Add(line);
            state.Rebuild();

            Console.WriteLine("Approval stored.");
            Console.WriteLine("Pending reguests: " + state.PendingRequests.Count);
            Console.WriteLine("Patients: " + state.Patients.Count);
        }
        else if (choice == "3")
        {
            PrintPendingRequests(state);
        }
        else if (choice == "4")
        {
            PrintPatients(state);
        }
        else
        {
            Console.WriteLine("Unknown option.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

static string CreateRegistrationRequestedLine(State state)
{
    Console.Write("First name: ");
    string firstName = (Console.ReadLine() ?? "").Trim();

    Console.Write("Last name: ");
    string lastName = (Console.ReadLine() ?? "").Trim();

    Console.Write("Personalnumber (12 digits): ");
    string personalnumber = (Console.ReadLine() ?? "").Trim();

    if (firstName.Length == 0) throw new Exception("First name is required.");
    if (lastName.Length == 0) throw new Exception("Last name is required.");
    if (personalnumber.Length != 12 || !personalnumber.All(char.IsDigit))
        throw new Exception("Personnummer must be exactly 12 digits.");

    // MVP rule: keep parsing simple
    if (firstName.Contains("|") || lastName.Contains("|") || personalnumber.Contains("|"))
        throw new Exception("Character '|' is not allowed.");

    int requestId = state.NextRequestId;
    return Events.RegistrationRequested(requestId, firstName, lastName, personalnumber);
}

static string CreateRegistrationApprovedLine(State state)
{
    if (state.PendingRequests.Count == 0)
        throw new Exception("No pending requests.");

    PrintPendingRequests(state);

    Console.Write("RequestId to approve: ");
    string input = (Console.ReadLine() ?? "").Trim();

    if (!int.TryParse(input, out int requestId))
        throw new Exception("Invalid RequestId.");

    if (!state.PendingRequests.ContainsKey(requestId))
        throw new Exception("RequestId not found.");

    int patientId = state.NextPatientId;
    return Events.RegistrationApproved(requestId, patientId);
}

static void PrintPendingRequests(State state)
{
    if (state.PendingRequests.Count == 0)
    {
        Console.WriteLine("No pending requests.");
        return;
    }

    foreach (KeyValuePair<int, RegistrationRequest> entry in state.PendingRequests)
    {
        RegistrationRequest r = entry.Value;
        Console.WriteLine("RequestId " + r.RequestId + ": " + r.FirstName + " " + r.LastName + " (" + r.PersonalNumber + ")");
    }
}

static void PrintPatients(State state)
{
    if (state.Patients.Count == 0)
    {
        Console.WriteLine("No patients.");
        return;
    }

    foreach (KeyValuePair<int, Patient> entry in state.Patients)
    {
        Patient p = entry.Value;
        Console.WriteLine("PatientId " + p.PatientId + ": " + p.FirstName + " " + p.LastName + " (" + p.PersonalNumber + ")");
    }
}
