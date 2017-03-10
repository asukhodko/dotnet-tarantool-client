using System;
using System.Collections.Generic;
using System.Linq;

namespace Tarantool.Client.Models
{
    public class ConnectionOptions
    {
        public ConnectionOptions(string connectionString)
        {
            foreach (var url in connectionString.Split(','))
                Nodes.Add(url.StartsWith("tarantool://")
                    ? new Uri(url)
                    : new Uri("tarantool://" + url));
        }

        public List<Uri> Nodes { get; set; }

        public override string ToString()
        {
            return string.Join(",", Nodes.Select(x => x.ToString()));
        }
    }
}