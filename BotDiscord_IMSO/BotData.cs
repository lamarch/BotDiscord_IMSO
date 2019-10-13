using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotDiscord_IMSO
{
    static partial class Program
    {
        private class BotData
        {
            //TODO : Add bot settings saved in a file
            string URL;
            string PREFIX;
            int UPDATE_TIME;
            uint CHAN_UPDATE;
            uint CHAN_BOT;
            bool ADMIN_ONLY;
            bool DISCORD_CMD;

            Dictionary<string, string> settings;

            public BotData(string uRL = "romlegend.aternos.me", string pREFIX = "!", int uPDATE_TIME = 60_000, uint cHAN_UPDATE = 0, uint cHAN_BOT = 0, bool aDMIN_ONLY = true, bool dISCORD_CMD = true)
            {
                URL = uRL ?? throw new ArgumentNullException(nameof(uRL));
                PREFIX = pREFIX ?? throw new ArgumentNullException(nameof(pREFIX));
                UPDATE_TIME = uPDATE_TIME;
                CHAN_UPDATE = cHAN_UPDATE;
                CHAN_BOT = cHAN_BOT;
                ADMIN_ONLY = aDMIN_ONLY;
                DISCORD_CMD = dISCORD_CMD;
            }

            public void Load(string fname)
            {
                List<string> lines = File.ReadAllLines(fname).ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] kv = lines[i].Split('=');
                    if(kv.Length != 2)
                    {
                        //TODO : error broken line
                        throw new Exception("Config file broken at line ("+i+") !");
                    }
                    settings.Add(kv[0], kv[1]);
                }
            }

            public async Task Save(string fname)
            {
                StringBuilder textFile = new StringBuilder();
                string str_value = "";
                string str_key = "";
                foreach (string key in settings.Keys)
                {
                    str_value = settings[key].Replace('=', '_').Trim();
                    str_key = key.Replace('=', '_').Trim();

                    textFile.Append(str_key + "=" + str_value + "\n");
                }

                await File.WriteAllTextAsync(fname, textFile.ToString());
            }
        }
    }
}
