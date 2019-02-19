using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoClient.Services
{
    public interface IDataStore<T>
    {
        Task<IEnumerable<T>> GetItemsAsync();
    }
}
