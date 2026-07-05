namespace Candidate.Application.Common.Options
{
    public class ApplicantInvitationSettings
    {
        public string? OfferInvitePath { get; set; } = "/applicant/offer-invite";

        public string? InviteTokenQueryName { get; set; } = "token";

        public int? InviteTokenExpiryDays { get; set; } = 15;
    }
}
