using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    [Serializable]
    internal class ContributionNotification
    {
        public Contribution Contribution { get; set; }
    }
}