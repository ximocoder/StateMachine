using System;

namespace ConsoleAppStateMachine.StateMachines
{
    public class Bug
    {
        private string title;
        private DateTime date;
        private string info;

        public DateTime Date { get => date; set => date = value; }
        public string Title { get => title; set => title = value; }
        public string Info { get => info; set => info = value; }
    }
}