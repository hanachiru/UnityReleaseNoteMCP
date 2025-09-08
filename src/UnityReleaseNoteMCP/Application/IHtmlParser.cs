namespace UnityReleaseNoteMCP.Application;

public interface IHtmlParser
{
    string GetSummary(string htmlContent);
}
