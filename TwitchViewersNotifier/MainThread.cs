using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchViewersNotifier
{
    class MainThread
    {
        public static void MainRoutine()
        {
            while (true) { 
                Debug.Print("Thread working");
                Thread.Sleep(5000);
            }
        }

        static async Task UserIdCall(String username)
        {
            HttpClient client = new HttpClient();
            return client.GetAsync("http://webcode.me");
        }
    }
}
