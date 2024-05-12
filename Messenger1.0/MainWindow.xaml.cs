using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Messenger1._0
{
    public partial class MainWindow : Window
    {
        private MyDbContext db = new MyDbContext();
     
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordBox.Password;

            User user = db.Users.FirstOrDefault(u => u.Login == login);

            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                
                MessageBox.Show("Login successful!");

               
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "C:\\Users\\Евгений\\Documents\\Учеба Академия ТОР\\Сетевое программирование\\ClientConsole\\ClientConsole\\bin\\Debug\\net8.0\\ClientConsole.exe";
                startInfo.Arguments = $"{login} {password}"; 
                Process.Start(startInfo);

                
                this.Close();
            }
            else
            {
                
                errorMessage.Text = "Invalid login or password. Please try again.";
            }
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordBox.Password;

            if (db.Users.Any(u => u.Login == login))
            {
                errorMessage.Text = "This username is already taken. Please choose another one.";
                return;
            }

            byte[] passwordHash = HashPassword(password);

            User newUser = new User { Login = login, PasswordHash = passwordHash };
            db.Users.Add(newUser);
            db.SaveChanges();

            errorMessage.Text = "Registration successful. You can now log in.";
        }

        private byte[] HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return hashedPassword;
            }
        }

        private bool VerifyPassword(string password, byte[] storedHash)
        {
            byte[] hashedPassword = HashPassword(password);
            return storedHash.SequenceEqual(hashedPassword);
        }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public byte[] PasswordHash { get; set; }
    }

    public class MyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
