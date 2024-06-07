namespace Badminton.Business.Shared;

public static class OrderShared
{
    public static List<string> Type()
    {
        string[] Type = { "Paid", "UnPaid" };
        return new List<string>(Type);
    }
}