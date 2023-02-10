using Nml.Improve.Me.Dependencies;
using suffusive.uveitis.Dependencies.Model;

namespace suffusive.uveitis.Dependencies.Interfaces
{
    public class InReviewApplicationViewModel
        : ApplicationViewModel
    {
        public string InReviewMessage { get; set; }
        public Review InReviewInformation { get; set; }
    }
}