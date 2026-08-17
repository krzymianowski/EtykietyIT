using EtykietyIT.Printing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Printing;

[TestClass]
public sealed class LabelContentOptionsTests
{
    [TestMethod]
    public void Constructor_DefaultsQrToDisabledForLegacyRendering()
    {
        var options = new LabelContentOptions("Firma", "IT-", 6);

        Assert.IsFalse(options.QrEnabled);
    }
}
