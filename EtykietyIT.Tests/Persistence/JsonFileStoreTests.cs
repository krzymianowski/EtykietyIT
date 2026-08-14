using EtykietyIT.Models;
using EtykietyIT.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Persistence;

[TestClass]
public sealed class JsonFileStoreTests
{
    [TestMethod]
    public async Task SaveAndLoadAsync_RoundTripsDocument()
    {
        string directoryPath = CreateTemporaryDirectory();
        string filePath = Path.Combine(directoryPath, "document.json");

        try
        {
            var store = new JsonFileStore();
            var expected = new TestDocument(
                1,
                "Testowy dokument",
                ApplicationMode.Portable);

            await store.SaveAsync(filePath, expected);
            TestDocument? actual = await store.LoadAsync<TestDocument>(filePath);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            string json = await File.ReadAllTextAsync(filePath);
            StringAssert.Contains(json, "\"displayName\"");
            StringAssert.Contains(json, "\"mode\": \"portable\"");
            StringAssert.Contains(json, Environment.NewLine);
        }
        finally
        {
            Directory.Delete(directoryPath, true);
        }
    }

    [TestMethod]
    public async Task SaveAsync_ReplacesExistingDocument()
    {
        string directoryPath = CreateTemporaryDirectory();
        string filePath = Path.Combine(directoryPath, "document.json");

        try
        {
            var store = new JsonFileStore();
            var first = new TestDocument(1, "Pierwszy", ApplicationMode.Standard);
            var second = new TestDocument(1, "Drugi", ApplicationMode.Portable);

            await store.SaveAsync(filePath, first);
            await store.SaveAsync(filePath, second);

            TestDocument? actual = await store.LoadAsync<TestDocument>(filePath);

            Assert.AreEqual(second, actual);
            Assert.IsEmpty(Directory.GetFiles(directoryPath, "*.tmp"));
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

    private sealed record TestDocument(
        int SchemaVersion,
        string DisplayName,
        ApplicationMode Mode);
}
