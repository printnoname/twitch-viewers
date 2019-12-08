using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace TwitchViewersNotifier
{
    class TwitchData
    {
        public static String getClientId()
        {
            try
            {
                String jsonString = File.ReadAllText("config.json");
                String client_id = "Not found shit";
                Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

                data.TryGetValue("client_id", out client_id);
                return client_id;
            } catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
