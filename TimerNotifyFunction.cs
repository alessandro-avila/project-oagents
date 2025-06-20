using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.Azure.NotificationHubs;

namespace Company.Function
{
    public static class TimerNotifyFunction
    {
        // Connection string and hub name for Notification Hubs - replace with your values or set as app settings
        private static readonly string NotificationHubConnectionString = Environment.GetEnvironmentVariable("NotificationHubConnectionString");
        private static readonly string NotificationHubName = Environment.GetEnvironmentVariable("NotificationHubName");

        // Example URI of task service or database API endpoint that returns tasks with due dates
        private static readonly string TaskApiEndpoint = Environment.GetEnvironmentVariable("TaskApiEndpoint");

        // HttpClient is intended to be reused for better performance
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("TimerNotifyFunction")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"TimerNotifyFunction executed at: {DateTime.UtcNow}");

            try
            {
                // Determine the time window: now to next hour UTC
                DateTime now = DateTime.UtcNow;
                DateTime nextHour = now.AddHours(1);

                // Query tasks API for tasks due in next hour
                // The API is expected to accept query parameters startDue and endDue in ISO8601 format
                string requestUri = $"{TaskApiEndpoint}?startDue={now:o}&endDue={nextHour:o}";

                log.LogInformation($"Requesting tasks due between {now:o} and {nextHour:o}");

                HttpResponseMessage response = await httpClient.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    log.LogError($"Failed to retrieve tasks. Status code: {response.StatusCode}");
                    return;
                }

                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize tasks - expecting a JSON array of task objects with id, title, dueDate, assignedUser
                List<TaskItem> tasks = JsonSerializer.Deserialize<List<TaskItem>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tasks == null || tasks.Count == 0)
                {
                    log.LogInformation("No tasks due in the next hour.");
                    return;
                }

                // Initialize Notification Hub client
                NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(NotificationHubConnectionString, NotificationHubName);

                foreach (var task in tasks)
                {
                    // Compose notification message
                    string message = $"Task due soon: \"{task.Title}\" is due at {task.DueDate.ToLocalTime():f}.";

                    // Create payload for a generic notification (template or native platform can be used)
                    // Here we send a simple template notification for all platforms
                    var notificationPayload = new
                    {
                        data = new { message }
                    };
                    string payloadJson = JsonSerializer.Serialize(notificationPayload);

                    // Send notification targeting the assigned user tag (assumes user is registered with tag "user:{assignedUser}")
                    string userTag = $"user:{task.AssignedUser}";

                    try
                    {
                        // Send template notification asynchronously
                        await hub.SendTemplateNotificationAsync(notificationPayload, userTag);
                        log.LogInformation($"Notification sent to user {task.AssignedUser} for task {task.Id}");
                    }
                    catch (Exception ex)
                    {
                        log.LogError($"Failed to send notification for task {task.Id} to user {task.AssignedUser}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Exception in TimerNotifyFunction: {ex.Message}");
            }
        }

        private class TaskItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public DateTime DueDate { get; set; }
            public string AssignedUser { get; set; }
        }
    }
}
