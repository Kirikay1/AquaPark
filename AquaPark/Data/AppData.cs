using AquaPark.Models;

namespace AquaPark.Data
{
    public class AppData
    {
        public static AquaParkContext db = new AquaParkContext();
        public static User? CurrentUser { get; set; }

    }
}
