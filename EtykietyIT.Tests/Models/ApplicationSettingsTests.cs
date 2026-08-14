using EtykietyIT.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Models;

[TestClass]
public sealed class ApplicationSettingsTests
{
    [TestMethod]
    public void Validate_AcceptsDefaultSettings()
    {
        var settings = new ApplicationSettings();

        settings.Validate();
    }

    [TestMethod]
    public void Validate_RejectsEmptyCompanyName()
    {
        var settings = new ApplicationSettings { CompanyName = " " };

        AssertInvalid(settings.Validate);
    }

    [TestMethod]
    public void Validate_RejectsInvalidAssetIdSettings()
    {
        var settings = new ApplicationSettings
        {
            AssetId = new AssetIdSettings { Digits = 0 }
        };

        AssertInvalid(settings.Validate);
    }

    [TestMethod]
    public void Validate_RejectsNegativeNextAssetNumber()
    {
        var settings = new ApplicationSettings { NextAssetNumber = -1 };

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
