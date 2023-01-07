using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WW_SYSTEM_BOT
{
    class Program
    {
        static void Main(string[] args)
        {
        
            Logger.Info("INIT", "STARTING BOT..");
            var Bot = new Bot();
            Bot.RunAsync().GetAwaiter().GetResult();

        }



    }
}
