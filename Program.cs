﻿using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;


public static class ScraperFunction
{

    [FunctionName("ScraperFunction")]
    public static async Task Run([TimerTrigger("0 0 8 * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        // Scrape data from the website
        var url = "https://www.basketball-reference.com/boxscores/";
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        // Example XPath to select box scores
        var boxScores = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'stats_table')]");

        // Format the data
        var emailBody = new StringBuilder();

        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        CultureInfo userCulture = CultureInfo.CurrentCulture;
        string formattedDate = date.ToString(userCulture);

        if (boxScores != null)
        {
            emailBody.Append("<html><head><style>");
            emailBody.Append("table { border-collapse: collapse; width: 100%; max-width: 600px; margin: 0 auto; }");
            emailBody.Append("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            emailBody.Append("tr:nth-child(even) { background-color: #f2f2f2; }");
            emailBody.Append("</style></head><body>");

            emailBody.Append("<h2 style='text-align: center; width: 100%;'>NBA Standings and Stats</h2>"); // Add heading

            emailBody.Append($"<h3 style='text-align: center; width: 100%;'>for {date}</h3>"); // Add heading

            foreach (var table in boxScores)
            {
                // Remove text from caption tags
                var captionNode = table.SelectSingleNode(".//caption");
                if (captionNode != null)
                {
                    captionNode.InnerHtml = ""; // Clear the caption text
                }

                // Extract relevant data from each score
                var styledTable = table.OuterHtml
                    .Replace("<table", "<table style='border-collapse:collapse; width: 100%; max-width: 600px;'")
                    .Replace("<td", "<td style='border:1px solid #ddd;padding:8px' display:block; text-align:right;' data-label='Score'")
                    .Replace("<th", "<th style='background-color:#f2f2f2;border:1px solid #ddd;padding:8px'");

                emailBody.Append(styledTable);
            }

            emailBody.Append("</body></html>");
        }

        // Send the email
        using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
        {
            string emailAddress = Environment.GetEnvironmentVariable("EMAIL_ADDRESS");
            string emailAppPassword = Environment.GetEnvironmentVariable("EMAIL_APP_PASSWORD");

            smtpClient.Credentials = new NetworkCredential(emailAddress, emailAppPassword);

            smtpClient.EnableSsl = true;

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(emailAddress),
                Subject = "Daily Box Scores",
                Body = emailBody.ToString(),
                IsBodyHtml = true // Critical for HTML formatting
            };

            mailMessage.To.Add("andy.strickland@gmail.com");

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("\nEmail sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFailed to send email: {ex.Message}");
            }
        }

    }

}

