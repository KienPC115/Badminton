namespace Badminton.Business.Shared;

public static class CourtShared
{
    public static List<string> Status()
    {
        string[] Status = { "Available", "Maintenance"};
        return new List<string>(Status);
    }
}