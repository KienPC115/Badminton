namespace Badminton.Business.Shared;

public static class OrderShared
{
    public static List<string> Type()
    {
        string[] Type = { "Cash Payment", "Online Payment" };
        return new List<string>(Type);
    }
}