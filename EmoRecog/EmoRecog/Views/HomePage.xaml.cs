using EmoRecog.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EmoRecog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        HomeViewModel vm;
        public HomePage()
        {
            InitializeComponent();
            vm = new HomeViewModel();
            BindingContext = vm;
        }
    }
}