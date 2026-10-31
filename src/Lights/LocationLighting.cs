namespace Cthangover.Mods.Time
{
    using Godot;
    /// <summary>One phase and portable-lamp policy for background scenes and tile views; fixed locations never change the game clock.</summary>
    public static class LocationLighting
    {
        public static Vector4 Weights(float hour, string mode, bool darkMode, bool useLight)
        {
            if (mode == "day") return new Vector4(0, 1, 0, 0);
            if (mode == "night" || darkMode) return new Vector4(0, 0, 0, 1);
            return useLight ? UiLightController.CalculateTimeWeights(hour) : new Vector4(0, 1, 0, 0);
        }
        public static bool LampAvailable(float hour, string mode, bool darkMode, bool useLight)
        {
            if (mode == "day") return false;
            if (mode == "night") return true;
            return darkMode || (useLight && (hour < 6 || hour >= 22));
        }
    }
}
