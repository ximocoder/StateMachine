using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppStateMachine.StateMachines
{
    public class Invoices
    {
        enum Triggers
        {
            Receive,
            Send
        }

        enum States
        {
            Init,
            Received,
            Send
        }
        readonly StateMachine<States, Triggers> StateMachine;
        StateMachine<States, Triggers>.TriggerWithParameters<string> AssignTrigger;

        // Mapped by ORM
        public Invoice StateObject { get; set; }

        States StateI = States.Init;

        public Invoices()
        {
            StateObject = new Invoice() { Name = "test"};
            StateMachine = new StateMachine<States, Triggers>(() => StateI, s => StateI = s);
            AssignTrigger = StateMachine.SetTriggerParameters<string>(Triggers.Receive);

            StateMachine.Configure(States.Init)
                .Permit(Triggers.Receive, States.Received);

            StateMachine.Configure(States.Received)
                .Permit(Triggers.Send, States.Send); ;

            StateMachine.Configure(States.Send);
        }

        public void Go()
        {
            StateObject.Name = "processing...";
            StateMachine.Fire(Triggers.Receive);
        }

        public void Send()
        {
            StateObject.Name = "processed...";
            StateMachine.Fire(Triggers.Send);
        }
        public string ToDotGraph()
        {
            return UmlDotGraph.Format(StateMachine.GetInfo());
        }
    }
}
