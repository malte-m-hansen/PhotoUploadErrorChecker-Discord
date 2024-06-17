using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using System.Threading.Tasks;
using System.IO;

namespace AppPhotoErrorChecker
{
    class Program
    {
        static void Main(string[] args) {
            string cDir = Directory.GetCurrentDirectory();
            string settingsPath = cDir + @"\settings.txt";
            string webhook_url = "";
            string title = "";
            string desc = "";
            string msg = "";
            string errorPath = @"";
            string importPath = @"";

            Console.WriteLine("Running AppPhotoErrorChecker: Discord");

            if (!File.Exists(settingsPath))
            {
                using (var tw = new StreamWriter(settingsPath, true))
                {
                    tw.WriteLine("webhook_url=https://discord.com/api/webhooks/{id}/{token}");
                    tw.WriteLine(@"errorPath=\\<PATH>\Error");
                    tw.WriteLine(@"importPath=\\<PATH>\\Import");
                    Console.WriteLine("Settings file has been generted. Please relaunch program");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            else
            {
                string settings = File.ReadAllText(settingsPath);
                string[] lines = settings.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None
                );


                webhook_url = lines[0].Replace("webhook_url=", "");
                errorPath = lines[1].Replace("errorPath=", "");
                importPath = lines[2].Replace("importPath=", "");
            }



            string[] errorfiles = Directory.GetFiles(errorPath);
            string[] importfiles = Directory.GetFiles(importPath);


            int errorcount = errorfiles.Count() - 1;
            int importcount = importfiles.Count() - 1;

            Console.WriteLine(errorcount + " - " + importcount);

            //main check. Send message if any error
            if ((errorcount > 1) || (importcount > 1))
            {
                msg = " PhotoUpload error";
                title = "PhotoUpload might have some errors! Please investigate.";
                desc = "There are still pictures in folders that should be empty!\n" +
                    "Pictures with errors: " + errorcount +"\n"  +
                    "Pictures not uploaded: " + importcount;
            }
            else
            {
                Environment.Exit(1);
            }


        new Program().SendDiscordMessage(title, desc, webhook_url, msg).GetAwaiter().GetResult();
    }








        public async Task SendDiscordMessage(string title, string desc, string url, string msg)
        {
            using (var client = new DiscordWebhookClient(url))
            {
                var embed = new EmbedBuilder
                {
                    Title = title,
                    Description = desc,
                    Timestamp = DateTime.Now
                };
                await client.SendMessageAsync(text: msg, embeds: new[] { embed.Build() });
            }
        }
    }
}
