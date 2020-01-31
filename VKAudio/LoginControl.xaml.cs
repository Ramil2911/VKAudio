using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
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

        private Configuration config;

        public LoginControl()
        {
            InitializeComponent();

            config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);

            services.AddAudioBypass();

            LoginAsync(true);
        }

        private async void LoginAsync(bool withKey)
        {
            Task auth;
            api = new VkApi(services);

            ErrorText.Text = "Вход...";

            if (!withKey)
            {
                auth = api.AuthorizeAsync(
                   new VkNet.Model.ApiAuthParams()
                   {
                       Login = LoginField.Text,
                       Password = PasswordField.Password,
                       Settings = VkNet.Enums.Filters.Settings.All,
                   });
            }
            else if (ConfigurationManager.AppSettings.Get("vkkey").ToString() == null)
            {
                auth = api.AuthorizeAsync(
                   new VkNet.Model.ApiAuthParams()
                   {
                       AccessToken = config.AppSettings.Settings["vkkey"].Value.ToString(),
                       Settings = VkNet.Enums.Filters.Settings.All,
                   });
            }
            else
            {
                ErrorText.Text = "";
                return;
            }

            try
            {
                if (!withKey)
                {
                    await api.AuthorizeAsync(
                       new VkNet.Model.ApiAuthParams()
                       {
                           Login = LoginField.Text,
                           Password = PasswordField.Password,
                           Settings = VkNet.Enums.Filters.Settings.All,
                       });
                }
                else if (ConfigurationManager.AppSettings.Get("vkkey").ToString() == null)
                {
                    await api.AuthorizeAsync(
                       new VkNet.Model.ApiAuthParams()
                       {
                           AccessToken = config.AppSettings.Settings["vkkey"].Value.ToString(),
                           Settings = VkNet.Enums.Filters.Settings.All,
                       });
                }
                else
                {
                    ErrorText.Text = "";
                    return;
                }
            }
            catch (VkNet.Exception.VkApiAuthorizationException)
            {
                ErrorText.Text = "Неправильный логин или пароль";
                return;
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
            catch (VkNet.AudioBypassService.Exceptions.VkAuthException)
            {
                ErrorText.Text = "Неправильный логин или пароль";
                return;
            }
            catch (AggregateException)
            {
                ErrorText.Text = "Error!";
                return;
            }

            ErrorText.Text = "Успешно!";

            config.AppSettings.Settings["vkkey"].Value = api.Token.ToString();

            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.OpenMainPage_Func(api);
        }

        public void Login_Click(object sender, RoutedEventArgs e) => LoginAsync(false); // Button Login Click
    }
}