using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace NSModCreator
{
    public partial class Error : Window
    {
        public Error()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void OnOkClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
