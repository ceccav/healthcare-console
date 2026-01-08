using System;
using System.Collections.Generic;

namespace App;

sealed class State
{
    public int NextRequestId = 1;
    public int NextPatientId = 1;

    public List<string> EventLines = new List<string>();

    public Dictionary<int, RegistrationRequest> PendingRequests =
        new Dictionary<int, RegistrationRequest>();

    public Dictionary<int, Patient> Patients =
        new Dictionary<int, Patient>();

    public void Rebuild()
    {
        NextRequestId = 1;
        NextPatientId = 1;
        PendingRequests = new Dictionary<int, RegistrationRequest>();
        Patients = new Dictionary<int, Patient>();

        for (int i = 0; i < EventLines.Count; i++)
        {
            Apply(EventLines[i]);
        }
    }

    public void Apply(string line)
    {
        string[] p = Events.Parts(line);
        if (p.Length < 2)
            throw new Exception("Invalid event line.");

        string type = p[0];

        if (type == "PatientRegistrationRequested")
        {
            int requestId = int.Parse(p[2]);
            string firstName = p[3];
            string lastName = p[4];
            string personalnumber = p[5];

            PendingRequests[requestId] = new RegistrationRequest(requestId, firstName, lastName, personalnumber);

            if (requestId >= NextRequestId)
                NextRequestId = requestId + 1;

            return;
        }

        if (type == "PatientRegistrationApproved")
        {
            int requestId = int.Parse(p[2]);
            int patientId = int.Parse(p[3]);

            if (!PendingRequests.ContainsKey(requestId))
                return;

            RegistrationRequest req = PendingRequests[requestId];

            Patients[patientId] = new Patient(patientId, req.FirstName, req.LastName, req.PersonalNumber);
            PendingRequests.Remove(requestId);

            if (patientId >= NextPatientId)
                NextPatientId = patientId + 1;

            return;
        }

        throw new Exception("Unknown event type.");
    }
}

public sealed record RegistrationRequest(int RequestId, string FirstName, string LastName, string PersonalNumber);
public sealed record Patient(int PatientId, string FirstName, string LastName, string PersonalNumber);
