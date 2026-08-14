using EtykietyIT.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Models;

[TestClass]
public sealed class AssetIdSettingsTests
{
    [TestMethod]
    public void Validate_AcceptsDefaultSettings()
    {
        var settings = new AssetIdSettings();

        settings.Validate();
    }

    [TestMethod]
    public void Validate_RejectsEmptyPrefix()
    {
        var settings = new AssetIdSettings { Prefix = " " };

        AssertInvalid(settings.Validate);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(13)]
    public void Validate_RejectsDigitsOutsideSupportedRange(int digits)
    {
        var settings = new AssetIdSettings { Digits = digits };

        AssertInvalid(settings.Validate);
    }

    private static void AssertInvalid(Action validation)
    {
        try
        {
            validation();
        }
        catch (InvalidOperationException)
        {
            return;
        }

        Assert.Fail("Oczekiwano błędu walidacji.");
    }
}
