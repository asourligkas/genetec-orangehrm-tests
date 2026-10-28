namespace OrangeHRM.Tests.TestData;

public record EmployeeData(string FirstName, string LastName, string EmployeeId, string MiddleName = "")
{
    public static EmployeeData CreateUnique()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        return new EmployeeData($"QA{suffix}", "Demo", $"DM{suffix}");
    }
}
