using System;
using System.Security.Principal;

namespace My.ps2DLC.DLL
{
    public partial class PS2DLC
    {
        private static void EnsureAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                throw new InvalidOperationException("Administrator privileges are required.");
            }
        }
    }
}
