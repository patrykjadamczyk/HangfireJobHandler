using System;

namespace HangfireJobHandler.Exceptions
{
    public class DuplicateJobException: Exception
    {
        public DuplicateJobException(string message): base(message)
        {
            
        }
    }
}
