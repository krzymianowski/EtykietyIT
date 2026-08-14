using EtykietyIT.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Models;

[TestClass]
public sealed class ApplicationSettingsTests
{
    [TestMethod]
    public void Validate_AcceptsSchemaVersionTwoWithActiveOrganization()
    {
        var settings = new ApplicationSettings
        {
            ActiveOrganizationProfileId = $"organization.{Guid.NewGuid():D}"
        };

        settings.Validate();
    }

    [TestMethod]
    public void Validate_RejectsMissingActiveOrganization()
    {
        var settings = new ApplicationSettings();

        Assert.ThrowsExactly<InvalidOperationException>(settings.Validate);
    }

    [TestMethod]
    public void Validate_RejectsLegacySchemaVersion()
    {
        var settings = new ApplicationSettings
        {
            SchemaVersion = 1,
            ActiveOrganizationProfileId = $"organization.{Guid.NewGuid():D}"
        };

        Assert.ThrowsExactly<InvalidOperationException>(settings.Validate);
    }
}
