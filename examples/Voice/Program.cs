﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wit;
using Wit.Data;
using Wit.Input;
using Wit.Tools;

namespace Voice
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            if (Config.Load().TryGetValue(typeof(Program).Namespace!, out var ct))
            {
                args = new[] { ct };
            }

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("usage: {0} <wit-access-token>", "voice");
                Environment.Exit(1);
                return;
            }

            var accessToken = args[0];
            var logger = WitLog.Create(LogLevel.Trace);
            using var client = new WitClient(accessToken, logger);
            await client.DoInteractive();
        }
    }
}