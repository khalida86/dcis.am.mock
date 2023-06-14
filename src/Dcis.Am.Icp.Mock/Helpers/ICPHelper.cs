using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Xml;
using Ato.EN.Security.Authorisation.AM.Messaging.ICP;

namespace Dcis.Am.Mock.Icp.Helpers
{
    public static class IcpHelper
    {
        /// <summary>
        /// Tax Agent client role type
        /// </summary>
        public const string TaxAgentClientRoleType = "90";

        /// <summary>
        /// Business Service Provider client role type
        /// </summary>
        public const string BusinessServiceProviderClientRoleType = "95";

        /// <summary>
        /// UPN identifier type
        /// </summary>
        public const string UpnIdentifierType = "1";

        /// <summary>
        /// TFN holder client identifier type
        /// </summary>
        public const string TfnIdentifierType = "5";

        /// <summary>
        /// WPN holder client identifier type
        /// </summary>
        public const string WpnIdentifierType = "201";

        /// <summary>
        /// EIN holder client identifier type
        /// </summary>
        public const string EinIdentifierType = "201";

        /// <summary>
        /// ABN client identifier type
        /// </summary>
        public const string AbnIdentifierType = "10";

        /// <summary>
        /// RAN client identifier type
        /// </summary>
        public const string RanIdentifierType = "15";

        /// <summary>
        /// Link client identifier type
        /// </summary>
        public const string LinkIdentifierType = "500";

        /// <summary>
        /// Exchange client identifier type
        /// </summary>
        public const string ExchangeIdentifierType = "510";

        /// <summary>
        /// Client internal identifier type
        /// </summary>
        public const string ClientInternalIdentifierType = "0";

        /// <summary>
        /// The default client data level indicator
        /// </summary>
        public const string DefaultClientDataLevelIndicator = "N";

        /// <summary>
        /// Unclassified client security classification indicator
        /// </summary>
        public const string UnclassifiedClientSecurityClassfication = "0";

        /// <summary>
        /// Highly protected client security classification indicator
        /// </summary>
        public const string HighlyProtectedClientSecurityClassfication = "30";

        /// <summary>
        /// Protected client security classification indicator
        /// </summary>
        public const string ProtectedClientSecurityClassfication = "50";

        /// <summary>
        /// Sensitive client security classification indicator
        /// </summary>
        public const string SensitiveClientSecurityClassfication = "60";

        /// <summary>
        /// Payroll provider client link code
        /// </summary>
        public const string PayrollProviderClientLinkCode = "642";

        /// <summary>
        /// Tax agent client link code
        /// </summary>
        public const string TaxAgentClientLinkCode = "100";

        /// <summary>
        /// Gets the XML for the specified request
        /// </summary>
        /// <param name="instance">The instance of the object to serialize to XML.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetXml(object instance)
        {
            return XmlHelper.SerializeToXml(instance, Encoding.UTF8);
        }

        /// <summary>
        /// Gets the XML for the specified request
        /// </summary>
        /// <param name="instance">The instance of the object to serialize to XML.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetXmlWithNamespaces(object instance)
        {
            return XmlHelper.SerializeToXmlWithNamespaces(
                instance,
                new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = false,
                    Indent = false,
                },
                GetEaiNamespaces()
            );
        }

        /// <summary>
        /// Return the EAI namespaces.
        /// </summary>
        /// <param name="allow">When true return the namespaces otherwise return null.</param>
        /// <returns>Returns a collection of tuples </returns>
        public static Tuple<string, string>[] GetEaiNamespaces(bool allow)
        {
            return allow ? GetEaiNamespaces() : null;
        }

        /// <summary>
        /// Return the EAI namespaces.
        /// </summary>
        /// <returns>Returns a collection of tuples </returns>
        public static Tuple<string, string>[] GetEaiNamespaces()
        {
            return new Tuple<string, string>[]
            {
                new Tuple<string, string>("tns", "http://www.ato.gov.au/Subjects/EAI/2005/09/ATOCanonical"),
                new Tuple<string, string>("xsd", "http://www.w3.org/2001/XMLSchema"),
                new Tuple<string, string>("xsi", "http://www.w3.org/2001/XMLSchema-instance"),
            };
        }

        /// <summary>
        /// Gets the <see cref="ControlType"/> for the specified details.
        /// </summary>
        /// <param name="requestedService">The requested service to process.</param>
        /// <param name="messageSourceId">The message source id to process.</param>
        /// <param name="userLoginText">The user login text to process.</param>
        /// <param name="sessionId">The session id to process.</param>
        /// <param name="transactionId">The transaction id to process.</param>
        /// <returns>Returns <see cref="ControlType"/></returns>
        public static ControlType GetControlType(
            string requestedService,
            string messageSourceId,
            string userLoginText,
            string sessionId,
            string transactionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrEmpty(transactionId))
            {
                transactionId = Guid.NewGuid().ToString("N");
            }

