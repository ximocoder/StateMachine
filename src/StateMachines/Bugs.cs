using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppStateMachine.StateMachines
{
    public class Bugs
    {
        enum State { Open, Assigned, Deferred, Resolved, Closed }
        enum Trigger { Assign, Defer, Resolve, Close }

        State _state = State.Open;
        StateMachine<State, Trigger> _machine;
        StateMachine<State, Trigger>.TriggerWithParameters<string> _assignTrigger;

        Bug _title;
        string _assignee;

        public Bugs(Bug title)
        {
            _title = title;

            _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

            _assignTrigger = _machine.SetTriggerParameters<string>(Trigger.Assign);

            _machine.Configure(State.Open)
                .Permit(Trigger.Assign, State.Assigned);

            _machine.Configure(State.Assigned)
                .SubstateOf(State.Open)
                .OnEntryFrom(_assignTrigger, assignee => OnAssigned(assignee))
                .PermitReentry(Trigger.Assign)
                .Permit(Trigger.Close, State.Closed)
                .Permit(Trigger.Defer, State.Deferred)
                .OnExit(() => OnDeassigned());

            _machine.Configure(State.Deferred)
                .OnEntry(() => _assignee = null)
                .Permit(Trigger.Assign, State.Assigned);
        }

        void OnAssigned(string assignee)
        {
            if (_assignee != null && assignee != _assignee)
                SendEmailToAssignee("Don't forget to help the new employee.");

            _assignee = assignee;
            SendEmailToAssignee("You own it.");
            SavetoDb(_title);
        }

        private void SavetoDb(Bug title)
        {
            
        }

        void OnDeassigned()
        {
            SendEmailToAssignee("You're off the hook.");
        }

        void SendEmailToAssignee(string message)
        {
            Console.WriteLine("{0}, RE {1}: {2}", _assignee, _title, message);
        }

        public void Close()
        {
            _machine.Fire(Trigger.Close);
        }

        public void Assign(string assignee)
        {
            _assignee = assignee;
            _machine.Fire(Trigger.Assign);
        }

        public string ToDotGraph()
        {
            return UmlDotGraph.Format(_machine.GetInfo());
        }
    }
}
