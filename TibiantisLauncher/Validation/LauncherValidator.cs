using System.Diagnostics;

namespace TibiantisLauncher.Validation
{
    internal class LauncherValidator
    {
        public static void ValidateLauncherNotRunning()
        {
            Process[] processList = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

            if (processList.Length >= 2)
                throw new ValidationException("Another Tibiantis Launcher instance is already running.");
        }
    }
}
