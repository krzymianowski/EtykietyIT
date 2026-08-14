using EtykietyIT.Models;
using EtykietyIT.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Persistence;

[TestClass]
public sealed class AppDataPathsTests
{
    [TestMethod]
    public void Create_UsesLocalApplicationData_InStandardMode()
    {
        string localApplicationDataDirectory = Path.Combine(
            Path.GetTempPath(),
            $"LocalAppData.{Guid.NewGuid():N}");
        string executableDirectory = Path.Combine(
            Path.GetTempPath(),
            $"Executable.{Guid.NewGuid():N}");

        AppDataPaths paths = AppDataPaths.Create(
            ApplicationMode.Standard,
            executableDirectory,
            localApplicationDataDirectory);

        Assert.AreEqual(
            Path.Combine(localApplicationDataDirectory, "EtykietyIT", "v3"),
            paths.RootDirectory);
        Assert.IsFalse(Directory.Exists(paths.RootDirectory));
    }

    [TestMethod]
    public void Create_UsesDataNextToExecutable_InPortableMode()
    {
        string executableDirectory = Path.Combine(
            Path.GetTempPath(),
            $"Executable.{Guid.NewGuid():N}");

        AppDataPaths paths = AppDataPaths.Create(
            ApplicationMode.Portable,
            executableDirectory,
            string.Empty);

        Assert.AreEqual(
            Path.Combine(executableDirectory, "Data", "v3"),
            paths.RootDirectory);
        Assert.AreEqual(
            Path.Combine(paths.RootDirectory, "profiles"),
            paths.ProfilesDirectory);
        Assert.AreEqual(
            Path.Combine(paths.RootDirectory, "history", "print-history.jsonl"),
            paths.HistoryFilePath);
        Assert.IsFalse(Directory.Exists(paths.RootDirectory));
    }
}
