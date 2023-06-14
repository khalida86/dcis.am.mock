using System.Text.RegularExpressions;

namespace Dcis.Amfi.Isf.Mock.Models
{
    public class Entity
    {
        private string? _externalIdType;
        public string? ExternalIdType
        {
            get { return _externalIdType; }
            set
            {
                _externalIdType = value;
                _formattedValue = null;
            }
        }
        private string? _externalIdValue;
        public string? ExternalIdValue
        {
            get { return _externalIdValue; }
            set
            {
                _externalIdValue = value;
                _formattedValue = null;
            }
        }
        public string? DisplayName { get; set; }
        public bool isAbn => string.IsNullOrWhiteSpace(ExternalIdType) ? false : ExternalIdType.Equals("ABN", StringComparison.InvariantCultureIgnoreCase);
        public bool isArn => string.IsNullOrWhiteSpace(ExternalIdType) ? false : ExternalIdType.Equals("ARN", StringComparison.InvariantCultureIgnoreCase);
        public string? ABN => isAbn ? FormatExternalIdValue : "";
        public string? ARN => isArn ? FormatExternalIdValue : "";
        public string? Identifier => Regex.Replace($"{ExternalIdType}{ExternalIdValue}", @"\s", "");
        private string? _formattedValue;
        public string FormatExternalIdValue
        {
            get
            {
                if (string.IsNullOrEmpty(_formattedValue))
                {
                    var formattedValue = Regex.Replace(ExternalIdValue ?? "", @"\s", "");
                    var regex = ".*";

                    if (isAbn)
                        regex = "^(.{3})(.{3})(.{3})(.*)$";

                    if (isArn)
                        regex = "^(.{2})(.{3})(.{3})(.{3})(.*)$";

                    var match = Regex.Match(formattedValue, regex);

                    _formattedValue = string.Join(" ", match.Groups.Values.Skip(1));
                }

                return _formattedValue;
            }
        }
    }

    //public get formattedValue(): string {
    //if(this._formattedValue){
    //    return this._formattedValue;
    //}
    //else{
    //    let regEx = /.*/ g;
    //    let formattedValue = this.value?.replace(/\s / g, "");

    //    switch (this.scheme?.toUpperCase())
    //    {
    //        case "ARN":
    //            regEx = / ^(.{ 3})(.{ 3})(.{ 3})(.*)$/ g;
    //            break;
    //        case "ABN":
    //            regEx = / ^(.{ 2})(.{ 3})(.{ 3})(.{ 3})(.*)$/ g;
    //            break;
    //    }

    //    const found = regEx.exec(formattedValue);

    //    if (found)
    //    {
    //        formattedValue = found.slice(1).join(' ').trim();
    //    }

    //    return formattedValue;
    //}

}