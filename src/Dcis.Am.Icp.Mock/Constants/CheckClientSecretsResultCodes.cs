namespace Dcis.Am.Mock.Icp.Constants
{
    public class CheckClientSecretsResultCodes
    {
        public const int InputClientExternalIdAndClientExternalIdTypeAreBothMissing = 40190;
        public const int InputClientExternalIdMissingAndClientExternalIdTypePresent = 40192;
        public const int InputClientExternalIdPresentAndClientExternalIdTypeMissing = 25153;
        public const int NoPOROAnswersWereProvided = 41025;
        public const int InputChannelIsMissing = 91894;
        public const int ChannelAandExternalIdTypeNotFoundInPOROChannelList = 91895;
        public const int QuestionTypeIncorrect = 61333;
        public const int TooFewQuestionsProvided = 40205;
        public const int AnswerNotProvided = 60286;

    }
}
