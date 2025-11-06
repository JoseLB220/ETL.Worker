using ETL.Worker.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ETL.Worker.Application.Interfaces
{
    public interface IExtractor
    {
        Task<IEnumerable<RecordModel>> ExtractAsync(CancellationToken cancellationToken = default);
    }
}
