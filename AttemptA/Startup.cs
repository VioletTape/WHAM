using System;
using Nancy.Hosting.Self;

namespace AttemptA {
    public class Startup {
        private static void Main(string[] args) {
            var hostConfiguration = new HostConfiguration {
                UrlReservations = new UrlReservations {CreateAutomatically = true}
            };


            using (var host = new NancyHost(hostConfiguration, new Uri("http://localhost:1234"))) {
                host.Start();
                Console.WriteLine("Running on http://localhost:1234");
                Console.ReadLine();
            }
        }
    }
}