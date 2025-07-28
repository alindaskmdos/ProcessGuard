using System;

namespace ProcessGuard.Services
{
    public static class PasswordManager
    {
        private static string? parentalPassword = null;

        public static bool IsPasswordSet()
        {
            return parentalPassword != null;
        }

        public static void SetPassword()
        {
            Console.WriteLine("Установка пароля для разблокировки:");
            Console.Write("Пароль: ");
            parentalPassword = ReadHiddenPassword();
            Console.WriteLine("\nПароль установлен!\n");
        }

        public static bool CheckPassword()
        {
            Console.Write("Пароль: ");
            string input = ReadHiddenPassword();
            Console.WriteLine();
            return input == parentalPassword;
        }

        private static string ReadHiddenPassword()
        {
            string password = "";

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            return password;
        }
    }
}
