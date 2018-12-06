
namespace UsefulAttributes
{
    public class Attributes
    {
        public static void ToggleBool(ref bool boolToToggle)
        {
            if (boolToToggle)
            {
                boolToToggle = false;
            }
            else
            {
                boolToToggle = true;
            }
        }
    }
}
