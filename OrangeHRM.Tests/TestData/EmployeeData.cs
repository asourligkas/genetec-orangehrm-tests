namespace OrangeHRM.Tests.TestData;

public record EmployeeData(
    string FirstName,
    string LastName,
    string EmployeeId,
    string MiddleName = "",
    string OtherId = "",
    string Nationality = "",
    DateOnly? DateOfBirth = null)
{
    public static EmployeeData CreateUnique()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        return new EmployeeData($"QA{suffix}", "Demo", $"DM{suffix}");
    }
}
