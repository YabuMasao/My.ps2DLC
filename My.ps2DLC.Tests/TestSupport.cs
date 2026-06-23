using System;
using System.Security.Principal;
using My.ps2DLC.DLL;

namespace My.ps2DLC.Tests;

internal static class TestSupport
{
    private const string RunIntegrationTestsVariable = "MY_PS2DLC_RUN_INTEGRATION_TESTS";

    public static bool CanRunIntegrationTests(TestContext testContext)
    {
        if (!string.Equals(Environment.GetEnvironmentVariable(RunIntegrationTestsVariable), "1", StringComparison.Ordinal))
        {
            testContext.WriteLine("Skipped. Set " + RunIntegrationTestsVariable + "=1 to run Enable/Disable integration tests.");
            return false;
        }

        if (!IsAdministrator())
        {
            testContext.WriteLine("Skipped. Enable/Disable integration tests require Administrator.");
            return false;
        }

        return true;
    }

    public static int CountByCategory(PS2DLCFileStatus[] fileStatuses, string category)
    {
        int count = 0;

        foreach (PS2DLCFileStatus fileStatus in fileStatuses)
        {
            if (string.Equals(fileStatus.Category, category, StringComparison.Ordinal))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsAdministrator()
    {
        using WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
