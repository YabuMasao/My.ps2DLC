using System;
using My.ps2DLC.DLL;

namespace My.ps2DLC.Tests;

[TestClass]
public sealed class CheckTests
{
    [TestMethod]
    public void Check_CanBeCalled()
    {
        bool actual = PS2DLC.Check();

        Assert.IsInstanceOfType(actual, typeof(bool));
    }

    [TestMethod]
    public void Check_Returns18FileStatuses()
    {
        PS2DLCFileStatus[] fileStatuses;

        bool actual = PS2DLC.Check(out fileStatuses);

        Assert.IsInstanceOfType(actual, typeof(bool));
        Assert.HasCount(18, fileStatuses);
        Assert.AreEqual(16, TestSupport.CountByCategory(fileStatuses, "GAC"));
        Assert.AreEqual(2, TestSupport.CountByCategory(fileStatuses, "Registry"));

        foreach (PS2DLCFileStatus fileStatus in fileStatuses)
        {
            Assert.IsFalse(string.IsNullOrEmpty(fileStatus.FileName));
            Assert.IsFalse(string.IsNullOrEmpty(fileStatus.ResourceName));
            Assert.IsFalse(string.IsNullOrEmpty(fileStatus.Category));
            Assert.IsTrue(fileStatus.IsEmbedded);
        }
    }
}
