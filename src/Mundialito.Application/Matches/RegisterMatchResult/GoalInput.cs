using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Matches.RegisterMatchResult
{
    public sealed record GoalInput(Guid PlayerId, int Minute);
}
