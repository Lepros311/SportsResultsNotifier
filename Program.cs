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
var boxScores = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'box_score')]");


// Format the data
var results = new StringBuilder();

foreach (var score in boxScores)
{
    // Extract relevant data from each score
    results.Append(score.InnerText); // Customize as needed
}



// Send the email
var smtpClient = new SmtpClient("smtp.google.com")
{
    Port = 587,
    Credentials = new NetworkCredential("stricklycoding@gmail.com", "password"),
    EnableSsl = true,
};

var mailMessage = new MailMessage
{
    From = new MailAddress("stricklycoding@gmail.com"),
    Subject = "Daily Box Scores",
    Body = results.ToString(),
    IsBodyHtml = false,
};

mailMessage.To.Add("andy.strickland@gmail.com");

await smtpClient.SendMailAsync(mailMessage);