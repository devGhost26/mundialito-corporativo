using Mundialito.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mundialito.Application.Abstractions
{
    public interface IUnitOfWork
    {
        Task<Result> CommitAsync(CancellationToken cancellationToken = default);
    }
}
