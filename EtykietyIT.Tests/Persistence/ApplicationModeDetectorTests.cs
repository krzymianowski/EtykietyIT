using EtykietyIT.Models;
using EtykietyIT.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Persistence;

[TestClass]
public sealed class ApplicationModeDetectorTests
{
    [TestMethod]
    public void Detect_ReturnsPortable_WhenPortableArgumentIsPresent()
    {
        var detector = new ApplicationModeDetector();
        string nonExistingDirectory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString("N"));

        ApplicationMode result = detector.Detect(
            ["--PORTABLE"],
            nonExistingDirectory);

        Assert.AreEqual(ApplicationMode.Portable, result);
    }

    [TestMethod]
    public void Detect_ReturnsPortable_WhenMarkerFileExists()
    {
        string directoryPath = CreateTemporaryDirectory();

        try
        {
            File.WriteAllText(
                Path.Combine(
                    directoryPath,
                    ApplicationModeDetector.PortableMarkerFileName),
                string.Empty);

            var detector = new ApplicationModeDetector();
            ApplicationMode result = detector.Detect([], directoryPath);

            Assert.AreEqual(ApplicationMode.Portable, result);
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }

    [TestMethod]
    public void Detect_ReturnsStandard_WhenNoPortableSignalExists()
    {
        string directoryPath = CreateTemporaryDirectory();

        try
        {
            var detector = new ApplicationModeDetector();
            ApplicationMode result = detector.Detect([], directoryPath);

            Assert.AreEqual(ApplicationMode.Standard, result);
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }

    private static string CreateTemporaryDirectory()
    {
        string directoryPath = Path.Combine(
            Path.GetTempPath(),
            $"EtykietyIT.Tests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(directoryPath);
        return directoryPath;
    }
}
