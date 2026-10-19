using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Common.Results
{
    public enum ErrorType
    {
        Failure,
        Validation,
        NotFound,
        Conflict
    }
}
