using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static CSharp16.User;

namespace CSharp16
{
    public class User
    {
        [Required(ErrorMessage = "Id is required")]
        [Range(1000, 9999, ErrorMessage = "Id must be between 1000 and 9999")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Login is required")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Login can only contain letters and numbers")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^[a-zA-Z0-9]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain only letters and numbers")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [RegularExpression(@"^[a-zA-Z0-9]{8,}$", ErrorMessage = "Confirm Password must be at least 8 characters long and contain only letters and numbers")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Credit Card is required")]
        [CreditCard(ErrorMessage = "Invalid credit card number")]
        public string CreditCard { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\+38-0\d{2}-\d{3}-\d{2}-\d{2}$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Serializable] 
        public class UserSerializable
        {
            public int Id { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
            public string Email { get; set; }
            public string CreditCard { get; set; }
            public string Phone { get; set; }

            public UserSerializable() { } 

            public UserSerializable(User user)
            {
                Id = user.Id;
                Login = user.Login;
                Password = user.Password;
                ConfirmPassword = user.ConfirmPassword;
                Email = user.Email;
                CreditCard = user.CreditCard;
                Phone = user.Phone;
            }
        }
    }

    internal class Program
    {
        private static Dictionary<int, User> _users = new Dictionary<int, User>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Register new user");
                Console.WriteLine("2. Load users from file");
                Console.WriteLine("3. Save users to file");
                Console.WriteLine("4. Exit");

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        RegisterUser();
                        break;
                    case 2:
                        LoadUsersFromFile();
                        break;
                    case 3:
                        SaveUsersToFile();
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
        }

        private static void RegisterUser()
        {
            while (true)
            {
                User user = new User();

                while (true) 
                {
                    Console.Write("Enter Id: ");
                    if (!int.TryParse(Console.ReadLine(), out int id) || id < 1000 || id > 9999)
                    {
                        Console.WriteLine("Invalid Id. Id must be between 1000 and 9999.");
                        continue; 
                    }
                    user.Id = id;
                    break; 
                }

                while (true) 
                {
                    Console.Write("Enter login: ");
                    user.Login = Console.ReadLine();
                    if (!Regex.IsMatch(user.Login, @"^[a-zA-Z0-9]+$"))
                    {
                        Console.WriteLine("Login can only contain letters and numbers");
                        continue; 
                    }
                    break; 
                }

                while (true) 
                {
                    Console.Write("Enter password: ");
                    string password = Console.ReadLine();
                    if (!Regex.IsMatch(password, @"^[a-zA-Z0-9]{8,}$"))
                    {
                        Console.WriteLine("Password must be at least 8 characters long and contain only letters and numbers");
                        continue; 
                    }
                    user.Password = password;
                    break; 
                }

                while (true) 
                {
                    Console.Write("Enter confirm password: ");
                    user.ConfirmPassword = Console.ReadLine();
                    if (user.Password != user.ConfirmPassword)
                    {
                        Console.WriteLine("Password and confirm password do not match");
                        continue; 
                    }
                    break; 
                }

                while (true) 
                {
                    Console.Write("Enter email: ");
                    user.Email = Console.ReadLine();
                    if (!Regex.IsMatch(user.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                    {
                        Console.WriteLine("Invalid email address");
                        continue; 
                    }
                    break; 
                }

                while (true) 
                {
                    Console.Write("Enter credit card: ");
                    user.CreditCard = Console.ReadLine();
                    if (!Regex.IsMatch(user.CreditCard, @"^[0-9]{13,16}$"))
                    {
                        Console.WriteLine("Invalid credit card number");
                        continue; 
                    }
                    break;
                }

                while (true)
                {
                    Console.Write("Enter phone: ");
                    user.Phone = Console.ReadLine();
                    if (!Regex.IsMatch(user.Phone, @"^[0-9]{10,12}$"))
                    {
                        Console.WriteLine("Invalid phone number");
                        continue; 
                    }
                    break; 
                }

                _users.Add(user.Id, user);
                Console.WriteLine("User registered successfully");
                break;
            }
        }


        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidCreditCard(string creditCardNumber)
        {
            
            string digitsOnly = Regex.Replace(creditCardNumber, @"\D", "");

            
            int sum = 0;
            bool doubleDigit = false;
            for (int i = digitsOnly.Length - 1; i >= 0; i--)
            {
                int digit = digitsOnly[i] - '0';
                if (doubleDigit)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }
                sum += digit;
                doubleDigit = !doubleDigit;
            }

            return sum % 10 == 0;
        }

        private static void SaveUsersToFile()
        {
            string filePath = "users.xml";

            List<UserSerializable> serializableUsers = _users.Values.Select(u => new UserSerializable(u)).ToList();

            using (FileStream stream = File.Create(filePath))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(List<UserSerializable>));
                formatter.Serialize(stream, serializableUsers);
            }

            Console.WriteLine("Users saved to file");
        }

        public class UserCollection
        {
            public List<User> Users { get; set; }

            public UserCollection(Dictionary<int, User> users)
            {
                Users = users.Values.ToList();
            }
        }

        private static void LoadUsersFromFile()
        {
            string filePath = "users.xml";

            if (File.Exists(filePath))
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(List<UserSerializable>));
                    List<UserSerializable> serializedUsers = (List<UserSerializable>)formatter.Deserialize(stream);

                    _users.Clear(); 

                    foreach (var serializedUser in serializedUsers)
                    {
                        User user = new User
                        {
                            Id = serializedUser.Id,
                            Login = serializedUser.Login,
                            Password = serializedUser.Password,
                            ConfirmPassword = serializedUser.ConfirmPassword,
                            Email = serializedUser.Email,
                            CreditCard = serializedUser.CreditCard,
                            Phone = serializedUser.Phone
                        };

                        _users.Add(user.Id, user);
                    }

                    Console.WriteLine("Users loaded from file");
                }
            }
            else
            {
                Console.WriteLine("File not found");
            }
        }

        private static void MergeUsers(Dictionary<int, User> loadedUsers)
        {
            Console.WriteLine("Loaded users:");
            foreach (var user in loadedUsers)
            {
                Console.WriteLine($"Id: {user.Key}, Login: {user.Value.Login}");
            }

            Console.WriteLine("Current users:");
            foreach (var user in _users)
            {
                Console.WriteLine($"Id: {user.Key}, Login: {user.Value.Login}");
            }

            Console.WriteLine("Choose action:");
            Console.WriteLine("1. Overwrite current users with loaded users");
            Console.WriteLine("2. Merge loaded users with current users");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    _users = loadedUsers;
                    break;
                case 2:
                    foreach (var user in loadedUsers)
                    {
                        if (!_users.ContainsKey(user.Key))
                        {
                            _users.Add(user.Key, user.Value);
                        }
                        else
                        {
                            Console.WriteLine($"User with Id {user.Key} already exists. Do you want to overwrite? (y/n)");
                            string response = Console.ReadLine();
                            if (response.ToLower() == "y")
                            {
                                _users[user.Key] = user.Value;
                            }
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}
