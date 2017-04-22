using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Tarantool.Client.Models;

namespace Tarantool.Client
{
    public class UpdateDefinition<T>
    {
        private readonly List<UpdateOperation> _updateOperations = new List<UpdateOperation>();

        public IEnumerable<UpdateOperation> UpdateOperations => _updateOperations;

        public UpdateDefinition<T> AddOpertation<TField>(Expression<Func<T, TField>> field, UpdateOperationCode operation, TField value, uint offset = 0, uint position = 0)
        {
            _updateOperations.Add(new UpdateOperation<T, TField>(field)
            {
                Argument = value,
                Operation = operation,
                Offset = offset,
                Position = position
            });
            return this;
        }
    }
}