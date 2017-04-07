using System.Collections.Generic;

namespace Tarantool.Client.Models.ClientMessages
{
    public class ReplaceRequest : ReplaceRequest<IEnumerable<object>>
    {
        public ReplaceRequest()
        {
            Tuple = new List<object>();
        }
    }
}