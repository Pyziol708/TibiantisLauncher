using System.Linq;
using System.Windows;
using TibiantisLauncher.Profiles;
using TibiantisLauncher.Validation;

namespace TibiantisLauncher.UI
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class YesNoWindow : Window
    {
        public string? Message { get; set; } = "Are you sure?";

        public YesNoWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Confirm();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void CloseIconButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Confirm()
        {
            DialogResult = true;
            Close();
        }

        private void Cancel()
        {
            DialogResult = false;
            Close();
        }
    }
}
