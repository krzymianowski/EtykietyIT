using EtykietyIT.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtykietyIT.Tests.Models;

[TestClass]
public sealed class LabelProfileTests
{
    [TestMethod]
    public void Validate_AcceptsValidProfile()
    {
        var profile = CreateValidProfile();

        profile.Validate();
    }

    [TestMethod]
    public void Validate_RejectsProfileWithoutIdentity()
    {
        LabelProfile profile = CreateValidProfile() with { Id = string.Empty };

        AssertInvalid(profile.Validate);
    }

    [TestMethod]
    public void Validate_RejectsTooSmallLabel()
    {
        LabelProfile profile = CreateValidProfile() with { WidthMm = 19.9 };

        AssertInvalid(profile.Validate);
    }

    [TestMethod]
    public void Validate_RejectsLayoutWithoutColumns()
    {
        LabelProfile profile = CreateValidProfile() with { Columns = 0 };

        AssertInvalid(profile.Validate);
    }

    private static LabelProfile CreateValidProfile()
    {
        return new LabelProfile
        {
            Id = "builtin.89x41.2up",
            Name = "89 × 41 mm — 2 szt. w poziomie",
            WidthMm = 89.0,
            HeightMm = 41.0,
            Columns = 2,
            Rows = 1,
            DrawCutLines = true
        };
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
