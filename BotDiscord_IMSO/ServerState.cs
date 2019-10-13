using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PM_Utils;

namespace BotDiscord_IMSO
{
    static partial class Program
    {


        private static async Task<ServerStatus> GetServerStatus()
        {
            string content = null;

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    content = await Task.Run(() => webClient.DownloadString(new Uri(URL)));
                }
            }
            catch (Exception e)
            {
                //cannot reach url
                return ServerStatus.ServerStatusError("err:url\n" + e);
            }


            //cannot get html content
            if (content == null)
            {
                return ServerStatus.ServerStatusError("err:dl");
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            string text_status = "";
            string text_nbPlayer = "";
            int nbPlayer = 0;

            try
            {
                text_status = doc.DocumentNode.Descendants()
                    .Where((n) => n.HasClass("status"))
                    .First().Descendants().ElementAt(1).InnerText;
                //WriteLog("SERV_STATUS : " + text_status);

            }
            catch (Exception e)
            {
                return ServerStatus.ServerStatusError("err:html_status\n" + e);
            }

            try
            {
                text_nbPlayer = doc.DocumentNode.Descendants()
                    .Where((n) => n.HasClass("info-label")).First().InnerText;
                //WriteLog("SERV_NB_PLAYER : " + text_nbPlayer);
                text_nbPlayer = text_nbPlayer.Split('/')[0];

            }
            catch (Exception e)
            {
                return ServerStatus.ServerStatusError("err:html_nbPlayer\n" + e);
            }

            if (text_status != "Online" && text_status != "Offline")
            {
                return ServerStatus.ServerStatusError("err:text_status");
            }

            if (!int.TryParse(text_nbPlayer, out nbPlayer))
            {
                return ServerStatus.ServerStatusError("err:text_nbPlayer (=" + text_nbPlayer+")");
            }

            if(text_status == "Online")
            {
                return ServerStatus.ServerStatusOnline(nbPlayer);
            }
            else
            {
                return ServerStatus.ServerStatusOffline();
            }


        }




#pragma warning disable CS0659 // Le type se substitue à Object.Equals(object o) mais pas à Object.GetHashCode()
        private readonly struct ServerStatus
        {
            public readonly Status status;
            public readonly string error;
            public readonly int nbPlayer;
            //public readonly List<string> players;
            //TODO : Add players list

            public ServerStatus(Status status, string error, int nbPlayer)
            {
                this.status = status;
                this.error = error;
                this.nbPlayer = nbPlayer;
            }

            public static ServerStatus ServerStatusOnline(int nbPlayer)
            {
                return new ServerStatus(Status.Online, "", nbPlayer);
            }

            public static ServerStatus ServerStatusOffline()
            {
                return new ServerStatus(Status.Offline, "", 0);
            }

            public static ServerStatus ServerStatusError(string err)
            {
                return new ServerStatus(Status.Error, err, -1);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                ServerStatus serverStatus = (ServerStatus)obj;
                return this.status == serverStatus.status && this.error == serverStatus.error && this.nbPlayer == serverStatus.nbPlayer;
            }

            public enum Status
            {
                Null,
                Online,
                Offline,
                Error
            }
        }
    }
}
