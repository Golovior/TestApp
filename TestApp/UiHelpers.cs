using System.Windows.Forms;

namespace TestApp
{
    internal static class UiHelpers
    {
        public static bool ConfirmDelete(string message) =>
            MessageBox.Show(message, "Verwijderen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
    }
}
