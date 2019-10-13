using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PM_Utils;



namespace BotDiscord_IMSO
{
    static partial class Program
    {

        private static bool loopErrorOccured = false;

        //url
        private static string URL = "https://romlegend.aternos.me/";

        //prefix before bot command
        private static string PREFIX = "!";

        //sleep time before update server status
        private static int UPDATE_TIME = 60_000;

        //bot messages name channel
        private static string botChannel = "bot";

        //bot
        private static DiscordSocketClient client = null;

        //update channel
        private static IMessageChannel updateChannel;

        //last message
        private static IUserMessage last_stmsg;

        //last server status
        private static ServerStatus last_servst;




        private static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            WriteLog("demarrage BOT...");
            try
            {
                using (client = new DiscordSocketClient())
                {
                    client.Log += BotLog;
                    client.MessageReceived += OnMessageReceived;
                    client.Ready += Client_Ready;

                    await client.LoginAsync(Discord.TokenType.Bot, "NjMxNDk0MDA1OTM0NTIyMzg4.XaCO9g.SNDyoXPa4iik7rlHfn_WLK1fE1Y");
                    await client.StartAsync();

                    await Task.Delay(-1);
                }

            }
            catch(Exception e)
            {
                WriteError(e.ToString(), "MainAsyncException_0x1");
                PConsole.QuitWithAsk();
            }
            finally
            {
                client?.Dispose();
            }

        }

        private static Task Client_Ready()
        {
            updateChannel = (IMessageChannel)client.GetChannel(632546625209892864);
            Task.Run(() => LoopGetServerStatus());
            updateChannel.SendMessageAsync("Bonjour tout le monde !");
            return Task.CompletedTask;
        }

        private static async Task LoopGetServerStatus()
        {
            long i = 0;
            WriteLog("Mise en route boucle...");

            while (true)
            {
                WriteLoopLog("Boucle n " + (++i));

                WriteLoopLog("Récupération de l'état du serveur...");
                ServerStatus status = await GetServerStatus();

                WriteLoopLog("Etat serveur :");
                WriteLoopLog($"{status.status} : {status.nbPlayer} joueurs");
                if(status.status == ServerStatus.Status.Error)
                {
                    loopErrorOccured = true;
                    WriteLoopLog("Erreur !");
                    WriteError(status.error, "Loop_0x1");
                    await updateChannel.SendMessageAsync("J'ai rencontré une erreur lors de mon travail... Merci de contacter l'administrateur !");
                    WriteLoopLog("Appuyer sur une touche pour réessayer...");
                    PConsole.ReadChar();
                }
                else
                {
                    loopErrorOccured = false;
                }
                
                //status changed
                if (!status.Equals(last_servst))
                {

                    //status online
                    if (status.status == ServerStatus.Status.Online)
                    {
                        await updateChannel.SendMessageAsync($"Le serveur est désormais ouvert avec {status.nbPlayer} joueurs !");
                    }


                    //status offline
                    else if (status.status == ServerStatus.Status.Offline)
                    {
                        await updateChannel.SendMessageAsync("Le serveur est malheureusement fermé...");
                    }


                    //error
                    else
                    {
                        WriteLoopLog("etat serveur : ERREUR");

                        WriteError(status.error, "GetServerStatus_0x1");
                        WriteLoopLog("Appuyer sur une touche pour réessayer...");
                        PConsole.ReadChar();
                    }
                    last_servst = status;



                }
                WriteLoopLog("Sommeil...(" + UPDATE_TIME + " ms)");
                await Task.Delay(UPDATE_TIME);
            }
        }


