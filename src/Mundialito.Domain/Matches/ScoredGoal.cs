using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Domain.Matches
{
    public sealed record ScoredGoal(Guid PlayerId, Guid TeamId, int Minute);
}
