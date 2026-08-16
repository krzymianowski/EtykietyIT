using EtykietyIT.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Diagnostics;

[TestClass]
public sealed class UiDiagnosticsServiceTests
{
    [TestMethod]
    public void IsRequested_ReturnsTrue_ForCaseInsensitiveArgument()
    {
        bool result = UiDiagnosticsService.IsRequested(
            ["--portable", "--UI-DIAGNOSTICS"]);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsRequested_ReturnsFalse_WhenArgumentIsMissing()
    {
        bool result = UiDiagnosticsService.IsRequested(["--portable"]);

        Assert.IsFalse(result);
    }
}
