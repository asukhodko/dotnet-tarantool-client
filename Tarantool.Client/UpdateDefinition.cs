using System.Collections.Generic;

using Tarantool.Client.Models;

namespace Tarantool.Client
{
    public class UpdateDefinition<T>
    {
        private List<UpdateOperation> _updateOperations = new List<UpdateOperation>();

        public IEnumerable<UpdateOperation> UpdateOperations => _updateOperations;

        public UpdateDefinition<T> AddOpertation(UpdateOperation updateOperation)
        {
            _updateOperations.Add(updateOperation);
            return this;
        }
    }
}