namespace Matt40k.SIMSBulkImport
{
    internal class Theme
    {
        public bool IsValidThemeName(string value)
        {
            switch (value)
            {
                case "Amber":
                    return true;
                case "Blue":
                    return true;
                case "Brown":
                    return true;
                case "Cobalt":
                    return true;
                case "Crimson":
                    return true;
                case "Cyan":
                    return true;
                case "Emerald":
                    return true;
                case "Green":
                    return true;
                case "Indigo":
                    return true;
                case "Lime":
                    return true;
                case "Magenta":
                    return true;
                case "Mauve":
                    return true;
                case "Olive":
                    return true;
                case "Orange":
                    return true;
                case "Pink":
                    return true;
                case "Purple":
                    return true;
                case "Red":
                    return true;
                case "Sienna":
                    return true;
                case "Steel":
                    return true;
                case "Teal":
                    return true;
                case "Violet":
                    return true;
                case "Yellow":
                    return true;
                default:
                    return false;
            }
        }

        public bool IsDarkTheme(string value)
        {
            switch (value)
            {
                case "BaseDark":
                    return true;
                case "BaseLight":
                    return false;
                default:
                    return false;
            }
        }
    }
}