using Hangfire;
using Hangfire.States;
using System.Collections.Generic;

namespace HangfireJobHandler.Attributes
{
    public class ContinuationsSupportIncludingFailedStateAttribute : ContinuationsSupportAttribute
    {
        public ContinuationsSupportIncludingFailedStateAttribute()
            : this(new[] { SucceededState.StateName, DeletedState.StateName, FailedState.StateName })
        { }

        public ContinuationsSupportIncludingFailedStateAttribute(
            string[] knownFinalStates)
            : base(new HashSet<string>(knownFinalStates))
        { }
    }
}
