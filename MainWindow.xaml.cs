using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows;

namespace Mail_sender
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isSignUp = false;
        private User user;
        private List<User> users = new List<User>();
        private SmtpClient client;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SignUp(object sender, RoutedEventArgs e)
        {
            isSignUp = false;
            user = new User();
            foreach (User u in users)
            {
                if (u.Email == toEmail.Text && u.Password == toPassword.Password)
                {
                    isSignUp = true;
                    user = new User(toEmail.Text, toPassword.Password);
                    MessageBox.Show("Successfull!", "Email", MessageBoxButton.OK, MessageBoxImage.Information);
                    client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        EnableSsl = true,
                        Credentials = new NetworkCredential(user.Email, user.Password)
                    };
                }
            }
            if(!isSignUp)
            {
                toEmail.Text = toPassword.Password = "";
                MessageBox.Show("Error!", "Email", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AttachFile(MailMessage mail)
        {
            var result = MessageBox.Show("Do you want to attach a file?", "Email", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == true)
                {
                    mail.Attachments.Add(new Attachment(dialog.FileName));
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            User user = new User("prodoq@gmail.com", "bfcosbbxkfeuiokd");
            users.Add(user);
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {
            if (isSignUp)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"From : {user.Email}");
                sb.AppendLine($"Subject : {subjectTxtBox.Text}");
                sb.Append(bodyTxtBox.Text);
                MailPriority mailPriority = MailPriority.Normal;
                if ((bool)important.IsChecked)
                    mailPriority = MailPriority.High;
                MailMessage mail = new MailMessage(user.Email, toTxtBox.Text)
                {
                    Subject = subjectTxtBox.Text,
                    Body = sb.ToString(),
                    Priority = mailPriority
                };
                AttachFile(mail);
                try
                {
                    client.Send(mail);
                    MessageBox.Show("Mail sent!", "Email", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error : {ex.Message}", "Email", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("You must will sign up!", "Email", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Clean(object sender, RoutedEventArgs e)
        {
            bodyTxtBox.Text = toTxtBox.Text = subjectTxtBox.Text = "";
            important.IsChecked = false;
        }
    }
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public User()
        {

        }
        public User(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
