using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mel.Live
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Core.Startup(args).Run();
        }
    }
}
