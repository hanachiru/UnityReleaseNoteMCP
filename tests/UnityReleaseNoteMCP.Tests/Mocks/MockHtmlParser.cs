using UnityReleaseNoteMCP.Application;

namespace UnityReleaseNoteMCP.Tests.Mocks;

public class MockHtmlParser : IHtmlParser
{
    public string SummaryToReturn { get; set; } = "Mocked summary.";

    public string GetSummary(string htmlContent)
    {
        return SummaryToReturn;
    }
}
