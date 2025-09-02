using AngleSharp;
using AngleSharp.Dom;
using System.Linq;
using UnityReleaseNoteMCP.Application;

namespace UnityReleaseNoteMCP.Infrastructure;

public class AngleSharpHtmlParser : IHtmlParser
{
    public string GetSummary(string htmlContent)
    {
        if (string.IsNullOrWhiteSpace(htmlContent))
        {
            return string.Empty;
        }

        var context = BrowsingContext.New(Configuration.Default);
        var document = context.OpenAsync(req => req.Content(htmlContent)).Result;

        // Try to find the first non-empty paragraph tag as a summary.
        // This is a heuristic and might need adjustment if the website structure changes.
        var firstParagraph = document.QuerySelectorAll("p").FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.TextContent));

        var summary = firstParagraph?.TextContent.Trim() ?? string.Empty;

        // As a fallback, if no good paragraph is found, take the first 200 chars of the body text.
        if (string.IsNullOrWhiteSpace(summary))
        {
            var bodyText = document.Body?.TextContent.Trim() ?? string.Empty;
            summary = new string(bodyText.Take(200).ToArray());
        }

        return summary;
    }
}
