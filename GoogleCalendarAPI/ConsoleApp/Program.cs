using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar};
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream = new FileStream("C:\\Repos\\Google_Calendar_API\\GoogleCalendarAPI\\ConsoleApp\\client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to :" + credPath);

            }

            //Create Google Calendar API Service
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //Define parameters of request
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            //ListEvents
            Events events = request.Execute();
            Console.WriteLine("Upcoming Events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (string.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }

            var calendars = service.CalendarList.List().ExecuteAsync();

            foreach (var item in calendars.Result.Items)
            {
                Console.WriteLine(item.Description);

            }

            Event myEvent = new Event
            {
                Summary = "Prueba Evento",
                Location = "Ecopulse",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Now,
                    TimeZone = "America/Los_Angeles"
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Now,
                    TimeZone = "America/Los_Angeles"
                },
  //              Recurrence = new String[] {
  //    "RRULE:FREQ=WEEKLY;BYDAY=MO"
  //},
                Attendees = new List<EventAttendee>()
      {
        new EventAttendee() { Email = "johndoe@gmail.com" }
      }
            };

            Event recurringEvent = service.Events.Insert(myEvent, "primary").Execute();


            Console.Read();
        }
    }
}
