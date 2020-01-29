using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VkNet;
using VkNet.AudioBypassService.Extensions;

namespace VK
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        private ServiceCollection services = new ServiceCollection();
        private VkApi api;

        public LoginControl()
        {
            InitializeComponent();

            services.AddAudioBypass();
        }

        private async void LoginAsync()
        {
            api = new VkApi(services);

            ErrorText.Text = "Вход...";

            Task auth = api.AuthorizeAsync(
                   new VkNet.Model.ApiAuthParams()
                   {
                       Login = LoginField.Text,
                       Password = PasswordField.Password,
                       Settings = VkNet.Enums.Filters.Settings.All,
                       ApplicationId = 7295930
                   });

            try
            {
                await auth;
            }
            catch (VkNet.Exception.VkAuthorizationException)
            {
                ErrorText.Text = "Неправильный логин или пароль";
                return;
            }
            catch (VkNet.Exception.VkApiException)
            {
                ErrorText.Text = "Неизвестная ошибка";
                return;
            }
            catch (NullReferenceException)
            {
                ErrorText.Text = "NullReferenceException!";
                return;
            }
            catch (AggregateException)
            {
                ErrorText.Text = "Error!";
            }

            auth.Wait();

            LogInfo_firstName.Text = api.Account.GetProfileInfo().FirstName.ToString();
            LogInfo_lastName.Text = api.Account.GetProfileInfo().LastName.ToString();
            ErrorText.Text = "Успешно!";

            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.OpenMainPage_Func(api);
        }

        public void Login_Click(object sender, RoutedEventArgs e) => LoginAsync(); // Button Login Click
    }
}