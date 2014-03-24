using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class ContributionRequest
    {
        public Contribution Contribution { get; set; }
    }
}