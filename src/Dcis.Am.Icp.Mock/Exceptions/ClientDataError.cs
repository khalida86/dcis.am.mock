using Ato.EN.ApplicationServices.Documents;
using Dcis.Am.Mock.Icp.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dcis.Am.Mock.Icp.Exceptions
{
    /// <summary>
    /// Static class defining the error messages related to ClientData Error Codes (ICP)
    /// </summary>
    public class ClientDataError
    {
        const string UnknownErrorMessage = "An unknown error occured";

        public static List<Error> ErrorList;

        /// <summary>
        /// Lookup of the associated error message given an error code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetErrorMessage(int code)
        {
            if (ErrorList == null)
            {
                ErrorList = LoadListFromFile();
            }
            return ErrorList.Find(x => x.Code.Equals(code))?.Message ?? UnknownErrorMessage;
        }

        /// <summary>
        /// Loads known errors from a JSON file.
        /// </summary>
        /// <returns></returns>
        private static List<Error> LoadListFromFile()
        {
            try
            {
                var jsonString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Exceptions\ClientDataErrors.json"));
                return JsonConvert.DeserializeObject<List<Error>>(jsonString);
            }
            catch
            {
                return new List<Error>();
            }
            
        }

        public static ProcessMessage GetProcessMessage(int code)
        {
            if (ErrorList == null)
            {
                ErrorList = LoadListFromFile();
            }
            var message = ErrorList.Find(x => x.Code.Equals(code));

            if (message == null)
            {
                return GetUnkownErrorMessage(code);
            }

            return new ProcessMessage()
            {
                Id = message.Code.ToString(),
                Message = message.Message,
                Severity = message.Severity
            };
        }

        public static ProcessMessage GetUnkownErrorMessage(int code = 99999)
        {
            return new ProcessMessage()
            {
                Id = code.ToString(),
                Severity = ProcessMessageSeverity.Error,
                Message = UnknownErrorMessage
            };
        }
    }

    /// <summary>
    /// Internal class used to map errors from the JSON file.
    /// </summary>
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public ProcessMessageSeverity Severity { get; set; }
    }
}
