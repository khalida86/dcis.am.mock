using Ato.EN.ApplicationServices.Documents;
using Ato.EN.Security.Authorisation.AM.Messaging.ICP;
using Dcis.Am.Mock.Icp.Constants;
using Dcis.Am.Mock.Icp.Exceptions;
using Dcis.Am.Mock.Icp.Responses;
using Dcis.Am.Mock.Icp.Responses.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ato.EN.Security.Authorisation.AM.Messaging.ICP.GetClientDtls2ReponseDocument;
using static Ato.EN.Security.Authorisation.AM.Messaging.ICP.GetClientDtls2ReponseDocument.OutputClientDataDocument;
using ClientDetailsRequiredDocument = Ato.EN.Security.Authorisation.AM.Messaging.ICP.GetClientDtls2RequestDocument.ClientDetailsRequiredDocument;

namespace Dcis.Am.Mock.Icp.Services
{
    class GetClientDetailsV2 : BaseEaiCopyBookService<GetClientDetailsV2, GetClientDtls2RequestDocument, GetClientDtls2ReponseDocument>
    {
        public GetClientDetailsV2(IOptionsMonitor<ResponseScenarios> scenarios, ILogger<GetClientDetailsV2> logger) : base(scenarios, logger) { }

        protected override GetClientDtls2ReponseDocument CreateResponseDocument(GetClientDtls2RequestDocument request, bool generateDefaultResponse)
        {
            var response = CreateNewResponse(request);

            var clientDetails = new List<OutputClientDataDocument>();
            foreach (ClientDetailsRequiredDocument clientDetailRequest in request?.ClientDetailsRequiredCollection)
            {
                if (IsInvalidRequest(clientDetailRequest, out int? icpCode)
                    || ShouldReturnErrorResultCode(Scenarios.CurrentValue.GetIcpErrorMappings, clientDetailRequest, out icpCode))
                {
                    AddErrorResponse(response, clientDetails, clientDetailRequest, icpCode.Value);
                }
                else
                {
                    if (HasMatchingClientDetailScenario(Scenarios.CurrentValue.GetClientDetailsV2, clientDetailRequest, out ClientDetailScenario clientResponse))
                    {
                        clientDetails.Add(GetOutputClientDataDocument(clientDetailRequest, clientResponse));
                    }
                    else
                    {
                        // Client Type 66 equates to CID. ICP does not understand this request and will return client not found.
                        if (!(clientDetailRequest.ClientIdentifierType == 66) && generateDefaultResponse)
                        {
                            clientDetails.Add(SetGenericResponse(clientDetailRequest));
                        }
                        else
                        {
                            AddErrorResponse(response, clientDetails, clientDetailRequest, GetClientDetailsV2ResultCodes.IdNotFound);
                        }
                    }
                }
            }

            response.OutputClientDataCollection = clientDetails;
            response.OutputClientCount = clientDetails.Count;

            return response;
        }

        private static void AddErrorResponse(GetClientDtls2ReponseDocument responseListDocument, List<OutputClientDataDocument> clientDetails, ClientDetailsRequiredDocument clientDetailRequest, int resultCode)
        {
            ProcessMessage message = ClientDataError.GetProcessMessage(resultCode);
            responseListDocument.Header.ProcessMessages.Add(message);
            clientDetails.Add(CreateErrorResponse(resultCode, clientDetailRequest.InternalIdentifier, clientDetailRequest.ClientIdentifier, clientDetailRequest.ClientIdentifierType));
        }

        /// <summary>
        /// Returns an ICP Error Code if the request is invalid, otherwise null.
        /// </summary>
        private static bool IsInvalidRequest(ClientDetailsRequiredDocument request, out int? resultCode)
        {
            resultCode = null;
            if (request.InternalIdentifier == 0)
            {
                if (string.IsNullOrWhiteSpace(request.ClientIdentifier) && request.ClientIdentifierType == 0)
                {
                    resultCode = GetClientDetailsV2ResultCodes.IdNotFound;
                }
                else if (string.IsNullOrWhiteSpace(request.ClientIdentifier) || request.ClientIdentifierType == 0)
                {
                    resultCode = GetClientDetailsV2ResultCodes.InvalidExternalIdentifier;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(request.ClientIdentifier) && request.ClientIdentifierType > 0)
                {
                    resultCode = GetClientDetailsV2ResultCodes.MultipleIdentifiers;
                }
            }

            return resultCode.HasValue;
        }

