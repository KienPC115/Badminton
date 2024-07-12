namespace Badminton.Business.Shared;

public static class CourtShared
{
    public static List<string> Status()
    {
        return new List<string>() { "Available", "Maintenance" };
    }

    public static List<string> YardType() {
        return new List<string>() { "PVC carpet", "Acrylic paint", "Silicon PU", "Wood" };
    }

    public static List<string> Type() {
        return new List<string>() { "Single", "Dual", "Standard", "Self-Pratice" };
    }

    public static List<string> Location() {
        return new List<string>() { "Location A", "Location B", "Location C", "Location D" };
    }
    public static List<string> SpaceType() {
        return new List<string>() { "Indoor", "Outdoor" };
    }
}