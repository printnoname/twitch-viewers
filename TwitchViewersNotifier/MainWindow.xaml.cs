using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json.Linq;
using Stateless;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TwitchViewersNotifier
{
    public partial class MainWindow : Window
    {

        TaskbarIcon trayIcon;

        private StateMachine<State, Trigger> appState;
        private String clientId;
        private String username = "degrastream";
        private int viewers = 0;

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

        private void DoubleClickTrayIcon(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            trayIcon.Visibility = Visibility.Hidden;
        }

        public MainWindow()
        {
            InitializeComponent();

            trayIcon = new TaskbarIcon();
            trayIcon.Icon = Properties.Resources.TrayIcon;
            trayIcon.TrayMouseDoubleClick += DoubleClickTrayIcon;
            trayIcon.Visibility = Visibility.Hidden;

            clientId = TwitchData.getClientId();
            appState = initStateMachine();
        }

        private async void OnlineRoutine()
        {
            await Task.Run(async () =>
            {
                String userIdRequestUrl = "https://api.twitch.tv/helix/users?login=" + username;
                String streamDataRequestUrl = "";
                String userId = "";

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, userIdRequestUrl);
                HttpResponseMessage responseMessage = new HttpResponseMessage();
                HttpContent httpContent;

                String stringResult = "";
                int _viewers = viewers;
                JObject jsonResult = null;

                while (appState.IsInState(State.Online))
            {
                    
                    try
                    {
                        
                        if (String.IsNullOrEmpty(streamDataRequestUrl) || String.IsNullOrEmpty(userId)) {
                            requestMessage.Headers.Add("Client-ID", clientId);
                            responseMessage = await httpClient.SendAsync(requestMessage);
                            httpContent = responseMessage.Content;
                            stringResult = await httpContent.ReadAsStringAsync();

                            jsonResult = JObject.Parse(stringResult);
                            userId = JObject.Parse(jsonResult.GetValue("data")[0].ToString()).GetValue("id").ToString();

                            if (userId.Equals(""))
                            {
                                throw new Exception();
                            }
                        }

                        streamDataRequestUrl = "https://api.twitch.tv/helix/streams?user_id=" + userId;
                        requestMessage = new HttpRequestMessage(HttpMethod.Get, streamDataRequestUrl);
                        requestMessage.Headers.Add("Client-ID", clientId);
                        responseMessage = await httpClient.SendAsync(requestMessage);
                        httpContent = responseMessage.Content;
                        stringResult = await httpContent.ReadAsStringAsync();

                        jsonResult = JObject.Parse(stringResult);

                        String _viewersString = JObject.Parse(jsonResult.GetValue("data")[0].ToString()).GetValue("viewer_count").ToString();
                        Int32.TryParse(_viewersString, out _viewers);

                        if(_viewers != viewers)
                        {
                            OnViewersChange(_viewers);
                        }
                        viewers = _viewers;

                    }
                    catch (Exception ex) {
                        Debug.Print(ex.ToString());
                    } 
                Thread.Sleep(5000);
                }

                ((IDisposable)httpClient).Dispose();
                ((IDisposable)requestMessage).Dispose();
                ((IDisposable)responseMessage).Dispose();

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

        private void OnViewersChange(int viewers)
        {
            Debug.Print(viewers.ToString());
        }


        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                this.Hide();
                trayIcon.Visibility = Visibility.Visible;
            }
                

            base.OnStateChanged(e);
        }
    }
}
