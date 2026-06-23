using My.ps2DLC.DLL;

namespace My.ps2DLC.Tests;

[TestClass]
public sealed class EnableTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    [TestCategory("Integration")]
    public void Enable_WhenIntegrationEnabled_InstallsPayload()
    {
        if (!TestSupport.CanRunIntegrationTests(TestContext))
        {
            return;
        }

        bool actual = PS2DLC.Enable();

        Assert.IsTrue(actual);
        Assert.IsTrue(PS2DLC.Check());
    }
}
