using Stateless;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace TwitchViewersNotifier
{
    public partial class MainWindow : Window
    {

        enum State
        {
            Online,
            Offline
        }

        enum Trigger
        {
            Sync,
            Unsync
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Sync(object sender, RoutedEventArgs e)
        {
            Debug.Print(TwitchData.getClientId());
        }

        private StateMachine<State,Trigger> initStateMachine()
        {
            StateMachine<State, Trigger> stateMachine = new StateMachine<State, Trigger>(State.Offline);

            
        }

        private void UnSync(object sender, RoutedEventArgs e)
        {

        }

        public async Task MainLoop()
        {

        }
    }
}
