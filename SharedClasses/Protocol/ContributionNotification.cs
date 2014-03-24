using System;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    [Serializable]
    public class ContributionNotification
    {
        public Contribution Contribution { get; set; }
    }
}