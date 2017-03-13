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
                if (!string.IsNullOrEmpty(node.UserName) && UserName == null)
                {
                    UserName = node.UserName;
                    Password = node.Password;
                }
            }
        }

        public string Password { get; set; }

        public string UserName { get; set; }

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
            string userInfo;
            if (UserName == null)
                userInfo = "";
            else
            {
                userInfo = UserName;
                if (!string.IsNullOrEmpty(Password))
                    userInfo += ":" + Password;
                userInfo += "@";
            }
            return $"tarantool://{userInfo}{string.Join(",", Nodes.Select(x => x.Host + ":" + x.Port))}";
        }
    }
}