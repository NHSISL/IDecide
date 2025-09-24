// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace LondonDataServices.IDecide.Core.Models.Foundations.Notifications
{
    public class NotificationConfig
    {
        public bool InterceptNotificationProviderMode { get; set; }
        public string SmsCodeTemplateId { get; set; }
        public string EmailCodeTemplateId { get; set; }
        public string LetterCodeTemplateId { get; set; }
        public string SmsSubmissionSuccessTemplateId { get; set; }
        public string EmailSubmissionSuccessTemplateId { get; set; }
        public string LetterSubmissionSuccessTemplateId { get; set; }
        public string SmsSubscriberUsageTemplateId { get; set; }
        public string EmailSubscriberUsageTemplateId { get; set; }
        public string LetterSubscriberUsageTemplateId { get; set; }
    }
}
