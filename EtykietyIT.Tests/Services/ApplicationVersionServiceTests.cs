using System.Text.RegularExpressions;
using EtykietyIT.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Services;

[TestClass]
public sealed class ApplicationVersionServiceTests
{
    [TestMethod]
    public void UserVersion_ComesFromAssemblyMetadataWithoutGitHash()
    {
        var service = new ApplicationVersionService();

        Assert.AreEqual("3.0.0-rc.1", service.UserVersion);
        Assert.IsFalse(string.IsNullOrWhiteSpace(service.DiagnosticVersion));
        Assert.StartsWith(service.UserVersion, service.DiagnosticVersion);
        Assert.IsFalse(Regex.IsMatch(
            service.UserVersion,
            "[0-9a-f]{40}",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
    }
}
