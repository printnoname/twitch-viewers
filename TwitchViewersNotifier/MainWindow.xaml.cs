using Stateless;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TwitchViewersNotifier
{
    public partial class MainWindow : Window
    {
        private 
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
            StateMachine<State, Trigger> appState = initStateMachine();

            Thread mainThread = new Thread(MainThread.MainRoutine);
            mainThread.Start();

        }

        private void Sync(object sender, RoutedEventArgs e)
        {
            Debug.Print(TwitchData.getClientId());
        }

        private StateMachine<State,Trigger> initStateMachine()
        {
            StateMachine<State, Trigger> stateMachine = new StateMachine<State, Trigger>(State.Offline);
            return stateMachine;
            //var phoneCall = new StateMachine<State, Trigger>(State.OffHook);

            //phoneCall.Configure(State.OffHook)
            //    .Permit(Trigger.CallDialled, State.Ringing);

            //phoneCall.Configure(State.Ringing)
            //    .Permit(Trigger.CallConnected, State.Connected);

            //phoneCall.Configure(State.Connected)
            //    .OnEntry(() => StartCallTimer())
            //    .OnExit(() => StopCallTimer())
            //    .Permit(Trigger.LeftMessage, State.OffHook)
            //    .Permit(Trigger.PlacedOnHold, State.OnHold);

            //// ...

            //phoneCall.Fire(Trigger.CallDialled);
            //Assert.AreEqual(State.Ringing, phoneCall.State);
        }

        private void UnSync(object sender, RoutedEventArgs e)
        {

        }

        public async Task MainLoop()
        {

        }
    }
}
