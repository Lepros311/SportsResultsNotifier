using HtmlAgilityPack;
using System.Net;
using System.Net.Mail;
using System.Text;

// Scrape data from the website
var url = "https://www.basketball-reference.com/boxscores/";
var httpClient = new HttpClient();
var html = await httpClient.GetStringAsync(url);
var htmlDoc = new HtmlDocument();
htmlDoc.LoadHtml(html);

// Example XPath to select box scores
var boxScores = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'stats_table')]");

// Format the data
var results = new StringBuilder();

foreach (var score in boxScores)
{
    // Extract relevant data from each score
    results.Append(score.InnerText); // Customize as needed
}

// Send the email
using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
{
    //string emailAddress = Environment.GetEnvironmentVariable("EMAIL_ADDRESS");
    //string emailAppPassword = Environment.GetEnvironmentVariable("EMAIL_APP_PASSWORD");

    string emailAddress = "stricklycoding@gmail.com";
    string emailAppPassword = "grmacreubdqmjpgj";

    smtpClient.Credentials = new NetworkCredential(emailAddress, emailAppPassword);

    smtpClient.EnableSsl = true;


    MailMessage mailMessage = new MailMessage
    {
        From = new MailAddress("lepros311@gmail.com"),
        Subject = "Daily Box Scores",
        Body = results.ToString(),
        IsBodyHtml = false
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