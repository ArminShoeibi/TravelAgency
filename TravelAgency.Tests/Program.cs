using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TravelAgency.ProtocolBuffers;

namespace TravelAgency.Tests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri("https://localhost:5001/");

            List<Task> tasks = new();
            for (int i = 1; i <= 50; i++)
            {
                var tempIndex = i.ToString();
                var t = Task.Run(async () =>
                {
                    TicketStatusRequest ticketStatusRequest = new()
                    {
                        PNR = tempIndex,
                        TicketNumber = tempIndex
                    };
                    var ticketStatusRequestAsJson = JsonSerializer.Serialize(ticketStatusRequest);
                    //Console.WriteLine(ticketStatusRequestAsJson);

                    var ticketStatusRequestHttp = await httpClient.PostAsJsonAsync("api/TicketStatus", ticketStatusRequest);
                    var ticketStatusResponse = await ticketStatusRequestHttp.Content.ReadFromJsonAsync<TicketStatusResponse>();
                    var ticketStatusResponseAsJson = JsonSerializer.Serialize(ticketStatusResponse);
                 
                    Console.WriteLine($"{ticketStatusRequestAsJson}++{ticketStatusResponseAsJson}");
                });
                tasks.Add(t);
            }
            await Task.WhenAll(tasks);

            Console.ReadLine();
        }
    }
}
