using System.Collections.Generic;

namespace Tarantool.Client.Models.ClientMessages
{
    public class InsertRequest : InsertRequest<IEnumerable<object>>
    {
        public InsertRequest() : base()
        {
            Tuple = new List<object>();
        }
    }
}
