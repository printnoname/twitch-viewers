using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stateless;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TwitchViewersNotifier
{
    public partial class MainWindow : Window
    {
        private StateMachine<State, Trigger> appState;
        private String clientId;

        enum State
        {
            Online,
            Offline,
            //ErrorGetClientId,
            //ErrorInternetConnection,
            //ErrorGetStreamInfo
        }

        enum Trigger
        {
            SyncButtonClicked,
            UnsyncButtonClicked
        }

        public MainWindow()
        {
            InitializeComponent();
            clientId = TwitchData.getClientId();
            appState = initStateMachine();
        }

        private async void OnlineRoutine()
        {
            HttpClient client = new HttpClient();
            await Task.Run(async () =>
            {
                String userIdRequestUrl = "https://api.twitch.tv/helix/users?login=printnoname";

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, userIdRequestUrl);
                HttpResponseMessage responseMessage = new HttpResponseMessage();
                HttpContent httpContent;

                while (appState.IsInState(State.Online))
            {
                    
                    try
                    {
                        requestMessage.Headers.Add("Client-ID", clientId);
                        responseMessage = await httpClient.SendAsync(requestMessage);
                        httpContent = responseMessage.Content;
                        string result = await httpContent.ReadAsStringAsync();

                        JObject json = JObject.Parse(result);
                        String userId = JObject.Parse(json.GetValue("data")[0].ToString()).GetValue("id").ToString();

                        if(userId.Equals(""))
                        {
                            throw new Exception();
                        }

                    } catch (Exception ex) {
                       
                    } finally {
                        ((IDisposable)httpClient).Dispose();
                        ((IDisposable)requestMessage).Dispose();
                        ((IDisposable)responseMessage).Dispose();
                    }
                Thread.Sleep(5000);
                }

            });

        }

        private void Sync(object sender, RoutedEventArgs e)
        {
            appState.FireAsync(Trigger.SyncButtonClicked);
            syncButton.Visibility = Visibility.Hidden;
            unsyncButton.Visibility = Visibility.Visible;
        }

        private StateMachine<State,Trigger> initStateMachine()
        {
            StateMachine<State, Trigger> stateMachine = new StateMachine<State, Trigger>(State.Offline);

            stateMachine.Configure(State.Offline)
                .OnEntry(()=> Debug.Print("Utility is offline"))
                .Permit(Trigger.SyncButtonClicked, State.Online);

            stateMachine.Configure(State.Online)
                .OnEntryAsync(async () => await Task.Run(()=>OnlineRoutine()))
                .Permit(Trigger.UnsyncButtonClicked, State.Offline);

            return stateMachine;
        }

        private void UnSync(object sender, RoutedEventArgs e)
        {
            appState.Fire(Trigger.UnsyncButtonClicked);
            syncButton.Visibility = Visibility.Visible;
            unsyncButton.Visibility = Visibility.Hidden;
        }
    }
}
