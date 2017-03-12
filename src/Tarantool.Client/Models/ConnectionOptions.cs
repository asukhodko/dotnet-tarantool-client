using System;
using System.Collections.Generic;
using System.Linq;

namespace Tarantool.Client.Models
{
    public class ConnectionOptions
    {
        private ulong _nextRequestId = 1000;

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

        public ulong GetNextRequestId()
        {
            lock (this)
            {
                return _nextRequestId++;
            }
        }

        public override string ToString()
        {
            if (Nodes.Count == 0)
                return "";
            var node0 = Nodes.First();
            return "tarantool://" + (string.IsNullOrEmpty(node0.UserInfo) ? "" : node0.UserInfo + "@") +
                   string.Join(",", Nodes.Select(x => x.Host + ":" + x.Port));
        }
    }
}