using System.Net;

namespace MoopelFrontend.Tests;

public static class TestHelper
{
    /// <summary>
    /// Formats a diagnostic message for a failed HTTP assertion.
    /// </summary>
    public static async Task<string> TMsg(HttpStatusCode expected, HttpResponseMessage response)
    {
        ArgumentNullException.ThrowIfNull(response);

        string responseBody = await response.Content.ReadAsStringAsync() ?? "";

        return
            $"Expected status {expected}, " +
            $"actual {response.StatusCode}, " +
            $"endpoint {response.RequestMessage?.RequestUri}, " +
            $"raw body: {responseBody}";
    }
}
