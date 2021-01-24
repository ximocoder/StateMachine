using ConsoleAppStateMachine.StateMachines;
using Polly;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleAppStateMachine
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //TestStatePhoneCall();
            //Invoices();
            //Bugs();

            await PollyAsync();
        }

        private static async Task PollyAsync()
        {
            // 3 retries
            var policy1 = Policy.Handle<Exception>().RetryAsync(3, (exception, attempt) =>
            {
                Console.WriteLine("Policy1 logging: " + exception.Message);

            });

            //Circuit Breaker
            var policy2 = Policy.Handle<HttpRequestException>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(5),
            (exception, timespan) =>
            {
                Console.WriteLine("Policy2 logging on break");

            },
            () =>
            {
                Console.WriteLine("Policy2 logging reset");
            },
            () =>
            {
                Console.WriteLine("Policy2 logging on half open");
            });
            var cs = policy2.CircuitState;

            //var policy2 = Policy.Handle<Exception>().CircuitBreakerAsync(3, TimeSpan.FromSeconds(5000));

            // We asign here for testing
            var policy = policy1;

            int i = 0;

            while (true)
            {
                try
                {
                    HttpClient httpc = new HttpClient();
                    // Retry the following call according to the policy - 3 times.
                    await policy.ExecuteAsync(async () =>
                    {
                        // This code is executed within the Policy 


                        //http://worldtimeapi.org/api/timezone/Europe/Stockholm

                        i += 1;
                        // Make a request and get a response
                        var result = await JsonSerializer.DeserializeAsync<TimeObject>
                        (await httpc.GetStreamAsync($"http://worldtimeapi.org/api/timezone/Europe/Stock"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                        // Display the response message on the console
                        Console.WriteLine("Response : " + result.datetime.ToString());
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Request " + i + " eventually failed with: " + e.Message);
                };
            }


            Console.ReadLine();

        }

        static void Bugs()
        {
            Bug bug = new Bug();

            Bugs bugs = new Bugs(bug);
            bugs.Assign("ximo");
            Console.WriteLine(bugs.ToDotGraph());
            bugs.Close();
            string dotGraph = bugs.ToDotGraph();
            Console.WriteLine(dotGraph);

        }

        static void Invoices()
        {
            Invoices inv = new Invoices();
            inv.Go();
        }

        static void TestStatePhoneCall()
        {
            var phoneCall = new PhoneCall("Lokesh");

            phoneCall.Print();
            phoneCall.Dialed("Prameela");
            phoneCall.Print();
            phoneCall.Connected();
            phoneCall.Print();
            phoneCall.SetVolume(2);
            phoneCall.Print();
            phoneCall.Hold();
            phoneCall.Print();
            phoneCall.Mute();
            phoneCall.Print();
            phoneCall.Unmute();
            phoneCall.Print();
            phoneCall.Resume();
            phoneCall.Print();
            phoneCall.SetVolume(11);
            phoneCall.Print();

            Console.WriteLine("Graph...");

            Console.WriteLine(phoneCall.ToDotGraph());

            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }
    }

    public class TimeObject
    {
        public string abbreviation { get; set; }
        public string client_ip { get; set; }
        public DateTime datetime { get; set; }
        public int day_of_week { get; set; }
        public int day_of_year { get; set; }
        public bool dst { get; set; }
        public object dst_from { get; set; }
        public int dst_offset { get; set; }
        public object dst_until { get; set; }
        public int raw_offset { get; set; }
        public string timezone { get; set; }
        public int unixtime { get; set; }
        public DateTime utc_datetime { get; set; }
        public string utc_offset { get; set; }
        public int week_number { get; set; }
    }
}