        /// <summary>
        /// Tries to match the request to a ClientDetailScenario.
        /// </summary>
        private static bool HasMatchingClientDetailScenario(IEnumerable<ClientDetailScenario> clients, ClientDetailsRequiredDocument request, out ClientDetailScenario client)
        {
            client = null;
            try
            {
                if (request.InternalIdentifier > 0)
                {
                    foreach (ClientDetailScenario c in clients)
                    {
                        if (c.InternalId == request.InternalIdentifier)
                        {
                            client = c;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (ClientDetailScenario c in clients)
                    {
                        // Check if CorrelatedExternalId and IdentifierType matches request, and check if requested identifier exists in returned ClientIDs
                        if ((c.CorelatedExternalId == request.ClientIdentifier && c.IdentifierType == request.ClientIdentifierType) ||
                            (c.ClientIds != null && c.ClientIds.Any(id => id?.ClientID == request.ClientIdentifier && id?.ClientIDType == request.ClientIdentifierType)))
                        {
                            client = c;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return client is object;
        }

        private static OutputClientDataDocument GetOutputClientDataDocument(ClientDetailsRequiredDocument request, ClientDetailScenario client)
        {
            if (client == null)
            {
                return CreateErrorResponse(GetClientDetailsV2ResultCodes.IdNotFound, request.InternalIdentifier, request.ClientIdentifier, request.ClientIdentifierType);
            }
            else
            {
                OutputClientDataDocument outputClientDataDocument = new OutputClientDataDocument
                {
                    ResultCode = GetClientDetailsV2ResultCodes.Success,
                    InternalIdentifier = client.InternalId,
                    ClientIdentifier = client.Identifier,
                    ClientIdentifierType = client.IdentifierType,

                    // Retreive Details Indicators
                    RetrieveExternalIds = request.RetrieveExternalId,
                    RetrieveClientDetails = request.RetrieveClientDetails,
                    RetrieveNameDetails = request.RetrieveNameDetails,
                    RetrieveTelephoneDetails = request.RetrieveTelephoneDetails,
                    RetrieveClientIndicators = request.RetrieveClientIndicators,
                };

                if (outputClientDataDocument.RetrieveClientDetails.Equals("Y")) { RetrieveClientDetails(outputClientDataDocument, client); }
                if (outputClientDataDocument.RetrieveNameDetails.Equals("Y")) { RetrieveClientName(outputClientDataDocument, client); }
                if (outputClientDataDocument.RetrieveClientIndicators.Equals("Y")) { RetrieveClientIndicators(outputClientDataDocument, client); }
                if (outputClientDataDocument.RetrieveExternalIds.Equals("Y")) { RetrieveExternalIds(outputClientDataDocument, client); }
                if (outputClientDataDocument.RetrieveTelephoneDetails.Equals("Y")) { RetrieveTelephoneDetails(outputClientDataDocument, client); }

                outputClientDataDocument = SetValuesForClientInternalIdCall(request, outputClientDataDocument);

                return outputClientDataDocument;
            }
        }

        private static void RetrieveClientDetails(OutputClientDataDocument response, ClientDetailScenario client)
        {
            var clientDetails = client?.ClientDetails;
            if (clientDetails != null)
            {
                response.BusinessTypeCode = clientDetails.ClientType;
                response.BusinessSubCategory = clientDetails.ClientSubTypeIndicator;
                response.BusinessStartDate = clientDetails.BusinessStartDate;
                response.DayofBirth = clientDetails.DayOfBirth;
                response.MonthofBirth = clientDetails.MonthOfBirth;
                response.YearofBirth = clientDetails.YearOfBirth;
                response.Dateofdeath = clientDetails.DateOfDeath;
                response.DeathVerificationCode = clientDetails.DeathVerificationSource;
                response.SexCode = clientDetails.SexCode;
                response.DateDepartedCountry = clientDetails.DateDeparted;
                response.ResidencyIndicator = clientDetails.ResidentIndicator;
                response.IdentityStrength = clientDetails.IdentityStrength;
                response.RestrictedAccessCode = clientDetails.SecurityClassification;
                response.StaffIndicator = clientDetails.StaffIndicator;
                response.SpecialInterestIndicator = clientDetails.SecuritySpecialIndicator;
            }
        }

        private static void RetrieveClientName(OutputClientDataDocument response, ClientDetailScenario client)
        {
            var nameDetails = client?.ClientName;
            if (nameDetails != null)
            {
                response.Title = nameDetails.Title;
                response.FirstName = nameDetails.FirstName;
                response.OtherGivenName = nameDetails.MiddleName;
                response.Surname = nameDetails.Surname;
                response.Suffix = nameDetails.Suffix;
                response.EntityName = nameDetails.EntityName;
                response.CorrespondenceLine1 = nameDetails.NameShortLine1;
                response.CorrespondenceLine2 = nameDetails.NameShortLine2;
            }
        }

        private static void RetrieveClientIndicators(OutputClientDataDocument response, ClientDetailScenario client)
        {
            var indicators = client?.ClientIndicator;
            if (indicators != null)
            {
                response.LockedDownIndicator = indicators.ClientLockDownIndicator;
                response.DuplicateClientIndicator = indicators.ClientDuplicateIndicator;
                response.CompromisedIndicator = indicators.ClientCompromisedIndicator;
            }
        }

        private static void RetrieveExternalIds(OutputClientDataDocument response, ClientDetailScenario client)
        {
            List<OutputExternalIDArrayDocument> externalIdList = new List<OutputExternalIDArrayDocument>();
            var clientIds = client?.ClientIds;
            if (clientIds != null)
            {
                foreach (ClientId clientId in clientIds)
                {
                    OutputExternalIDArrayDocument externalId = new OutputExternalIDArrayDocument
                    {
                        ClientID = clientId.ClientID,
                        ClientIDStatusCode = clientId.ClientIDStatusCode,
                        ClientIDEndDate = clientId.ClientIDEndDate,
                        ClientIDStartDate = clientId.ClientIDStartDate,
                        ClientIDType = clientId.ClientIDType
                    };
                    externalIdList.Add(externalId);
                }
            }
            else
            {
                OutputExternalIDArrayDocument externalId = new OutputExternalIDArrayDocument
                {
                    ClientID = response.ClientIdentifier,
                    ClientIDStatusCode = 5,
                    ClientIDStartDate = DateTime.Now,
                    ClientIDEndDate = DateTime.Now.AddYears(2),
                    ClientIDType = response.ClientIdentifierType
                };
                externalIdList.Add(externalId);
            }
            response.OutputExternalIDArrayCollection = externalIdList;
            response.CountofExternalIds = externalIdList.Count;
        }

        private static void RetrieveTelephoneDetails(OutputClientDataDocument response, ClientDetailScenario client)
        {
            var phoneNumbers = client?.PhoneNumbers;
            if (phoneNumbers != null)
            {
                List<OutputPhoneArrayDocument> numberDocumentList = new List<OutputPhoneArrayDocument>();
                foreach (PhoneNumberDetail numberDetail in phoneNumbers)
                {
                    OutputPhoneArrayDocument numberDocument = new OutputPhoneArrayDocument
                    {
                        PhoneNumberType = numberDetail.PhoneNumberType,
                        PhoneNumberPrefix = numberDetail.PhoneNumberPrefix,
                        PhoneNumber = numberDetail.PhoneNumber
                    };
                    numberDocumentList.Add(numberDocument);
                }
                response.OutputPhoneArrayCollection = numberDocumentList;
                response.CountofTelephones = numberDocumentList.Count;
            }
        }


        private static GetClientDtls2ReponseDocument CreateClientDetailsReponseDocument(List<OutputClientDataDocument> clientDocumentCollection, GetClientDtls2RequestDocument request)
        {
            GetClientDtls2ReponseDocument response = new GetClientDtls2ReponseDocument()
            {
                Header =
                {
                    SessionId = request.Header.SessionId,
                    TransactionId = request.Header.TransactionId,
                    ApplicationId = 123,
                    // TODO: get the svc account programatically. Problem is we are using kerberos
                    UserId = "Dcis.Am.Mock.Icp",
                    ServiceName = request.Header.ServiceName,
                    QualityOfService = 1
                },
                OutputClientDataCollection = clientDocumentCollection,
                OutputClientCount = clientDocumentCollection.Count
            };

            return response;
        }

        private static OutputClientDataDocument CreateErrorResponse(int errorCode, long internalId, string externalId, int externalIdType)
        {
            var outputExternalIDArrayCollection = new List<OutputExternalIDArrayDocument>();
            var outputPhoneArrayCollection = new List<OutputPhoneArrayDocument>();

            for (int i = 0; i < 6; i++)
            {
                outputExternalIDArrayCollection.Add(new OutputExternalIDArrayDocument());
                outputPhoneArrayCollection.Add(new OutputPhoneArrayDocument());
            }

            return new OutputClientDataDocument
            {
                ResultCode = errorCode,
                InternalIdentifier = internalId,
                ClientIdentifierType = externalIdType,
                ClientIdentifier = externalId,
                CompromisedIndicator = "",
                DeathVerificationCode = 0,
                LockedDownIndicator = "",
                IdentityStrength = 0,
                BusinessTypeCode = 0,
                BusinessSubCategory = "",
                RestrictedAccessCode = 0,
                OutputExternalIDArrayCollection = outputExternalIDArrayCollection,
                OutputPhoneArrayCollection = outputPhoneArrayCollection
            };
        }

        private static OutputClientDataDocument SetGenericResponse(ClientDetailsRequiredDocument request)
        {
            var internalId = $"{request.ClientIdentifier}00";
            if (internalId.Length > 13) internalId = internalId.Substring(0, 13);
            if (!long.TryParse(internalId, out long internalIdentifier))
            {
                // Cannot generate long from passed Identifier (i.e. Identifier is myGov Link ID) - generate using magic hash
                uint hash = 23;
                foreach (char c in internalId)
                {
                    hash = (hash * 31) + c;
                }

                long.TryParse(hash.ToString().PadRight(13, '0').Substring(0, 13), out internalIdentifier);
            }
            OutputClientDataDocument outputClientDataDocument = new OutputClientDataDocument
            {
                ResultCode = GetClientDetailsV2ResultCodes.Success,
                InternalIdentifier = internalIdentifier,
                ClientIdentifier = request.ClientIdentifier,
                ClientIdentifierType = request.ClientIdentifierType,

                // Retreive Details Indicators
                RetrieveExternalIds = request.RetrieveExternalId,
                RetrieveClientDetails = request.RetrieveClientDetails,
                RetrieveNameDetails = request.RetrieveNameDetails,
                RetrieveTelephoneDetails = request.RetrieveTelephoneDetails,
                RetrieveClientIndicators = request.RetrieveClientIndicators
            };

            if (outputClientDataDocument.RetrieveClientDetails.Equals("Y")) { RetrieveClientDetails(outputClientDataDocument, null); }
            if (outputClientDataDocument.RetrieveNameDetails.Equals("Y")) { RetrieveClientName(outputClientDataDocument, null); }
            if (outputClientDataDocument.RetrieveClientIndicators.Equals("Y")) { RetrieveClientIndicators(outputClientDataDocument, null); }
            if (outputClientDataDocument.RetrieveExternalIds.Equals("Y")) { RetrieveExternalIds(outputClientDataDocument, null); }
            if (outputClientDataDocument.RetrieveTelephoneDetails.Equals("Y")) { RetrieveTelephoneDetails(outputClientDataDocument, null); }

            // For ABN lookup of subscriber using intermediary or TFN
            // Should be replaced later with scenarios in th GetClientDetailsV2Config.json
            // when testing specfic scenarios
            if (request.ClientIdentifierType.Equals(15) || request.ClientIdentifierType.Equals(5))
            {
                OutputExternalIDArrayDocument externalIdOfRan = new OutputExternalIDArrayDocument
                {
                    ClientID = request.ClientIdentifier,
                    ClientIDStatusCode = 5,
                    ClientIDStartDate = DateTime.Now,
                    ClientIDEndDate = DateTime.Now.AddYears(2),
                    ClientIDType = 10
                };
                outputClientDataDocument.OutputExternalIDArrayCollection.Add(externalIdOfRan);
                outputClientDataDocument.CountofExternalIds = outputClientDataDocument.OutputExternalIDArrayCollection.Count;
            }

            outputClientDataDocument = SetValuesForClientInternalIdCall(request, outputClientDataDocument);

            return outputClientDataDocument;
        }

        private static OutputClientDataDocument SetValuesForClientInternalIdCall(ClientDetailsRequiredDocument request, OutputClientDataDocument response)
        {
            // If request was made using the Client Internal Id, remove the Client Identifier and Client Identifier Type properties as ICP won't return them
            if (request.InternalIdentifier != 0 && request.ClientIdentifierType == 0)
            {
                response.ClientIdentifier = "";
                response.ClientIdentifierType = 0;
            }

            return response;
        }

        private static bool ShouldReturnErrorResultCode(List<IcpErrorMapping> getClientDetailsErrorMappings, ClientDetailsRequiredDocument clientDetailRequest, out int? resultCode)
        {
            foreach (var mapping in getClientDetailsErrorMappings)
            {
                if (clientDetailRequest.ClientIdentifier.Equals(mapping.IdentifierValue) && clientDetailRequest.ClientIdentifierType == mapping.IdentifierType)
                {
                    resultCode = mapping.IcpCode;
                    return true;
                }
            }

            resultCode = default;
            return false;
        }        
    }
}
