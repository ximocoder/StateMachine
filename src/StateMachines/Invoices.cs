using Stateless;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppStateMachine.StateMachines
{
    public class Invoices
    {
        enum Triggers
        {
            Receive
        }

        enum States
        {
            Init,
            Received,
            Send
        }
        readonly StateMachine<string, Triggers> _stateMachine;
        StateMachine<States, Triggers>.TriggerWithParameters<string> _assignTrigger;

        // Mapped by EF
        public string State { get; set; }

        States _state = States.Init;

        public Invoices()
        {
            //State = "initial";
            //_stateMachine = new StateMachine<States, Triggers>(() => _state, s => State = string.Empty);
            //_assignTrigger = _stateMachine.SetTriggerParameters<string>(Triggers.Receive);

            //_stateMachine.Configure(States.Init)
            //    .Permit(Triggers.Receive, States.Received);
        }

        public void Go()
        {
            _stateMachine.Fire(Triggers.Receive);
        }
    }
}
