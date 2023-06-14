using System;
using System.Collections.Generic;

namespace Dcis.Am.Mock.Icp
{
    public class ClientDetailScenario
    {
        public long InternalId { get; set; }
        public int IdentifierType { get; set; }
        public int ClientIDType { get; set; }
        public string Identifier { get; set; }
        public string CorelatedExternalId { get; set; }
        public List<ClientId> ClientIds { get; set; }
        public ClientDetails ClientDetails { get; set; }
        public ClientName ClientName { get; set; }
        public ClientIndicator ClientIndicator { get; set; }
        public List<PhoneNumberDetail> PhoneNumbers { get; set; }
    }


    public class ClientId
    {
        public string ClientID { get; set; }
        public int ClientIDType { get; set; }
        public int ClientIDStatusCode { get; set; }
        public DateTime ClientIDStartDate { get; set; }
        public DateTime ClientIDEndDate { get; set; }
    }

    public class ClientDetails
    {
        public int ClientType { get; set; }
        public int ClientSubType { get; set; }
        public string ClientSubTypeIndicator { get; set; }
        public DateTime BusinessStartDate { get; set; }
        public int DayOfBirth { get; set; }
        public int MonthOfBirth { get; set; }
        public int YearOfBirth { get; set; }
        public DateTime DateOfDeath { get; set; }
        public int DeathVerificationSource { get; set; }
        public int SexCode { get; set; }
        public DateTime DateDeparted { get; set; }
        public string ResidentIndicator { get; set; }
        public int IdentityStrength { get; set; }
        public int SecurityClassification { get; set; }
        public string StaffIndicator { get; set; }
        public string SecuritySpecialIndicator { get; set; }
    }

    public class ClientName
    {
        public int Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public int Suffix { get; set; }
        public string EntityName { get; set; }
        public string NameShortLine1 { get; set; }
        public string NameShortLine2 { get; set; }
    }

    public class ClientIndicator
    {
        public string ClientLockDownIndicator { get; set; }
        public string ClientDuplicateIndicator { get; set; }
        public string ClientCompromisedIndicator { get; set; }
    }

    public class PhoneNumberDetail
    {
        public int PhoneNumberType { get; set; }
        public string PhoneNumberPrefix { get; set; }
        public string PhoneNumber { get; set; }
    }
}