        private static async Task OnMessageReceived(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;
            if (message == null) return;
            if (message.Channel.Name != botChannel) return;
            if (loopErrorOccured) return;
            //full message
            string content = message.Content;

            string command = "";

            //if not start with prefix then drop
            if (!content.StartsWith(PREFIX)) return;

            //else remove prefix
            else content = content.Substring(PREFIX.Length);
            
            WriteLog("Nouveau message recu : " + content);

            List<string> args = content.Split(' ').ToList();
            if (args.Count >= 1)
            {
                command = args[0];
                args.RemoveAt(0);
            }

            //WriteLog("Commande : " + command);


            //request latency
            if (command == "ping" && !message.Author.IsBot)
            {

                WriteLog($"Fonction \"PONG\" : retour = {client.Latency} ms");

                await message.Channel.SendMessageAsync($"pong ({client.Latency} ms) ! " + string.Join(" ", args));
                return;
            }


            //request server status
            if(command == "??")
            {
                ServerStatus status = await GetServerStatus();

                if (status.status == ServerStatus.Status.Online)
                {
                    WriteLog("Fonction \"ETAT DU SERVEUR\" : retour = OUVERT");

                    await message.Channel.SendMessageAsync($"Le serveur est en ligne avec {status.nbPlayer} joueurs, va les rejoindre !");
                }
                else if (status.status == ServerStatus.Status.Offline)
                {
                    WriteLog("Fonction \"ETAT DU SERVEUR\" : retour = FERME");

                    await message.Channel.SendMessageAsync("Le serveur est hors-ligne, désolé...");
                }
                else
                {
                    WriteLog("Fonction \"ETAT DU SERVEUR\" : retour = ERREUR");

                    await message.Channel.SendMessageAsync("Une erreur est survenue, merci de contacter l'administrateur...");

                    WriteError(status.error, "GetServerStatus_0x1");
                }
                return;
            }


            //set some parameters...
            if(command == "param")
            {
                string name = "";
                if(args.Count >= 1)
                {
                    name = args[0];
                    args.RemoveAt(0);
                }

                WriteLog("Fonction \"PARAM\" nom : " + name + " args : " + string.Join(" ", args));

                ParseParam(name, args, message);
                return;
            }


            //request help
            if(command == "aide")
            {
                await message.Channel.SendMessageAsync("Désolé, mais cette fonction n'est encore pas disponible !");
                return;
            }


            //Just with the prefix, so no commmand
            await message.Channel.SendMessageAsync("Fonction vide. Ajoutez \"aide\" comme argument pour plus d'aide !");
        }

        private static void ParseParam(string name, List<string> args, SocketUserMessage message)
        {
            //TODO : add settings
            switch (name)
            {
                case "url-serv":
                {
                    message.Channel.SendMessageAsync("Désolé, mais cette fonction n'est encore pas disponible !");
                    break;
                }
                case "delai-maj":
                {
                    message.Channel.SendMessageAsync("Désolé, mais cette fonction n'est encore pas disponible !");
                    break;
                }
                case "salon-bot":
                {
                    message.Channel.SendMessageAsync("Désolé, mais cette fonction n'est encore pas disponible !");
                    break;
                }
                case "salon-maj":
                {
                    message.Channel.SendMessageAsync("Désolé, mais cette fonction n'est encore pas disponible !");
                    break;
                }
                case "admin":
                {
                    message.Channel.SendMessageAsync("Désolé, mais cette fonction n'est encore pas disponible !");
                    break;
                }
                case "discord_cmd":
                {
                    message.Channel.SendMessageAsync("Désolé, mais cette fonction n'est encore pas disponible !");
                    break;
                }
                case "prefix":
                {
                    message.Channel.SendMessageAsync("Désolé, mais cette fonction n'est encore pas disponible !");
                    break;
                }
                case "aide":
                {
                    message.Channel.SendMessageAsync("Aide des paramètres, encore non disponible.");
                    break;
                }
                case "":
                {
                    message.Channel.SendMessageAsync("Ajoutez \"aide\" comme argument pour plus d'aide !");
                    break;
                }
                default:
                {
                    message.Channel.SendMessageAsync("Cette commande est invalide, ajoutez \"aide\" comme argument pour plus d'aide !");
                    break;
                }

            }
        }

    }
}
