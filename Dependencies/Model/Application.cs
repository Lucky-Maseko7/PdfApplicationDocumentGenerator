using System;
using System.Collections.Generic;
using Nml.Improve.Me.Dependencies;

namespace suffusive.uveitis.Dependencies.Model
{
    public class Application
    {
        public Guid Id { get; set; }
        public ApplicationState State { get; set; }
        public string ReferenceNumber { get; set; }
        public Person Person { get; set; }
        public bool IsLegalEntity { get; set; }
        public DateTimeOffset Date { get; set; }
        public LegalEntity LegalEntity { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public Review CurrentReview { get; set; }
    }
}