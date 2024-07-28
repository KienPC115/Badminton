namespace Badminton.Business.Shared;

public static class OrderShared
{
    public static List<string> Type()
    {
        string[] Type = { "Cash Payment", "Online Payment" };
        return new List<string>(Type);
    }
    public static List<string> Status()
    {
        string[] Status = { "Canceled", "Pending", "Paid" };
        return new List<string>(Status);
    }
}