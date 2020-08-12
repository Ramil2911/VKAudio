
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VKAudio;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using System.IO;
using System.Text;

namespace VK
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        private readonly ServiceCollection _services = new ServiceCollection();
        private VkApi _api;

        public LoginControl()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            _services.AddAudioBypass();
            _api = new VkApi(_services);
            LoginByToken();
            base.OnInitialized(e);
        }

        private async Task LoginByToken()
        {
            ErrorText.Text = "Вход...";
            Debug.Print("LoginByToken()");
            Debug.Print(File.ReadAllLines("C:\\ProgramData\\ramil2911\\VKAudio\\token", Encoding.Default)[0]);
            if (string.IsNullOrWhiteSpace(File.ReadAllLines("C:\\ProgramData\\ramil2911\\VKAudio\\token", Encoding.Default)[0])) return;
                
            if (!StaticFunctions.CheckForInternetConnection())
            {
                Debug.Print("PIZDA");
                MainWindow mainWindowa = Window.GetWindow(this) as MainWindow;
                mainWindowa?.OpenMainPage_Func(null);
                ErrorText.Text = "";
                return;
            }
                
            try
            {
                await _api.AuthorizeAsync(
                    new VkNet.Model.ApiAuthParams
                    {
                        AccessToken = File.ReadAllLines("C:\\ProgramData\\ramil2911\\VKAudio\\token", Encoding.Default)[0],
                        Settings = VkNet.Enums.Filters.Settings.All
                    });
                Debug.Print(_api.Token);
            }
            catch (Exception ex)
            {
                Debug.Print($"PIZDA2 {ex.Message}");
                ErrorText.Text = "";
                return;
            }
            ErrorText.Text = "";
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.OpenMainPage_Func(_api);
        }

        private async void LoginAsync()
        {

            try
            {
                await _api.AuthorizeAsync(
                    new VkNet.Model.ApiAuthParams()
                    {
                        Login = LoginField.Text,
                        Password = PasswordField.Password,
                        Settings = VkNet.Enums.Filters.Settings.All,
                    });
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
            if (!Directory.Exists("C:\\ProgramData\\ramil2911\\VKAudio"))
                Directory.CreateDirectory("C:\\ProgramData\\ramil2911\\VKAudio");
            if (!File.Exists("C:\\ProgramData\\ramil2911\\VKAudio\\token"))
                File.Create("C:\\ProgramData\\ramil2911\\VKAudio\\token");
            File.WriteAllText("C:\\ProgramData\\ramil2911\\VKAudio\\token", _api.Token.ToString());

            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.OpenMainPage_Func(_api);
        }

        public void Login_Click(object sender, RoutedEventArgs e) => LoginAsync(); // Button Login Click
    }
}