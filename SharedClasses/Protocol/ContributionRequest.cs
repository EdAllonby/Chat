using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    [Serializable]
    internal class ContributionRequest
    {
        public Contribution Contribution { get; set; }
    }
}