            return new ControlType
            {
                RequestedService = requestedService,
                MessageSourceID = messageSourceId,
                UserLoginText = userLoginText,
                MessageDatetime = GetCurrentDateTime(),
                MessageDatetimeSpecified = true,
                SessionID = sessionId,
                TransactionID = transactionId,
            };
        }

        /// <summary>
        /// Return the current <see cref="DateTime"/> suitable for ICP use.
        /// </summary>
        /// <returns>Returns <see cref="DateTime"/></returns>
        public static DateTime GetCurrentDateTime()
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, Calendar.CurrentEra, DateTimeKind.Local);
        }

        /// <summary>
        /// Gets the domain user.
        /// </summary>
        /// <param name="userName">The name of the user to process.</param>
        /// <param name="prefix">The prefix to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetDomainUser(string userName)
        {
            return GetDomainUser(userName, null);
        }

        /// <summary>
        /// Gets the domain user.
        /// </summary>
        /// <param name="userName">The name of the user to process.</param>
        /// <param name="prefix">The prefix to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetDomainUser(string userName, string prefix)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = WindowsIdentity.GetCurrent().Name;
            }

            if (string.IsNullOrEmpty(prefix))
            {
                prefix = string.Empty;
            }

            userName = userName.Replace('/', '\\');

            var index = userName.LastIndexOf('\\');

            return index > -1 ? prefix + userName.Substring(index + 1) : prefix + userName;
        }

        /// <summary>
        /// Gets the name part of the specified identity (domain/name).
        /// </summary>
        /// <param name="identity">The identity to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetIdentityName(string identity)
        {
            if (string.IsNullOrEmpty(identity) || !identity.Contains("\\"))
            {
                return identity;
            }

            return identity.Split('\\')[1].Trim();
        }

        /// <summary>
        /// Gets the message source id from the specified <see cref="IEAIRequest"/>.
        /// </summary>
        /// <param name="control">The <see cref="IEAIRequest"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetMessageSourceId(IEAIRequest request)
        {
            return request != null ? GetMessageSourceId(request.Control) : null;
        }

        /// <summary>
        /// Gets the message source id from the specified <see cref="ControlType"/>.
        /// </summary>
        /// <param name="control">The <see cref="ControlType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetMessageSourceId(ControlType control)
        {
            return control != null ? control.MessageSourceID : null;
        }

        /// <summary>
        /// Gets the session id from the specified <see cref="IEAIRequest"/>.
        /// </summary>
        /// <param name="control">The <see cref="IEAIRequest"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetSessionId(IEAIRequest request)
        {
            return request != null ? GetSessionId(request.Control) : null;
        }

        /// <summary>
        /// Gets the session id for the specified <see cref="ControlType"/>
        /// </summary>
        /// <param name="control">The <see cref="ControlType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetSessionId(ControlType control)
        {
            return control != null ? control.SessionID : null;
        }

        /// <summary>
        /// Gets the transaction id from the specified <see cref="IEAIRequest"/>.
        /// </summary>
        /// <param name="control">The <see cref="IEAIRequest"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetTransactionId(IEAIRequest request)
        {
            return request != null ? GetTransactionId(request.Control) : null;
        }

        /// <summary>
        /// Gets the transaction id for the specified <see cref="ControlType"/>
        /// </summary>
        /// <param name="control">The <see cref="ControlType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetTransactionId(ControlType control)
        {
            return control != null ? control.TransactionID : null;
        }

        /// <summary>
        /// Gets the collection of <see cref="SearchClientType"/> for the specified details.
        /// </summary>
        /// <param name="identifier">The <see cref="string"/> identifier to process.</param>
        /// <param name="retrieveClientExternalIDIndicator">Specify to include the client externalID indicator in the response</param>
        /// <param name="retrieveClientDetailIndicator">Specify to include the client detail indicator in the response</param>
        /// <param name="retrieveClientNameDetailIndicator">Specify to include the client name detail indicator in the response</param>
        /// <param name="retrieveClientSuppressionIndicator">Specify to include the client suppression indicator in the response</param>
        /// <param name="retrieveClientTelephoneNumberDetailIndicator">Specify to include the client telephone number detail indicator in the response</param>
        /// <returns>Returns <see cref="SearchClientType"/></returns>
        public static ICollection<SearchClientType> GetSearchClientTypes(
            string identifiers,
            bool retrieveClientExternalIDIndicator,
            bool retrieveClientDetailIndicator,
            bool retrieveClientNameDetailIndicator,
            bool retrieveClientSuppressionIndicator,
            bool retrieveClientTelephoneNumberDetailIndicator)
        {
            var results = new List<SearchClientType>();

            foreach (var identifier in CollectionHelper.Split(identifiers))
            {
                var result = GetSearchClientType(
                    identifier: identifier,
                    retrieveClientExternalIDIndicator: retrieveClientExternalIDIndicator,
                    retrieveClientDetailIndicator: retrieveClientDetailIndicator,
                    retrieveClientNameDetailIndicator: retrieveClientNameDetailIndicator,
                    retrieveClientSuppressionIndicator: retrieveClientSuppressionIndicator,
                    retrieveClientTelephoneNumberDetailIndicator: retrieveClientTelephoneNumberDetailIndicator
                );

                if (result == null)
                {
                    continue;
                }

                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Gets the <see cref="SearchClientType"/> for the specified details.
        /// </summary>
        /// <param name="identifier">The <see cref="string"/> identifier to process.</param>
        /// <param name="retrieveClientExternalIDIndicator">Specify to include the client externalID indicator in the response</param>
        /// <param name="retrieveClientDetailIndicator">Specify to include the client detail indicator in the response</param>
        /// <param name="retrieveClientNameDetailIndicator">Specify to include the client name detail indicator in the response</param>
        /// <param name="retrieveClientSuppressionIndicator">Specify to include the client suppression indicator in the response</param>
        /// <param name="retrieveClientTelephoneNumberDetailIndicator">Specify to include the client telephone number detail indicator in the response</param>
        /// <returns>Returns <see cref="SearchClientType"/></returns>
        public static SearchClientType GetSearchClientType(
            string identifier,
            bool retrieveClientExternalIDIndicator,
            bool retrieveClientDetailIndicator,
            bool retrieveClientNameDetailIndicator,
            bool retrieveClientSuppressionIndicator,
            bool retrieveClientTelephoneNumberDetailIndicator)
        {
            var result = GetSearchClientType(identifier);

            if (result == null)
            {
                return result;
            }

            result.Indicators = new IndicatorType
            {
                RetrieveClientExternalIDIndicator = retrieveClientExternalIDIndicator ? "Y" : "N",
                RetrieveClientDetailIndicator = retrieveClientDetailIndicator ? "Y" : "N",
                RetrieveClientNameDetailIndicator = retrieveClientNameDetailIndicator ? "Y" : "N",
                RetrieveClientSuppressionIndicator = retrieveClientSuppressionIndicator ? "Y" : "N",
                RetrieveClientTelephoneNumberDetailIndicator = retrieveClientTelephoneNumberDetailIndicator ? "Y" : "N",
            };

            return result;
        }

        /// <summary>
        /// Gets the <see cref="SearchClientType"/> for the specified <see cref="string"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="string"/> identifier to process.</param>
        /// <returns>Returns <see cref="SearchClientType"/></returns>
        public static SearchClientType GetSearchClientType(string identifier)
        {
            return !string.IsNullOrEmpty(identifier) ? new SearchClientType
            {
                Client = GetClientType(identifier),
            } : null;
        }

        /// <summary>
        /// Gets the <see cref="SearchResultType"/> for the specified <see cref="string"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="string"/> identifier to process.</param>
        /// <returns>Returns <see cref="SearchResultType"/></returns>
        public static SearchResultType GetSearchResultType(string identifier)
        {
            return !string.IsNullOrEmpty(identifier) ? new SearchResultType
            {
                Client = GetClientType(identifier),
            } : null;
        }

        /// <summary>
        /// Gets the <see cref="ListOfClientsType"/> for the specified collection of <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The collection of <see cref="Identifier"/> to process.</param>
        /// <returns>Returns <see cref="ListOfClientsType"/></returns>
        public static ListOfClientsType GetListOfClientsType(IEnumerable<Identifier> identifiers)
        {
            if (identifiers == null)
            {
                return null;
            }

            var result = new ListOfClientsType
            {
                Client = new List<ClientType>(identifiers.Select(i => GetClientType(i))).ToArray(),
            };

            return result.Client.Length > 0 ? result : null;
        }

        /// <summary>
        /// Gets the <see cref="ListOfClientsType"/> for the specified collection of <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The collection of <see cref="Identifier"/> to process.</param>
        /// <returns>Returns <see cref="ListOfClientsType"/></returns>
        public static ListOfClientsType GetListOfClientsType(IEnumerable<string> identifiers)
        {
            if (identifiers == null)
            {
                return null;
            }

            var result = new ListOfClientsType
            {
                Client = new List<ClientType>(identifiers.Select(i => GetClientType(i))).ToArray(),
            };

            return result.Client.Length > 0 ? result : null;
        }

        /// <summary>
        /// Gets the <see cref="ClientType"/> for the specified collection of <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The collection of <see cref="Identifier"/> to process.</param>
        /// <returns>Returns <see cref="ClientType"/></returns>
        public static ClientType GetClientType(IEnumerable<Identifier> identifiers)
        {
            if (identifiers == null)
            {
                return null;
            }

            var results = new List<ClientIdentifierType>();

            results.AddRange(identifiers.Select(i => GetClientIdentifierType(i)));

            if (results.Count == 0)
            {
                return null;
            }

            return new ClientType
            {
                ClientIdentifiers = results.ToArray()
            };
        }

        /// <summary>
        /// Gets the <see cref="ClientType"/> for the specified collection of <see cref="string"/> identifiers.
        /// </summary>
        /// <param name="identifier">The collection of <see cref="string"/> identifiers to process.</param>
        /// <returns>Returns <see cref="ClientType"/></returns>
        public static ClientType GetClientType(IEnumerable<string> identifiers)
        {
            if (identifiers == null)
            {
                return null;
            }

            var results = new List<ClientIdentifierType>();

            results.AddRange(identifiers.Select(i => GetClientIdentifierType(i)));

            if (results.Count == 0)
            {
                return null;
            }

            return new ClientType
            {
                ClientIdentifiers = results.ToArray()
            };
        }

        /// <summary>
        /// Gets the <see cref="ClientType"/> for the specified collection of <see cref="ClientIdentifierType"/>
        /// </summary>
        /// <param name="clientIdentifiers">The collection of <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="ClientType"/></returns>
        public static ClientType GetClientType(params ClientIdentifierType[] clientIdentifiers)
        {
            return GetClientType((IEnumerable<ClientIdentifierType>)clientIdentifiers);
        }

        /// <summary>
        /// Gets the <see cref="ClientType"/> for the specified collection of <see cref="ClientIdentifierType"/>
        /// </summary>
        /// <param name="clientIdentifiers">The collection of <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="ClientType"/></returns>
        public static ClientType GetClientType(IEnumerable<ClientIdentifierType> clientIdentifiers)
        {
            if (clientIdentifiers == null)
            {
                return null;
            }

            var results = new List<ClientIdentifierType>();

            results.AddRange(clientIdentifiers.Where(i => i != null).Select(i => i));

            if (results.Count == 0)
            {
                return null;
            }

            return new ClientType
            {
                ClientIdentifiers = results.ToArray()
            };
        }

        /// <summary>
        /// Gets the <see cref="ClientType"/> for the specified <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <returns>Returns <see cref="ClientType"/></returns>
        public static ClientType GetClientType(Identifier identifier)
        {
            return GetClientType(identifier, null);
        }

        /// <summary>
        /// Gets the <see cref="ClientType"/> for the specified <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <param name="roleTypeCode">The role type code.</param>
        /// <returns>Returns <see cref="ClientType"/></returns>
        public static ClientType GetClientType(Identifier identifier, string roleTypeCode)
        {
            if (identifier == null || !identifier.HasId)
            {
                return null;
            }

            var clientIdentifier = GetClientIdentifierType(identifier, roleTypeCode);

            if (clientIdentifier == null)
            {
                return null;
            }

            var clientAccount = GetClientAccountType(identifier);

            return new ClientType
            {
                ClientIdentifiers = new ClientIdentifierType[] { clientIdentifier },
                ClientAccount = clientAccount != null ? new ClientAccountType[] { clientAccount } : null
            };
        }

        /// <summary>
        /// Gets the <see cref="ClientType"/> for the specified <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The <see cref="string"/> identifier to process.</param>
        /// <returns>Returns <see cref="ClientType"/></returns>
        public static ClientType GetClientType(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return null;
            }

            var clientIdentifier = GetClientIdentifierType(identifier);

            if (clientIdentifier == null)
            {
                return null;
            }

            return new ClientType
            {
                ClientIdentifiers = new ClientIdentifierType[] { clientIdentifier },
            };
        }

        /// <summary>
        /// Gets the <see cref="ClientIdentifierType"/> for the specified <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <returns>
        /// Returns <see cref="ClientIdentifierType"/>
        /// </returns>
        public static ClientIdentifierType GetClientIdentifierType(Identifier identifier)
        {
            return GetClientIdentifierType(identifier, null);
        }

        /// <summary>
        /// Gets the <see cref="ClientIdentifierType"/> for the specified <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <param name="roleTypeCode">The role type code.</param>
        /// <returns>
        /// Returns <see cref="ClientIdentifierType"/>
        /// </returns>
        public static ClientIdentifierType GetClientIdentifierType(Identifier identifier, string roleTypeCode)
        {
            if (identifier == null || !identifier.HasId)
            {
                return null;
            }

            var isInternal = CID.IsType(identifier);

            if (string.IsNullOrEmpty(roleTypeCode))
            {
                roleTypeCode = identifier["ClientRoleTypeCode"];
            }

            var result = new ClientIdentifierType
            {
                ClientInternalID = !isInternal ? identifier["ClientInternalId"] : identifier.Id,
                ClientIdentifierTypeCode = !isInternal ? GetClientIdentifierTypeCode(identifier.Type) : null,
                ClientIdentifierValueID = !isInternal ? identifier.Id : null,
                ClientAccountID = identifier["ClientAccountId"],
                ClientRoleTypeCode = !isInternal ? roleTypeCode : null,
            };

            if (RAN.IsType(identifier))
            {
                result.ClientRoleTypeCode = GetClientRoleType(RAN.GetRole(identifier));
            }

            return result;
        }

        /// <summary>
        /// Gets the <see cref="ClientIdentifierType"/> for the specified details.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value to process.</param>
        /// <returns>
        /// Returns <see cref="ClientIdentifierType"/>
        /// </returns>
        public static ClientIdentifierType GetClientIdentifierType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var parts = value.Split('/');

            switch (parts.Length)
            {
                case 1:
                    return new ClientIdentifierType
                    {
                        ClientInternalID = parts[0].Trim()
                    };

                case 2:
                    var type = GetClientIdentifierTypeCode(parts[0].Trim());

                    if (type == ClientInternalIdentifierType)
                    {
                        return new ClientIdentifierType
                        {
                            ClientInternalID = parts[1].Trim()
                        };
                    }

                    return new ClientIdentifierType
                    {
                        ClientIdentifierTypeCode = type,
                        ClientIdentifierValueID = parts[1].Trim(),
                    };

                case 3:
                    return new ClientIdentifierType
                    {
                        ClientIdentifierTypeCode = GetClientIdentifierTypeCode(parts[0].Trim()),
                        ClientIdentifierValueID = parts[1].Trim(),
                        ClientInternalID = parts[2].Trim(),
                    };

                case 4:
                    return new ClientIdentifierType
                    {
                        ClientIdentifierTypeCode = GetClientIdentifierTypeCode(parts[0].Trim()),
                        ClientIdentifierValueID = parts[1].Trim(),
                        ClientInternalID = parts[2].Trim(),
                        ClientAccountID = parts[3].Trim(),
                    };

                default:
                    return new ClientIdentifierType
                    {
                        ClientIdentifierTypeCode = GetClientIdentifierTypeCode(parts[0].Trim()),
                        ClientIdentifierValueID = parts[1].Trim(),
                        ClientInternalID = parts[2].Trim(),
                        ClientAccountID = parts[3].Trim(),
                        ClientRoleTypeCode = parts[4].Trim(),
                    };
            }
        }

        /// <summary>
        /// Gets the <see cref="ClientIdentifierType"/> for the specified details.
        /// </summary>
        /// <param name="clientType">The client type code to process.</param>
        /// <param name="clientId">The client identifier to process.</param>
        /// <returns>
        /// Returns <see cref="ClientIdentifierType"/>
        /// </returns>
        public static ClientIdentifierType GetClientIdentifierType(
            string clientType,
            string clientId)
        {
            return GetClientIdentifierType(clientType, clientId, null);
        }

        /// <summary>
        /// Gets the <see cref="ClientIdentifierType"/> for the specified details.
        /// </summary>
        /// <param name="clientType">The client type code to process.</param>
        /// <param name="clientId">The client identifier to process.</param>
        /// <param name="clientInternalId">The client internal identifier to process.</param>
        /// <returns>
        /// Returns <see cref="ClientIdentifierType"/>
        /// </returns>
        public static ClientIdentifierType GetClientIdentifierType(
            string clientType,
            string clientId,
            string clientInternalId)
        {
            return GetClientIdentifierType(clientType, clientId, clientInternalId, null, null);
        }

        /// <summary>
        /// Gets the <see cref="ClientIdentifierType"/> for the specified details.
        /// </summary>
        /// <param name="clientType">The client type code to process.</param>
        /// <param name="clientId">The client identifier to process.</param>
        /// <param name="clientInternalId">The client internal identifier to process.</param>
        /// <param name="clientRole">The client role code to process.</param>
        /// <param name="clientAccountId">The client account identifier to process.</param>
        /// <returns>
        /// Returns <see cref="ClientIdentifierType"/>
        /// </returns>
        public static ClientIdentifierType GetClientIdentifierType(
            string clientType,
            string clientId,
            string clientInternalId,
            string clientRole,
            string clientAccountId)
        {
            if (string.IsNullOrEmpty(clientType) || string.IsNullOrEmpty(clientId))
            {
                return null;
            }

            if (clientRole == string.Empty)
            {
                clientRole = null;
            }

            if (clientAccountId == string.Empty)
            {
                clientAccountId = null;
            }

            return new ClientIdentifierType
            {
                ClientIdentifierTypeCode = clientType,
                ClientIdentifierValueID = clientId,
                ClientInternalID = clientInternalId,
                ClientRoleTypeCode = clientRole,
                ClientAccountID = clientAccountId
            };
        }

        /// <summary>
        /// Returns an external client identifier for given values
        /// </summary>
        /// <param name="clientType"></param>
        /// <param name="clientId"></param>
        /// <param name="clientInternalId"></param>    
        /// <param name="clientIdStatusCode"></param>
        /// <returns></returns>
        public static ClientIdentifierType GetExternalClientIdentifier(
            string clientType,
            string clientId,
            string clientInternalId,
            string clientIdStatusCode)
        {
            return GetExternalClientIdentifier(
                clientType,
                clientId,
                clientInternalId,
                clientIdStatusCode,
                DateTime.MinValue,
                DateTime.MinValue
            );
        }

        /// <summary>
        /// Returns an external client identifier for given values
        /// </summary>
        /// <param name="clientType"></param>
        /// <param name="clientId"></param>
        /// <param name="clientInternalId"></param>    
        /// <param name="clientIdStatusCode"></param>
        /// <param name="clientIdStartDate"></param>
        /// <param name="clientIdEndDate"></param>
        /// <returns></returns>
        public static ClientIdentifierType GetExternalClientIdentifier(
            string clientType,
            string clientId,
            string clientInternalId,
            string clientIdStatusCode,
            DateTime clientIdStartDate,
            DateTime clientIdEndDate)
        {
            if (string.IsNullOrEmpty(clientType) || string.IsNullOrEmpty(clientId))
            {
                return null;
            }

            if (clientIdStatusCode == string.Empty)
            {
                clientIdStatusCode = null;
            }

            if (clientIdStartDate == DateTime.MinValue)
            {
                TimeSpan timeSpan = new TimeSpan(3650, 0, 0, 0);
                clientIdStartDate = DateTime.Now.Subtract(timeSpan);
            }

            if (clientIdEndDate == DateTime.MinValue)
            {
                clientIdEndDate = new DateTime(9999, 12, 31);
            }

            return new ClientIdentifierType
            {
                ClientIdentifierTypeCode = clientType,
                ClientIdentifierValueID = clientId,
                ClientInternalID = clientInternalId,
                ClientIdentifierStatusCode = clientIdStatusCode,
                ClientIdentifierStartDate = clientIdStartDate,
                ClientIdentifierStartDateSpecified = true,
                ClientIdentifierEndDate = clientIdEndDate,
                ClientIdentifierEndDateSpecified = true
            };
        }


        /// <summary>
        /// Gets the client identifier type code for the specified <see cref="Identifier"/>
        /// </summary>
        /// <param name="value">The <see cref="string"/> value to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetClientIdentifierTypeCode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            switch (value)
            {
                case EXID.Code:
                    return ExchangeIdentifierType;

                case WPN.Code:
                    return WpnIdentifierType;

                case EIN.Code:
                    return EinIdentifierType;

                case TFN.Code:
                    return TfnIdentifierType;

                case ABN.Code:
                    return AbnIdentifierType;

                case LINKID.Code:
                    return LinkIdentifierType;

                case CID.Code:
                    return ClientInternalIdentifierType;

                case RAN.Code:
                    return RanIdentifierType;

                default:
                    if (RAN.IsTaxAgent(value))
                    {
                        return RanIdentifierType;
                    }
                    else if (RAN.IsBusinessServiceProvider(value))
                    {
                        return RanIdentifierType;
                    }
                    return value;
            }
        }

        /// <summary>
        /// Gets the user id from the specified <see cref="SubjectType"/>.
        /// </summary>
        /// <param name="subject">The <see cref="SubjectType"/> to process.</param>
        /// <returns></returns>
        public static string GetUserId(SubjectType subject)
        {
            return subject != null ? GetUserId(subject.SubjectIdentifiers) : null;
        }

        /// <summary>
        /// Gets the first user id from the specified collection of <see cref="SubjectIdentifierType"/>/
        /// </summary>
        /// <param name="subjectIdentifiers">The collection of <see cref="SubjectIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetUserId(IEnumerable<SubjectIdentifierType> subjectIdentifiers)
        {
            if (subjectIdentifiers == null)
            {
                return null;
            }

            foreach (var subjectIdentifier in subjectIdentifiers)
            {
                return GetUserId(subjectIdentifier);
            }

            return null;
        }

        /// <summary>
        /// Gets the user id from the specified <see cref="SubjectIdentifierType"/>.
        /// </summary>
        /// <param name="subjectIdentifier">The <see cref="SubjectIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetUserId(SubjectIdentifierType subjectIdentifier)
        {
            return subjectIdentifier != null ? subjectIdentifier.SubjectIdentifierValueID : null;
        }

        /// <summary>
        /// Return true if the specified <see cref="SubjectType"/> contains valid <see cref="SubjectIdentifierType"/> identifiers.
        /// </summary>
        /// <param name="subject">The <see cref="SubjectType"/> to process.</param>
        /// <param name="validCodes">The collection of valid  <see cref="SubjectIdentifierType"/> codes to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasValidSubjectIdentifiers(SubjectType subject, params string[] validCodes)
        {
            return HasValidSubjectIdentifiers(subject, new HashSet<string>(validCodes, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Return true if the specified <see cref="SubjectType"/> contains valid <see cref="SubjectIdentifierType"/> identifiers.
        /// </summary>
        /// <param name="subject">The <see cref="SubjectType"/> to process.</param>
        /// <param name="validCodes">The collection of valid  <see cref="SubjectIdentifierType"/> codes to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasValidSubjectIdentifiers(SubjectType subject, ICollection<string> validCodes)
        {
            if (subject == null)
            {
                return true;
            }

            if (validCodes == null || validCodes.Count == 0)
            {
                return false;
            }

            if (subject.SubjectIdentifiers == null)
            {
                return true;
            }

            foreach (var identifier in subject.SubjectIdentifiers)
            {
                if (!IsValidSubjectIdentifier(identifier, validCodes))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Return true if the specified <see cref="SubjectIdentifierType"/> contains a code found in the specified collection of valid codes.
        /// </summary>
        /// <param name="subjectIdentifier">The <see cref="SubjectIdentifierType"/> to process.</param>
        /// <param name="validCodes">The collection of valid codes to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool IsValidSubjectIdentifier(SubjectIdentifierType subjectIdentifier, ICollection<string> validCodes)
        {
            if (subjectIdentifier == null)
            {
                return true;
            }

            if (validCodes == null || validCodes.Count == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(subjectIdentifier.SubjectIdentifierTypeCode))
            {
                return true;
            }

            return validCodes.Contains(subjectIdentifier.SubjectIdentifierTypeCode);
        }

        /// <summary>
        /// Gets the first <see cref="ClientIdentifierType"/> for the specified collection of <see cref="ListOfClientsType"/>.
        /// </summary>
        /// <param name="clients">The collection of <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns <see cref="ClientIdentifierType"/></returns>
        public static ICollection<ClientIdentifierType> GetClientIdentifierTypes(IEnumerable<ListOfClientsType> clients)
        {
            var results = new List<ClientIdentifierType>();

            if (clients != null)
            {
                results.AddRange(clients.SelectMany(c => GetClientIdentifierTypes(c.Client)));
            }

            return results;
        }

        /// <summary>
        /// Gets the collection of <see cref="ClientIdentifierType"/> for the specified <see cref="ListOfClientsType"/>.
        /// </summary>
        /// <param name="clients">The <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="ClientIdentifierType"/></returns>
        public static ICollection<ClientIdentifierType> GetClientIdentifierTypes(ListOfClientsType clients)
        {
            var results = new List<ClientIdentifierType>();

            if (clients != null)
            {
                results.AddRange(GetClientIdentifierTypes(clients.Client));
            }

            return results;
        }

        /// <summary>
        /// Gets the collection of <see cref="ClientIdentifierType"/> for the specified collection of <see cref="ClientType"/>.
        /// </summary>
        /// <param name="clients">The collection of <see cref="ClientType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="ClientIdentifierType"/></returns>
        public static ICollection<ClientIdentifierType> GetClientIdentifierTypes(IEnumerable<ClientType> clients)
        {
            var results = new List<ClientIdentifierType>();

            if (clients != null)
            {
                results.AddRange(clients.SelectMany(c => GetClientIdentifierTypes(c)));
            }

            return results;
        }

        /// <summary>
        /// Gets the collection of <see cref="ClientIdentifierType"/> for the specified collection of <see cref="ClientType"/>.
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="ClientIdentifierType"/></returns>
        public static ICollection<ClientIdentifierType> GetClientIdentifierTypes(ClientType client)
        {
            var results = new List<ClientIdentifierType>();

            if (client != null)
            {
                results.AddRange(client.ClientIdentifiers);
            }

            return results;
        }

        /// <summary>
        /// Gets the collection of <see cref="ClientIdentifierType"/> for the specified collection of <see cref="ClientType"/>.
        /// </summary>
        /// <param name="clients">The collection of <see cref="string"/> to process.</param>
        /// <returns>Returns a collection of <see cref="ClientIdentifierType"/></returns>
        public static ICollection<ClientIdentifierType> GetClientIdentifierTypes(IEnumerable<string> clients)
        {
            var results = new List<ClientIdentifierType>();

            if (clients == null)
            {
                return results;
            }

            foreach (var client in clients)
            {
                if (string.IsNullOrEmpty(client))
                {
                    continue;
                }

                results.Add(GetClientIdentifierType(client));
            }

            return results;
        }

        /// <summary>
        /// Gets the collection of <see cref="ClientIdentifierType"/> for the specified collection of <see cref="ClientType"/>.
        /// </summary>
        /// <param name="clients">The collection of <see cref="string"/> to process.</param>
        /// <returns>Returns an array of <see cref="ClientIdentifierType"/></returns>
        public static ClientIdentifierType[] GetClientIdentifierTypesArray(IEnumerable<string> clients)
        {
            var results = GetClientIdentifierTypes(clients);
            return results.Count > 0 ? results.ToArray() : null;
        }

        /// <summary>
        /// Gets the collection of the first <see cref="ClientIdentifierType"/> for the specified collection of <see cref="ListOfClientsType"/>.
        /// </summary>
        /// <param name="client">The <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="ClientIdentifierType"/></returns>
        public static ICollection<ClientIdentifierType> GetAllFirstClientIdentifierType(IEnumerable<ListOfClientsType> clients)
        {
            var results = new List<ClientIdentifierType>();

            if (clients != null)
            {
                results.AddRange(clients.SelectMany(c => GetAllFirstClientIdentifierType(c.Client)));
            }

            return results;
        }

        /// <summary>
        /// Gets the collection of the first <see cref="ClientIdentifierType"/> for the specified <see cref="ListOfClientsType"/>.
        /// </summary>
        /// <param name="client">The <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="ClientIdentifierType"/></returns>
        public static ICollection<ClientIdentifierType> GetAllFirstClientIdentifierType(ListOfClientsType clients)
        {
            var results = new List<ClientIdentifierType>();

            if (clients != null)
            {
                results.AddRange(GetAllFirstClientIdentifierType(clients.Client));
            }

            return results;
        }

        /// <summary>
        /// Gets the collection of the first <see cref="ClientIdentifierType"/> in each specified <see cref="ClientType"/> from the collection of <see cref="ClientType"/>.
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="ClientIdentifierType"/></returns>
        public static ICollection<ClientIdentifierType> GetAllFirstClientIdentifierType(IEnumerable<ClientType> clients)
        {
            var results = new List<ClientIdentifierType>();

            if (clients != null)
            {
                results.AddRange(clients.Select(c => GetFirstClientIdentifierType(c)));
            }

            return results;
        }

        /// <summary>
        /// Gets the first <see cref="ClientIdentifierType"/> for the specified collection of <see cref="ListOfClientsType"/>.
        /// </summary>
        /// <param name="clients">The collection of <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns <see cref="ClientIdentifierType"/></returns>
        public static ClientIdentifierType GetFirstClientIdentifierType(IEnumerable<ListOfClientsType> clients)
        {
            var item = clients != null ? clients.FirstOrDefault() : null;
            return item != null ? GetFirstClientIdentifierType(item.Client) : null;
        }

        /// <summary>
        /// Gets the first <see cref="ClientIdentifierType"/> for the specified <see cref="ListOfClientsType"/>.
        /// </summary>
        /// <param name="clients">The <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns <see cref="ClientIdentifierType"/></returns>
        public static ClientIdentifierType GetFirstClientIdentifierType(ListOfClientsType clients)
        {
            return clients != null ? GetFirstClientIdentifierType(clients.Client) : null;
        }

        /// <summary>
        /// Gets the first <see cref="ClientIdentifierType"/> for the specified collection of <see cref="SearchClientType"/>.
        /// </summary>
        /// <param name="searchClients">The collection of <see cref="SearchClientType"/> to process.</param>
        /// <returns>Returns <see cref="ClientIdentifierType"/></returns>
        public static ClientIdentifierType GetFirstClientIdentifierType(IEnumerable<SearchClientType> searchClients)
        {
            return searchClients != null ? GetFirstClientIdentifierType(searchClients.FirstOrDefault()) : null;
        }

        /// <summary>
        /// Gets the first <see cref="ClientIdentifierType"/> for the specified collection of <see cref="ClientType"/>.
        /// </summary>
        /// <param name="clients">The collection of <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="ClientIdentifierType"/></returns>
        public static ClientIdentifierType GetFirstClientIdentifierType(IEnumerable<ClientType> clients)
        {
            return clients != null ? GetFirstClientIdentifierType(clients.FirstOrDefault()) : null;
        }

        /// <summary>
        /// Gets the first <see cref="ClientIdentifierType"/> for the specified <see cref="SearchClientType"/>
        /// </summary>
        /// <param name="searchClient">The <see cref="SearchClientType"/> to process.</param>
        /// <returns>Returns <see cref="ClientIdentifierType"/></returns>
        public static ClientIdentifierType GetFirstClientIdentifierType(SearchClientType searchClient)
        {
            return searchClient != null ? GetFirstClientIdentifierType(searchClient.Client) : null;
        }

        /// <summary>
        /// Gets the first <see cref="ClientIdentifierType"/> for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="ClientIdentifierType"/></returns>
        public static ClientIdentifierType GetFirstClientIdentifierType(ClientType client)
        {
            return client != null && client.ClientIdentifiers != null ? client.ClientIdentifiers.FirstOrDefault() : null;
        }

        /// <summary>
        /// Gets the first <see cref="ClientAccountType"/> for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="ClientAccountType"/></returns>
        public static ClientAccountType GetFirstClientAccount(ClientType client)
        {
            return client != null && client.ClientAccount != null ? client.ClientAccount.FirstOrDefault() : null;
        }

        /// <summary>
        /// Gets the first <see cref="ClientNameType"/> for the specified <see cref="ClientAccountType"/>
        /// </summary>
        /// <param name="clientAccount">The <see cref="ClientAccountType"/> to process.</param>
        /// <returns>Returns <see cref="ClientNameType"/></returns>
        public static ClientNameType GetFirstClientName(ClientAccountType clientAccount)
        {
            return clientAccount != null && clientAccount.ClientAccountName != null ? clientAccount.ClientAccountName.FirstOrDefault() : null;
        }

        /// <summary>
        /// Gets the client identifier for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetFirstClientIdentifierValue(ClientType client)
        {
            var result = GetFirstClientIdentifierType(client);
            return result != null ? result.ClientIdentifierValueID : null;
        }

        /// <summary>
        /// Gets the <see cref="ClientId"/> type for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetFirstClientTypeCode(ClientType client)
        {
            var result = GetFirstClientIdentifierType(client);
            return result != null ? GetClientTypeCode(result) : null;
        }

        /// <summary>
        /// Return true if the specified <see cref="SubjectType"/> contains valid <see cref="SubjectClientType"/> identifiers.
        /// </summary>
        /// <param name="subject">The <see cref="SubjectType"/> to process.</param>
        /// <param name="validCodes">The collection of valid  <see cref="SubjectClientType"/> codes to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasValidSubjectClient(SubjectType subject, params string[] validCodes)
        {
            return HasValidSubjectClient(subject, new HashSet<string>(validCodes, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Return true if the specified <see cref="SubjectType"/> contains valid <see cref="SubjectClientType"/> identifiers.
        /// </summary>
        /// <param name="subject">The <see cref="SubjectType"/> to process.</param>
        /// <param name="validCodes">The collection of valid  <see cref="SubjectClientType"/> codes to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasValidSubjectClient(SubjectType subject, ICollection<string> validCodes)
        {
            return subject != null ? IsValidSubjectClient(subject.SubjectOwnerClient, validCodes) : true;
        }

        /// <summary>
        /// Return true if the specified <see cref="SubjectClientType"/> contains a code found in the specified collection of valid codes.
        /// </summary>
        /// <param name="subjectClient">The <see cref="SubjectClientType"/> to process.</param>
        /// <param name="validCodes">The collection of valid codes to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool IsValidSubjectClient(SubjectClientType subjectClient, ICollection<string> validCodes)
        {
            return subjectClient != null ? IsValidClientIdentifier(subjectClient.ClientIdentifier, validCodes) : true;
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientIdentifierType"/> contains a code found in the specified collection of valid codes.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <param name="validCodes">The collection of valid codes to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool IsValidClientIdentifier(ClientIdentifierType clientIdentifier, ICollection<string> validCodes)
        {
            if (clientIdentifier == null)
            {
                return true;
            }

            if (validCodes == null || validCodes.Count == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(clientIdentifier.ClientIdentifierTypeCode))
            {
                return true;
            }

            return validCodes.Contains(clientIdentifier.ClientIdentifierTypeCode);
        }

        /// <summary>
        /// Gets the client identifier value from the specified <see cref="SubjectType"/>.
        /// </summary>
        /// <param name="subject">The <see cref="SubjectType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetClientIdentifierValue(SubjectType subject)
        {
            return subject != null ? GetClientIdentifierValue(subject.SubjectOwnerClient) : null;
        }

        /// <summary>
        /// Gets the client identifier value from the specified <see cref="SubjectClientType"/>.
        /// </summary>
        /// <param name="subjectClient">The <see cref="SubjectClientType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetClientIdentifierValue(SubjectClientType subjectClient)
        {
            return subjectClient != null ? GetClientIdentifierValue(subjectClient.ClientIdentifier) : null;
        }

        /// <summary>
        /// Gets the value of the specified <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetClientIdentifierValue(ClientIdentifierType clientIdentifier)
        {
            return clientIdentifier != null ? clientIdentifier.ClientIdentifierValueID : null;
        }

        /// <summary>
        /// Return true if the specified <see cref="ControlType"/> contains either technical exceptions or process messages.
        /// </summary>
        /// <param name="control">The <see cref="ControlType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasFault(ControlType control)
        {
            if (control == null)
            {
                return false;
            }

            if (control.TechnicalExceptions != null && control.TechnicalExceptions.Length > 0)
            {
                return true;
            }

            if (control.ProcessErrorMsgs != null && control.ProcessErrorMsgs.Length > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="ClientId"/> type for the specified <see cref="ClientIdentifierType"/>
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetClientTypeCode(ClientIdentifierType clientIdentifier)
        {
            if (clientIdentifier == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(clientIdentifier.ClientIdentifierTypeCode) && !string.IsNullOrEmpty(clientIdentifier.ClientInternalID))
            {
                return CID.Code;
            }

            return GetClientTypeCode(clientIdentifier.ClientIdentifierTypeCode);
        }

        /// <summary>
        /// Gets the <see cref="string"/> type for the specified <see cref="string"/> code.
        /// </summary>
        /// <param name="code">The <see cref="string"/> code to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetClientTypeCode(string code)
        {
            switch (code)
            {
                case TfnIdentifierType:
                    return TFN.Code;

                case WpnIdentifierType:
                    return WPN.Code;

                case AbnIdentifierType:
                    return ABN.Code;

                case LinkIdentifierType:
                    return LINKID.Code;

                case RanIdentifierType:
                    return RAN.Code;

                default:
                    return !string.IsNullOrEmpty(code) ? code : CID.Code;
            }
        }

        /// <summary>
        /// Gets the role <see cref="string"/> for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetFirstIntermediaryRole(ClientType client)
        {
            var result = GetFirstClientIdentifierType(client);
            return result != null ? GetIntermediaryRole(result) : null;
        }

        /// <summary>
        /// Gets the role <see cref="string"/> for the specified <see cref="ClientIdentifierType"/>
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetIntermediaryRole(ClientIdentifierType clientIdentifier)
        {
            return clientIdentifier != null ? GetIntermediaryRole(clientIdentifier.ClientRoleTypeCode) : null;
        }

        /// <summary>
        /// Gets the role <see cref="string"/> for the specified <see cref="ClientIdentifierType"/>
        /// </summary>
        /// <param name="code">The role <see cref="string"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetIntermediaryRole(string code)
        {
            switch (code)
            {
                case TaxAgentClientRoleType:
                    return RAN.TaxAgentRole;

                case BusinessServiceProviderClientRoleType:
                    return RAN.BusinessServiceProviderRole;

                default:
                    return code;
            }
        }

        /// <summary>
        /// Gets the client role type for the specified role type
        /// </summary>
        /// <param name="roleType">Type of the role to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetClientRoleType(string roleType)
        {
            if (RAN.IsTaxAgent(roleType))
            {
                return TaxAgentClientRoleType;
            }

            if (RAN.IsBusinessServiceProvider(roleType))
            {
                return BusinessServiceProviderClientRoleType;
            }

            return roleType;
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> for the specified collection of <see cref="SearchClientType"/>.
        /// </summary>
        /// <param name="searchClients">The collection of <see cref="SearchClientType"/> to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetFirstSearchClientId(IEnumerable<SearchClientType> searchClients)
        {
            return searchClients != null ? GetFirstSearchClientId(searchClients.FirstOrDefault()) : null;
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> for the specified <see cref="SearchClientType"/>
        /// </summary>
        /// <param name="searchClient">The <see cref="SearchClientType"/> to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetFirstSearchClientId(SearchClientType searchClient)
        {
            return searchClient != null ? GetFirstClientId(searchClient.Client) : null;
        }

        /// <summary>
        /// Gets the name of the client for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetFirstClientName(ClientType client)
        {
            var account = GetFirstClientAccount(client);

            if (account == null)
            {
                return null;
            }

            var clientName = GetFirstClientName(account);

            if (clientName != null && clientName.UnstructuredName != null)
            {
                return clientName.UnstructuredName.UnstructuredFullName;
            }

            return null;
        }

        /// <summary>
        /// Gets the collection of <see cref="Identifier"/> for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="client">The collection of <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="Identifier"/></returns>
        public static List<Identifier> GetAllFirstClientId(IEnumerable<ListOfClientsType> clients)
        {
            return new List<Identifier>(GetAllFirstClientIdentifierType(clients).Select(c => GetClientId(c)));
        }

        /// <summary>
        /// Gets the collection of <see cref="Identifier"/> for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="client">The collection of <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="Identifier"/></returns>
        public static List<Identifier> GetAllFirstClientId(ListOfClientsType clients)
        {
            return new List<Identifier>(GetAllFirstClientIdentifierType(clients).Select(c => GetClientId(c)));
        }

        /// <summary>
        /// Gets the <see cref="ListOfClientsType"/> for the specified collection of <see cref="ClientType"/>
        /// </summary>
        /// <param name="clients">The collection of <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="ListOfClientsType"/></returns>
        public static ListOfClientsType GetListOfClientsType(IEnumerable<ClientType> clients)
        {
            if (clients == null)
            {
                return null;
            }

            var results = new List<ClientType>(clients);

            if (results.Count == 0)
            {
                return null;
            }

            return new ListOfClientsType { Client = clients.ToArray() };
        }

        /// <summary>
        /// Gets the <see cref="ListOfClientsType"/> for the specified collection of <see cref="ClientIdentifierType"/>
        /// </summary>
        /// <param name="clientIdentifiers">The collection of <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="ListOfClientsType"/></returns>
        public static ListOfClientsType GetListOfClientsType(IEnumerable<ClientIdentifierType> clientIdentifiers)
        {
            if (clientIdentifiers == null)
            {
                return null;
            }

            var results = new List<ClientIdentifierType>(clientIdentifiers);

            ///No subject client (no selected clientId in ACS call)
            if (results.Count == 0)
            {
                return null;
            }

            ///Tax Agent scenario with a selected clientID
            return new ListOfClientsType
            {
                Client = new ClientType[]
                {
                    new ClientType
                    {
                        ClientIdentifiers = results.ToArray(),
                    }
                }
            };
        }

        /// <summary>
        /// Gets the <see cref="ListOfClientsType"/> for the specified collection of <see cref="ClientIdentifierType"/>
        /// </summary>
        /// <param name="clientIdentifiers">The collection of <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="ListOfClientsType"/></returns>
        public static ListOfClientsType GetListOfClientTypesWithSecurityCode(IEnumerable<ClientType> clientTypes)
        {
            if (clientTypes == null)
            {
                return null;
            }

            var results = new List<ClientType>(clientTypes);

            ///No subject client (no selected clientId in ACS call)
            if (results.Count == 0)
            {
                return null;
            }

            ///Tax Agent scenario with a selected clientID
            return new ListOfClientsType
            {
                Client = results.ToArray()
            };
        }

        /// <summary>
        /// Gets the <see cref="string"/> type for the specified <see cref="string"/> code.
        /// </summary>
        /// <param name="code">The <see cref="string"/> code to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetSecurityTypeCode(string code)
        {
            switch (code)
            {
                case HighlyProtectedClientSecurityClassfication:
                    return SecurityClassification.HighlyProtected;

                case ProtectedClientSecurityClassfication:
                    return SecurityClassification.Protected;

                case SensitiveClientSecurityClassfication:
                    return SecurityClassification.Sensitive;

                default:
                    return SecurityClassification.Unclassified;
            }
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetFirstClientId(ClientType client)
        {
            Identifier result = null;

            var clientIdentifier = GetFirstClientIdentifierType(client);

            if (clientIdentifier == null)
            {
                return result;
            }

            result = GetClientId(clientIdentifier);

            if (result != null)
            {
                ABN.SetName(result, GetFirstClientName(client));
            }

            return result;
        }

        /// <summary>
        /// Gets the client id.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetClientId(ClientIdentifierType clientIdentifier)
        {
            if (clientIdentifier == null)
            {
                return new Identifier(null);
            }

            var type = GetClientTypeCode(clientIdentifier);
            var id = CID.IsType(type) ? clientIdentifier.ClientInternalID : clientIdentifier.ClientIdentifierValueID;

            return new Identifier(type, id);
        }

        /// <summary>
        /// Gets the security code classification.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetClientSecurityCode(ClientSecurityDetailsType clientSecurity)
        {
            if (clientSecurity.ClientSecurityDetailSecurityClassificationCode == null)
            {
                return new Identifier(null);
            }

            var result = new Identifier(GetSecurityTypeCode(clientSecurity.ClientSecurityDetailSecurityClassificationCode));

            return result;
        }

        /// <summary>
        /// Gets the client id.
        /// </summary>
        /// <param name="subjectClient">The <see cref="SubjectClientType"/> to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetClientId(SubjectClientType subjectClient)
        {
            return subjectClient != null ? GetClientId(subjectClient.ClientIdentifier) : new Identifier(null);
        }

        /// <summary>
        /// Gets the client id.
        /// </summary>
        /// <param name="subject">The <see cref="SubjectType"/> to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetClientId(SubjectType subject)
        {
            return subject != null ? GetClientId(subject.SubjectOwnerClient) : new Identifier(null);
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetIdentifier(ClientType client)
        {
            if (client == null)
            {
                return null;
            }

            var type = GetFirstClientTypeCode(client);

            var result = new Identifier(type, GetFirstClientIdentifierValue(client));
            RAN.SetName(result, GetFirstClientName(client));

            if (RAN.IsType(type))
            {
                RAN.SetRole(result, GetFirstIntermediaryRole(client));
            }

            return result;
        }

        /// <summary>
        /// Return a collection of <see cref="Identifier"/> from the specified collection of <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="clientIdentifiers">The collection of <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="Identifier"/></returns>
        public static List<Identifier> GetIdentifiers(IEnumerable<ClientIdentifierType> clientIdentifiers)
        {
            var results = new List<Identifier>();

            if (clientIdentifiers == null)
            {
                return results;
            }

            foreach (var clientIdentifier in clientIdentifiers)
            {
                var result = GetIdentifier(clientIdentifier);

                if (result == null)
                {
                    continue;
                }

                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> for the specified <see cref="ClientType"/>
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetIdentifier(ClientIdentifierType clientIdentifier)
        {
            return clientIdentifier != null ? GetIdentifier(
                clientIdentifier.ClientIdentifierTypeCode,
                clientIdentifier.ClientIdentifierValueID,
                clientIdentifier.ClientInternalID,
                clientIdentifier.ClientRoleTypeCode
            ) : null;
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> for the specified details.
        /// </summary>
        /// <param name="clientType">The client type to process.</param>
        /// <param name="clientId">The client identifier to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetIdentifier(string clientType, string clientId)
        {
            return GetIdentifier(clientType, clientId, null);
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> for the specified details.
        /// </summary>
        /// <param name="clientType">The client type to process.</param>
        /// <param name="clientId">The client identifier to process.</param>
        /// <param name="clientInternalId">The client internal identifier to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetIdentifier(string clientType, string clientId, string clientInternalId)
        {
            return GetIdentifier(clientType, clientId, clientInternalId, null);
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> for the specified details.
        /// </summary>
        /// <param name="clientType">The client type to process.</param>
        /// <param name="clientId">The client identifier to process.</param>
        /// <param name="clientInternalId">The client internal identifier to process.</param>
        /// <param name="clientRole">The client role to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetIdentifier(string clientType, string clientId, string clientInternalId, string clientRole)
        {
            if (string.IsNullOrEmpty(clientType) || string.IsNullOrEmpty(clientId))
            {
                return null;
            }

            var type = GetClientTypeCode(clientType);

            var result = new Identifier(type, clientId);

            if (!string.IsNullOrEmpty(clientInternalId))
            {
                result.Set("ClientInternalId", clientInternalId);
            }

            if (!string.IsNullOrEmpty(clientRole))
            {
                RAN.SetRole(result, GetIntermediaryRole(clientRole));
            }

            return result;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <param name="activeRanDetail">The <see cref="ActiveRanDetailType"/> to process.</param>
        /// <returns>Returns <see cref="Identifier"/></returns>
        public static Identifier GetIdentifier(ActiveRanDetailType activeRanDetail)
        {
            if (activeRanDetail == null)
            {
                return null;
            }

            var client = activeRanDetail.Client;

            if (client == null)
            {
                return null;
            }

            return GetIdentifier(client);
        }

        /// <summary>
        /// Gets the value of the specified <see cref="ActiveRanDetailType"/>.
        /// </summary>
        /// <param name="activeRanDetail">The <see cref="ActiveRanDetailType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetValue(ActiveRanDetailType activeRanDetail)
        {
            return activeRanDetail != null ? GetFirstValue(activeRanDetail.Client) : null;
        }

        /// <summary>
        /// Gets the value of the specified <see cref="ClientType"/>.
        /// </summary>
        /// <param name="activeRanDetail">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetFirstValue(ClientType client)
        {
            var result = GetFirstClientIdentifierType(client);
            return result != null ? result.ClientIdentifierValueID : null;
        }

        /// <summary>
        /// Gets the values of the specified <see cref="ClientType"/>.
        /// </summary>
        /// <param name="activeRanDetail">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="string"/></returns>
        public static List<string> GetValues(ClientType client)
        {
            var results = new List<string>();

            if (client == null || client.ClientIdentifiers == null)
            {
                return results;
            }

            foreach (var clientIdentifier in client.ClientIdentifiers)
            {
                results.Add(clientIdentifier.ClientIdentifierValueID);
            }

            return results;
        }

        /// <summary>
        /// Gets the padded value of the specified <see cref="string"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value to process.</param>
        /// <param name="paddingCharacter">The padding character to process.</param>
        /// <param name="paddingLength">The length of the padding to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetPaddedValue(string value, char paddingCharacter, int paddingLength)
        {
            var result = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new string(paddingCharacter, paddingLength), value);
            return result.Substring(result.Length - paddingLength);
        }

        /// <summary>
        /// Gets the <see cref="SubjectType"/> for the specified details.
        /// </summary>
        /// <param name="subjectIdentifiers">The array of <see cref="SubjectIdentifierType"/> to process.</param>
        /// <param name="subjectClient">The <see cref="SubjectClientType"/> to process.</param>
        /// <returns>Returns <see cref="SubjectType"/></returns>
        public static SubjectType GetSubjectType(SubjectIdentifierType[] subjectIdentifiers, SubjectClientType subjectClient)
        {
            return subjectIdentifiers != null || subjectClient != null ? new SubjectType
            {
                SubjectIdentifiers = subjectIdentifiers,
                SubjectOwnerClient = subjectClient
            } : null;
        }

        /// <summary>
        /// Gets an array of <see cref="SubjectIdentifierType"/> for the specified internal identity identifiers.
        /// </summary>
        /// <param name="identityIdentifiers">The variable collection of <see cref="string"/> identity identifiers to process.</param>
        /// <returns>Returns an a rray of <see cref="SubjectIdentifierType"/></returns>
        public static SubjectIdentifierType[] GetSubjectIdentifierTypes(params string[] identityIdentifiers)
        {
            var results = new List<SubjectIdentifierType>();

            if (identityIdentifiers != null && identityIdentifiers.Length > 0)
            {
                foreach (var identityIdentifier in identityIdentifiers)
                {
                    results.Add(GetSubjectIdentifierType(identityIdentifier));
                }
            }

            return results.Count > 0 ? results.ToArray() : null;
        }

        /// <summary>
        /// Gets the <see cref="SubjectIdentifierType"/> for the specified internal identity identifier.
        /// </summary>
        /// <param name="internalIdentityId">The internal identity identifier to process.</param>
        /// <returns>Returns <see cref="SubjectIdentifierType"/></returns>
        public static SubjectIdentifierType GetSubjectIdentifierType(string internalIdentityId)
        {
            return GetSubjectIdentifierType("1", internalIdentityId);
        }

        /// <summary>
        /// Gets the <see cref="SubjectIdentifierType"/> for the specified details.
        /// </summary>
        /// <param name="type">The type to process.</param>
        /// <param name="value">The value to process.</param>
        /// <returns>Returns <see cref="SubjectIdentifierType"/></returns>
        public static SubjectIdentifierType GetSubjectIdentifierType(string type, string value)
        {
            return new SubjectIdentifierType
            {
                SubjectIdentifierTypeCode = type,
                SubjectIdentifierValueID = value
            };
        }

        /// <summary>
        /// Gets the <see cref="ClientAccountType"/> for the specified <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <returns>Returns <see cref="ClientAccountType"/></returns>
        public static ClientAccountType GetClientAccountType(Identifier identifier)
        {
            if (identifier == null)
            {
                return null;
            }

            var clientName = GetClientNameType(identifier);

            if (clientName == null)
            {
                return null;
            }

            return new ClientAccountType
            {
                ClientAccountName = new ClientNameType[] { clientName }
            };
        }

        /// <summary>
        /// Gets the <see cref="ClientNameType"/> for the specified <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <returns>Returns <see cref="ClientNameType"/></returns>
        public static ClientNameType GetClientNameType(Identifier identifier)
        {
            if (identifier == null)
            {
                return null;
            }

            var name = ABN.GetName(identifier);

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return new ClientNameType
            {
                UnstructuredName = new UnstructuredNameType
                {
                    UnstructuredFullName = name
                }
            };
        }

        /// <summary>
        /// Gets the <see cref="ActiveRanDetailType"/> for the specified <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <returns>Returns <see cref="ActiveRanDetailType"/></returns>
        public static ActiveRanDetailType GetActiveRanDetailType(Identifier identifier)
        {
            if (identifier == null)
            {
                return null;
            }

            var client = GetClientType(identifier);

            if (client == null)
            {
                return null;
            }

            return new ActiveRanDetailType
            {
                Client = client
            };
        }

        /// <summary>
        /// Gets an array of <see cref="ActiveRanDetailType"/> for the specified collection of <see cref="Identifier"/>
        /// </summary>
        /// <param name="identifiers">The collection of <see cref="Identifier"/> to process.</param>
        /// <returns>Returns an array of <see cref="ActiveRanDetailType"/></returns>
        public static ActiveRanDetailType[] GetActiveRanDetailTypes(IEnumerable<Identifier> identifiers)
        {
            if (identifiers == null)
            {
                return null;
            }

            var results = new List<ActiveRanDetailType>();

            foreach (var identifier in identifiers)
            {
                var result = GetActiveRanDetailType(identifier);

                if (result == null)
                {
                    continue;
                }

                results.Add(result);
            }

            return results.Count > 0 ? results.ToArray() : null;
        }

        /// <summary>
        /// Gets the <see cref="SubjectClientType"/> for the specified <see cref="Identifier"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <returns>Returns <see cref="SubjectClientType"/></returns>
        public static SubjectClientType GetSubjectClientType(Identifier identifier)
        {
            return identifier != null ? new SubjectClientType
            {
                ClientIdentifier = GetClientIdentifierType(identifier)
            } : null;
        }

        /// <summary>
        /// Gets the collection of <see cref="Identifier"/> for the specified collection of <see cref="ActiveRanDetailType"/>
        /// </summary>
        /// <param name="details">The collection of <see cref="ActiveRanDetailType"/> to process.</param>
        /// <returns>
        /// Returns a collection of <see cref="Identifier"/>
        /// </returns>
        public static ICollection<Identifier> GetIdentifiers(IEnumerable<ActiveRanDetailType> details)
        {
            var results = new List<Identifier>();

            if (details == null)
            {
                return results;
            }

            foreach (var detail in details)
            {
                var client = detail.Client;

                if (client == null)
                {
                    continue;
                }

                var identifier = GetIdentifier(client);

                if (identifier == null)
                {
                    continue;
                }

                results.Add(identifier);
            }

            return results;
        }

        /// <summary>
        /// Determines whether the specified request is correlated.
        /// </summary>
        /// <param name="request">The <see cref="IEAIRequest"/> to process.</param>
        /// <param name="reply">The <see cref="IEAIReply"/> to process.</param>
        /// <returns>Return <see cref="bool"/></returns>
        public static bool IsCorrelated(IEAIRequest request, IEAIReply reply)
        {
            return request != null && reply != null ? IsCorrelated(request.Control, reply.Control) : false;
        }

        /// <summary>
        /// Return true if the correlation identifier for the request <see cref="ControlType"/> is the same as the response <see cref="ControlType"/>
        /// </summary>
        /// <param name="request">The request <see cref="ControlType"/> to process.</param>
        /// <param name="response">The response <see cref="ControlType"/> to process.</param>
        /// <returns>Return <see cref="bool"/></returns>
        public static bool IsCorrelated(ControlType request, ControlType response)
        {
            if (request == null && response == null)
            {
                return true;
            }

            if (request == null || response == null || request.TransactionID == null || response.TransactionID == null)
            {
                return false;
            }

            if (request.RequestedService != response.RequestedService)
            {
                return false;
            }

            if (request.MessageSourceID != response.MessageSourceID)
            {
                return false;
            }

            if (request.UserLoginText != response.UserLoginText)
            {
                return false;
            }

            if (request.TransactionID != response.TransactionID)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Return true if the specified array of <see cref="ActiveRanDetailType"/> is valid
        /// </summary>
        /// <param name="item">The array of <see cref="ActiveRanDetailType"/> to process.</param>
        /// <returns>Return <see cref="bool"/></returns>
        public static bool IsValid(ActiveRanDetailType[] items)
        {
            if (items == null || items.Length == 0)
            {
                return false;
            }

            foreach (var item in items)
            {
                if (IsValid(item))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Return true if the specified <see cref="ActiveRanDetailType"/> is valid
        /// </summary>
        /// <param name="item">The <see cref="ActiveRanDetailType"/> to process.</param>
        /// <returns>Return <see cref="bool"/></returns>
        public static bool IsValid(ActiveRanDetailType item)
        {
            return item != null ? IsValid(item.Client) : false;
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientType"/> is valid
        /// </summary>
        /// <param name="item">The <see cref="ClientType"/> to process.</param>
        /// <returns>Return <see cref="bool"/></returns>
        public static bool IsValid(ClientType item)
        {
            if (item == null)
            {
                return false;
            }

            foreach (var identifier in item.ClientIdentifiers)
            {
                if (IsValid(identifier))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientIdentifierType"/> is valid
        /// </summary>
        /// <param name="item">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Return <see cref="bool"/></returns>
        public static bool IsValid(ClientIdentifierType item)
        {
            return
                item != null &&
                !string.IsNullOrEmpty(item.ClientIdentifierValueID) &&
                !string.IsNullOrEmpty(item.ClientIdentifierTypeCode) &&
                !string.IsNullOrEmpty(item.ClientRoleTypeCode);
        }

        /// <summary>
        /// Return true if the specified <see cref="Identifier"/> matches the specified <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool IsMatched(ClientIdentifierType clientIdentifier, Identifier identifier)
        {
            if (clientIdentifier == null || identifier == null)
            {
                return false;
            }

            if (clientIdentifier.ClientIdentifierValueID != identifier.Id)
            {
                return false;
            }

            if (clientIdentifier.ClientIdentifierTypeCode != GetClientIdentifierTypeCode(identifier.Type))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientIdentifierType"/> matches the specified <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="left">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <param name="right">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool IsMatched(ClientIdentifierType left, ClientIdentifierType right)
        {
            if (left == null && right == null)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            if (left.ClientIdentifierValueID != right.ClientIdentifierValueID)
            {
                return false;
            }

            if (left.ClientIdentifierTypeCode != right.ClientIdentifierTypeCode)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientIdentifierType"/> represents an ABN client identifier.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool IsAbn(ClientIdentifierType clientIdentifier)
        {
            return clientIdentifier != null && clientIdentifier.ClientIdentifierTypeCode == AbnIdentifierType;
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientIdentifierType"/> represents a TFN client identifier.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool IsTfn(ClientIdentifierType clientIdentifier)
        {
            return clientIdentifier != null && clientIdentifier.ClientIdentifierTypeCode == TfnIdentifierType;
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientIdentifierType"/> represents a TFN client identifier.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool IsWpn(ClientIdentifierType clientIdentifier)
        {
            return clientIdentifier != null && clientIdentifier.ClientIdentifierTypeCode == WpnIdentifierType;
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientIdentifierType"/> represents a RAN client identifier.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool IsRan(ClientIdentifierType clientIdentifier)
        {
            return clientIdentifier != null && clientIdentifier.ClientIdentifierTypeCode == RanIdentifierType;
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientIdentifierType"/> represents a RAN client identifier.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool IsLink(ClientIdentifierType clientIdentifier)
        {
            return clientIdentifier != null && clientIdentifier.ClientIdentifierTypeCode == LinkIdentifierType;
        }

        /// <summary>
        /// Gets a collection of <see cref="ProcessMessageType"/> for the specfied <see cref="ControlType"/>
        /// </summary>
        /// <param name="errorMapping">The error mapping <see cref="IDictionary"/> to process.</param>
        /// <param name="control">The <see cref="ControlType"/> to process.</param>
        /// <returns>
        /// Returns a collection of <see cref="ProcessMessageType"/>
        /// </returns>
        public static ICollection<ProcessMessageType> GetProcessMessages(IDictionary<string, object[]> errorMapping, ControlType control)
        {
            var results = new List<ProcessMessageType>();

            if (control == null || control.ProcessErrorMsgs == null)
            {
                return results;
            }

            foreach (var item in control.ProcessErrorMsgs)
            {
                var code = item.MessageCode;

                if (string.IsNullOrEmpty(code))
                {
                    continue;
                }

                var value = StringHelper.IsPositiveWholeNumber(code) ? Convert.ToInt32(code) : -1;

                if (value == 0)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(item.DescriptionText))
                {
                    item.DescriptionText = GetMappedErrorDescription(errorMapping, code);
                }

                results.Add(item);
            }

            return results;
        }

        /// <summary>
        /// Gets a collection of <see cref="ProcessMessageType"/> for the specfied <see cref="ControlType"/>
        /// </summary>
        /// <param name="control">The <see cref="ControlType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="ProcessMessageType"/></returns>
        public static List<ProcessMessageType> GetProcessMessages(ControlType control)
        {
            var results = new List<ProcessMessageType>();

            if (control == null || control.ProcessErrorMsgs == null || control.ProcessErrorMsgs.Length == 0)
            {
                return results;
            }

            results.AddRange(control.ProcessErrorMsgs);

            return results;
        }

        /// <summary>
        /// Gets a collection of <see cref="TechnicalExceptionType"/> for the specfied <see cref="ControlType"/>
        /// </summary>
        /// <param name="control">The <see cref="ControlType"/> to process.</param>
        /// <returns>Returns a collection of <see cref="TechnicalExceptionType"/></returns>
        public static List<TechnicalExceptionType> GetTechnicalExceptions(ControlType control)
        {
            var results = new List<TechnicalExceptionType>();

            if (control == null || control.TechnicalExceptions == null || control.TechnicalExceptions.Length == 0)
            {
                return results;
            }

            results.AddRange(control.TechnicalExceptions);

            return results;
        }

        /// <summary>
        /// Return true if the specified collection of <see cref="ProcessMessageType"/> contains a non-zero entry.
        /// </summary>
        /// <param name="values">The collection of <see cref="ProcessMessageType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasNonZeroProcessMessage(IEnumerable<ProcessMessageType> values)
        {
            if (values == null)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (value == null)
                {
                    continue;
                }

                var code = StringHelper.GetInt32(value.MessageCode, -1);

                if (code <= 0)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Return true if the specified collection of <see cref="TechnicalExceptionType"/> contains a non-zero entry.
        /// </summary>
        /// <param name="values">The collection of <see cref="TechnicalExceptionType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasNonZeroTechnicalException(IEnumerable<TechnicalExceptionType> values)
        {
            if (values == null)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (value == null)
                {
                    continue;
                }

                var code = StringHelper.GetInt32(value.ExceptionCode, -1);

                if (code <= 0)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="BusinessException"/> for the specified details
        /// </summary>
        /// <param name="errorMapping">The error mapping <see cref="IDictionary"/> to process.</param>
        /// <param name="ignoredErrorCodes">The collection of ignored error codes to process.</param>
        /// <param name="notCorrelatedExceptionCode">The not correlated exception code to process.</param>
        /// <param name="technicalExceptionCode">The technical exception code to process.</param>
        /// <param name="requestControl">The request <see cref="ControlType"/> to process.</param>
        /// <param name="responseControl">The response <see cref="ControlType"/> to process.</param>
        /// <param name="response">The response <see cref="object"/> to process.</param>
        /// <returns>Returns <see cref="BusinessException"/></returns>
        public static BusinessException GetException(
            IDictionary<string, object[]> errorMapping,
            ICollection<string> ignoredErrorCodes,
            int notCorrelatedExceptionCode,
            int technicalExceptionCode,
            ControlType requestControl,
            ControlType responseControl,
            object response)
        {
            var exception = GetExceptionWhenNotCorrelated(
                notCorrelatedExceptionCode,
                requestControl,
                responseControl
            );

            if (exception == null)
            {
                exception = GetExceptionForTechnicalExceptions(
                    technicalExceptionCode,
                    errorMapping,
                    responseControl
                );
            }

            if (exception == null)
            {
                exception = GetExceptionForProcessMessages(
                    errorMapping,
                    ignoredErrorCodes,
                    responseControl,
                    response
                );
            }

            return exception;
        }

        /// <summary>
        /// Gets the log message.
        /// </summary>
        /// <param name="errorMapping">The error mapping <see cref="IDictionary"/> to process.</param>
        /// <param name="ignoredErrorCodes">The collection of ignored error codes to process.</param>
        /// <param name="requestControl">The request <see cref="ControlType"/> to process.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static string GetLogMessage(
            IDictionary<string, object[]> errorMapping,
            ICollection<string> ignoredErrorCodes,
            ControlType requestControl,
            object response)
        {
            var items = GetProcessMessages(errorMapping, requestControl);

            if (!HasReportableItem(ignoredErrorCodes, items))
            {
                return null;
            }

            var builder = new StringBuilder();

            builder.AppendLine("The non-error process messages below were returned:");
            builder.AppendLine();

            foreach (var item in items)
            {
                if (DoesErrorCodeExist(ignoredErrorCodes, item.MessageCode))
                {
                    continue;
                }

                builder.Append("- ");
                builder.AppendLine(GetDescription(null, item));
            }

            if (response != null)
            {
                builder.AppendLine();
                builder.AppendLine("For the following response:");
                builder.AppendLine();
                builder.Append(XmlHelper.SerializeToXml(response, Encoding.ASCII));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the exception when not correlated.
        /// </summary>
        /// <param name="exceptionCode">The exception code.</param>
        /// <param name="requestControl">The request <see cref="ControlType"/> to process.</param>
        /// <param name="responseControl">The response <see cref="ControlType"/> to process.</param>
        /// <returns>Returns <see cref="BusinessException"/></returns>
        private static BusinessException GetExceptionWhenNotCorrelated(
            int exceptionCode,
            ControlType requestControl,
            ControlType responseControl)
        {
            return !IsCorrelated(requestControl, responseControl) ? new BusinessException(exceptionCode) : null;
        }

        /// <summary>
        /// Gets the exception for technical exceptions.
        /// </summary>
        /// <param name="exceptionCode">The exception code to process.</param>
        /// <param name="errorMapping">The error mapping <see cref="IDictionary"/> to process.</param>
        /// <param name="control">The <see cref="ControlType"/> to process.</param>
        /// <returns>Returns <see cref="BusinessException"/></returns>
        private static BusinessException GetExceptionForTechnicalExceptions(
            int exceptionCode,
            IDictionary<string, object[]> errorMapping,
            ControlType control)
        {
            var items = GetTechnicalExceptions(control);

            if (!HasInvalidItem(items))
            {
                return null;
            }

            var exception = new BusinessException(exceptionCode);

            foreach (var item in items)
            {
                exception.SetData(
                    item.ExceptionCode,
                    GetDescription("TechnicalException", item)
                );
            }

            return exception;
        }

        /// <summary>
        /// Gets the exception for process messages.
        /// </summary>
        /// <param name="errorMapping">The error mapping <see cref="IDictionary"/> to process.</param>
        /// <param name="ignoredErrorCodes">The collection of ignored error codes to process.</param>
        /// <param name="control">The <see cref="ControlType"/> to process.</param>
        /// <param name="response">The response <see cref="object"/> to process.</param>
        /// <returns>Returns <see cref="BusinessException"/></returns>
        private static BusinessException GetExceptionForProcessMessages(
            IDictionary<string, object[]> errorMapping,
            ICollection<string> ignoredErrorCodes,
            ControlType control,
            object response)
        {
            var items = GetProcessMessages(errorMapping, control);

            if (HasInvalidItem(ignoredErrorCodes, items))
            {
                var exception = new BusinessException(Convert.ToInt32(GetMappedErrorCode(errorMapping, items, "50103")));

                foreach (var item in items)
                {
                    exception.SetData(
                        item.MessageCode,
                        GetDescription("ProcessMessage", item)
                    );
                }

                return exception;
            }

            if (!HasReportableItem(ignoredErrorCodes, items))
            {
                return null;
            }

            var builder = new StringBuilder();

            builder.AppendLine("The non-error process messages below were returned:");
            builder.AppendLine();

            foreach (var item in items)
            {
                if (DoesErrorCodeExist(ignoredErrorCodes, item.MessageCode))
                {
                    continue;
                }

                builder.Append("- ");
                builder.AppendLine(GetDescription(null, item));
            }

            if (response == null)
            {
                return null;
            }

            builder.AppendLine();
            builder.AppendLine("For the following response:");
            builder.AppendLine();
            builder.Append(XmlHelper.SerializeToXml(response, Encoding.ASCII));

            return null;
        }

        /// <summary>
        /// Return true if the specified collection of <see cref="ListOfClientsType"/> has instances of <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="clients">The collection of <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasClientIdentifiers(IEnumerable<ListOfClientsType> clients)
        {
            var item = clients != null ? clients.FirstOrDefault() : null;
            return item != null ? HasClientIdentifiers(item) : false;
        }

        /// <summary>
        /// Return true if the specified <see cref="ListOfClientsType"/> has instances of <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="clients">The <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasClientIdentifiers(ListOfClientsType clients)
        {
            var item = clients != null ? clients.Client.FirstOrDefault() : null;
            return item != null ? HasClientIdentifiers(item) : false;
        }

        /// <summary>
        /// Return true if the specified collection of <see cref="ClientType"/> has instances of <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="clients">The collection of <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasClientIdentifiers(IEnumerable<ClientType> clients)
        {
            return clients != null && HasClientIdentifiers(clients.FirstOrDefault());
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientType"/> has instances of <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasClientIdentifiers(ClientType client)
        {
            return client != null && client.ClientIdentifiers != null && client.ClientIdentifiers.Length > 0;
        }

        /// <summary>
        /// Return true if the specified <see cref="ClientIdentifierType"/> has the same value as the specified <see cref="Identifier"/>.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        public static bool HasClientIdentifierValue(ClientIdentifierType clientIdentifier, Identifier identifier)
        {
            if (clientIdentifier == null || identifier == null)
            {
                return false;
            }

            if (clientIdentifier.ClientIdentifierValueID != identifier.Id)
            {
                return false;
            }

            return clientIdentifier.ClientIdentifierTypeCode == GetClientIdentifierTypeCode(identifier.Type);
        }

        /// <summary>
        /// Gets the authenticated client identifier from the specified <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetAuthenticatedClientId(ClientIdentifierType clientIdentifier)
        {
            return GetAuthenticatedClientId(clientIdentifier, null);
        }

        /// <summary>
        /// Gets the authenticated  client identifier from the specified <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <param name="defaultValue">The default value to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetAuthenticatedClientId(ClientIdentifierType clientIdentifier, string defaultValue)
        {
            return clientIdentifier != null && !string.IsNullOrEmpty(clientIdentifier.ClientIdentifierValueID) ? clientIdentifier.ClientIdentifierValueID : defaultValue;
        }

        /// <summary>
        /// Get the client internal identifier from the specified <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetClientInternalId(ClientIdentifierType clientIdentifier)
        {
            return GetClientInternalId(clientIdentifier, null);
        }

        /// <summary>
        /// Get the client internal identifier from the specified <see cref="ClientIdentifierType"/>.
        /// </summary>
        /// <param name="clientIdentifier">The <see cref="ClientIdentifierType"/> to process.</param>
        /// <param name="defaultValue">The default value to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        public static string GetClientInternalId(ClientIdentifierType clientIdentifier, string defaultValue)
        {
            return clientIdentifier != null && !string.IsNullOrEmpty(clientIdentifier.ClientInternalID) ? clientIdentifier.ClientInternalID : defaultValue;
        }

        /// <summary>
        /// Get the list of client types from the specified <see cref="Identifier"/> and <see cref="ListOfClientsType"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> to process.</param>
        /// <param name="clientTypes">The <see cref="ListOfClientsType"/> to process.</param>
        /// <returns>Returns list of <see cref="ClientType"/></returns>
        public static List<ClientType> GetSubjectClients(
            Identifier identifier,
            ListOfClientsType clientTypes)
        {
            var results = new List<ClientType>();

            if (clientTypes != null
                && clientTypes.Client != null
                && clientTypes.Client.Length > 0)
            {
                foreach (ClientType client in clientTypes.Client)
                {
                    if (client.ClientIdentifiers[0] != null
                        && client.ClientIdentifiers[0].ClientIdentifierValueID != null
                        && client.ClientIdentifiers[0].ClientIdentifierValueID == identifier.Id)
                    {
                        results.Add(client);
                    }
                }

                return results;
            }

            return results == null ? new List<ClientType>() : results;
        }

        /// <summary>
        /// Get the client link type code from the specified <see cref="ClientType"/>.
        /// </summary>
        /// <param name="client">The <see cref="ClientType"/> to process.</param>
        /// <returns>Returns a <see cref="string"/></returns>
        public static string GetClientLinkTypeCode(ClientType client)
        {
            if (client != null
                && client.ClientLinks != null && client.ClientLinks.ClientLink != null
                && client.ClientLinks.ClientLink[0] != null
                && client.ClientLinks.ClientLink[0].ClientLinkTypeCode != null)
            {
                return client.ClientLinks.ClientLink[0].ClientLinkTypeCode;
            }

            return null;
        }

        /// <summary>
        /// Return true if the specified collection of <see cref="TechnicalExceptionType"/> contains an invalid <see cref="TechnicalExceptionType"/>
        /// </summary>
        /// <param name="items">The collection of <see cref="TechnicalExceptionType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        private static bool HasInvalidItem(IEnumerable<TechnicalExceptionType> items)
        {
            if (items == null)
            {
                return false;
            }

            foreach (var item in items)
            {
                var code = item.ExceptionCode;

                if (string.IsNullOrEmpty(code))
                {
                    continue;
                }

                var value = StringHelper.IsPositiveWholeNumber(code) ? Convert.ToInt32(code) : -1;

                if (value == 0)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Return true if the specified collection of <see cref="ProcessMessageType"/> contains an invalid <see cref="ProcessMessageType"/>
        /// </summary>
        /// <param name="ignoredErrorCodes">The collection of ignored error codes to process.</param>
        /// <param name="items">The collection of <see cref="ProcessMessageType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        private static bool HasInvalidItem(ICollection<string> ignoredErrorCodes, IEnumerable<ProcessMessageType> items)
        {
            if (items == null)
            {
                return false;
            }

            foreach (var item in items)
            {
                var severity = item.SeverityCode;

                if (!string.IsNullOrEmpty(severity) && severity == "1")
                {
                    return !DoesErrorCodeExist(ignoredErrorCodes, item.MessageCode);
                }
            }

            return false;
        }

        /// <summary>
        /// Return true if the specified collection of <see cref="ProcessMessageType"/> contains a reportable <see cref="ProcessMessageType"/>
        /// </summary>
        /// <param name="ignoredErrorCodes">The collection of ignored error codes to process.</param>
        /// <param name="items">The collection of <see cref="ProcessMessageType"/> to process.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        private static bool HasReportableItem(ICollection<string> ignoredErrorCodes, IEnumerable<ProcessMessageType> items)
        {
            if (items == null)
            {
                return false;
            }

            foreach (var item in items)
            {
                if (DoesErrorCodeExist(ignoredErrorCodes, item.MessageCode))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the mapped error description.
        /// </summary>
        /// <param name="errorMapping">The error mapping <see cref="IDictionary"/> to process.</param>
        /// <param name="code">The code to look up.</param>
        /// <returns>Returns <see cref="string"/></returns>
        private static string GetMappedErrorDescription(IDictionary<string, object[]> errorMapping, string code)
        {
            return errorMapping.ContainsKey(code) ? errorMapping[code][1] as string : null;
        }

        /// <summary>
        /// Gets the mapped error code.
        /// </summary>
        /// <param name="errorMapping">The mapping <see cref="IDictionary"/> to process.</param>
        /// <param name="code">The code to look up.</param>
        /// <returns>Returns <see cref="string"/></returns>
        private static string GetMappedErrorCode(IDictionary<string, object[]> errorMapping, string code)
        {
            return errorMapping.ContainsKey(code) ? errorMapping[code][0] as string : null;
        }

        /// <summary>
        /// Gets the mapped error code.
        /// </summary>
        /// <param name="errorMapping">The mapping <see cref="IDictionary"/> to process.</param>
        /// <param name="items">The collection of <see cref="ProcessMessageType"/> to process.</param>
        /// <param name="defaultErrorCode">The default error code to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        private static string GetMappedErrorCode(
            IDictionary<string, object[]> errorMapping,
            ICollection<ProcessMessageType> items,
            string defaultErrorCode)
        {
            if (items == null || items.Count == 0)
            {
                return defaultErrorCode;
            }

            var code = items.First().MessageCode;

            if (errorMapping.ContainsKey(code))
            {
                return errorMapping[code][0] as string;
            }

            return defaultErrorCode;
        }

        /// <summary>
        /// Return true if the specified error code exists from the specified collection of error codes.
        /// </summary>
        /// <param name="errorCodes">The collection of ignored error codes to process.</param>
        /// <param name="errorCode">The error code to check.</param>
        /// <returns>Returns <see cref="bool"/></returns>
        private static bool DoesErrorCodeExist(ICollection<string> errorCodes, string errorCode)
        {
            return errorCodes != null && !string.IsNullOrEmpty(errorCode) ? errorCodes.Contains(errorCode, StringComparer.OrdinalIgnoreCase) : false;
        }

        /// <summary>
        /// Gets the description for the specified <see cref="TechnicalExceptionType"/>
        /// </summary>
        /// <param name="title">The title to process.</param>
        /// <param name="item">The <see cref="TechnicalExceptionType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        private static string GetDescription(string title, TechnicalExceptionType item)
        {
            if (item == null)
            {
                return null;
            }

            switch (item.ExceptionCode)
            {
                case "0":
                    return null;

                default:
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}{1}{2}{3}",
                        GetTitle(title),
                        GetDescription("Code", item.ExceptionCode),
                        GetDescription("Message", item.ExceptionMessage),
                        GetDescription("Module", item.ExceptionModuleID)
                    ).Trim();
            }
        }

        /// <summary>
        /// Gets the description for the specified <see cref="ProcessMessageType"/>
        /// </summary>
        /// <param name="title">The title to process.</param>
        /// <param name="item">The <see cref="ProcessMessageType"/> to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        private static string GetDescription(string title, ProcessMessageType item)
        {
            if (item == null)
            {
                return null;
            }

            switch (item.MessageCode)
            {
                case "0":
                    return null;

                default:
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}{1}{2}{3}{4}{5}{6}",
                        GetTitle(title),
                        GetDescription("Code", item.MessageCode),
                        GetDescription("Category", item.CategoryCode),
                        GetDescription("Severity", item.SeverityCode),
                        GetDescription("Field", item.FieldName),
                        GetDescription("Location", item.LocationText),
                        GetDescription("Description", item.DescriptionText)
                    ).Trim();
            }
        }

        /// <summary>
        /// Gets the description for the specified name/value pair.
        /// </summary>
        /// <param name="name">The name to process.</param>
        /// <param name="value">The value to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        private static string GetDescription(string name, string value)
        {
            return !string.IsNullOrEmpty(value) ? string.Format(CultureInfo.InvariantCulture, " {0}({1})", name, value) : null;
        }

        /// <summary>
        /// Gets the title for the specified text
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <returns>Returns <see cref="string"/></returns>
        private static string GetTitle(string text)
        {
            return !string.IsNullOrEmpty(text) ? text + ":" : null;
        }
    }
}
