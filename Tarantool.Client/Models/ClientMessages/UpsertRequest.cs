using System.Collections.Generic;

namespace Tarantool.Client.Models.ClientMessages
{
    public class UpsertRequest : UpsertRequest<IEnumerable<object>>
    {
        public UpsertRequest()
        {
            Tuple = new List<object>();
        }
    }
}