using System;
using System.Collections.Generic;
using System.Linq;

namespace Tarantool.Client.Models
{
    public class ConnectionOptions
    {
        public ConnectionOptions(string connectionString)
        {
            Nodes = new List<Uri>();

            foreach (var url in connectionString.Split(','))
            {
                var node = url.StartsWith("tarantool://")
                    ? new UriBuilder(url)
                    : new UriBuilder("tarantool://" + url);
                if (node.Port == -1)
                    node.Port = 3301;
                Nodes.Add(node.Uri);
            }
        }

        public List<Uri> Nodes { get; set; }

        public override string ToString()
        {
            return "tarantool://" + string.Join(",", Nodes.Select(x => x.Host + ":" + x.Port));
        }
    }
}