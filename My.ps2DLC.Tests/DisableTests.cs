using My.ps2DLC.DLL;

namespace My.ps2DLC.Tests;

[TestClass]
public sealed class DisableTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    [TestCategory("Integration")]
    public void Disable_WhenIntegrationEnabled_RemovesPayload()
    {
        if (!TestSupport.CanRunIntegrationTests(TestContext))
        {
            return;
        }

        Assert.IsTrue(PS2DLC.Enable());
        Assert.IsTrue(PS2DLC.Check());

        bool actual = PS2DLC.Disable();

        Assert.IsTrue(actual);
        Assert.IsFalse(PS2DLC.Check());
    }
}
