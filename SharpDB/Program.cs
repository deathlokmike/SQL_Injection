using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;

namespace SharpDB
{
    class Program
    {
        static void Menu()
        {
            Console.WriteLine("\n |База данных хоккея:|\n" + 
                   "1. Посмотреть запись.\n" +
                   "2. Добавить запись.\n" +
                   "3. Обновить запись.\n" +
                   "4. Удалить запись.\n" +
                   "0. Закрыть приложение.\n");
        }
        private static bool Shielding(string str)
        {
            if ((str.Split('=').Length > 0) || (str.Split(')').Length > 0) || (str.Split(';').Length > 0) || (str.Split('\'').Length > 0) || (str.Split('#').Length > 0))
                return true;
            else
                return false;
        }
        private static bool Check_Security()
        {
            Console.WriteLine("Включить защиту от Sql injection?");
            string answ = Console.ReadLine();
            return answ == "-" ? false : true;
        }
        private static List<string>[] Select(MySqlConnection connection, string id, bool safe)
        {
            if (safe)
            {
                if (Shielding(id))
                {
                    Console.WriteLine("\nОшибка. Попытка нарушения целостности.\n");
                    return null;
                }
            }
            string query = "SELECT * FROM players WHERE ID_player=" + id + ";";
            List<string>[] list = new List<string>[4];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            list[3] = new List<string>();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                list[0].Add(dataReader["ID_player"] + "");
                list[1].Add(dataReader["Role"] + "");
                list[2].Add(dataReader["Name"] + "");
                list[3].Add(dataReader["Team"] + "");
            }
            dataReader.Close();
            return list;
        }
        private static void Insert(MySqlConnection connection, string id, string role, string name, string team, bool buf)
        {
            if (buf)
            {
                if (((Shielding(id)) && (Shielding(role)) && (Shielding(name)) && (Shielding(team))))
                {
                    Console.WriteLine("\nОшибка. Попытка нарушения целостности.\n");
                    return;
                }
            }
            string query = "INSERT INTO players (Team, Role, Name, ID_player) VALUES('" + team + "','" + role + "', '" + name + "', '" + id + "');";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }
        private static void Update(MySqlConnection connection, string id, string role, string name, string team, bool safe)
        {
            if (safe)
            {
                if (((Shielding(id)) && (Shielding(role)) && (Shielding(name)) && (Shielding(team))))
                {
                    Console.WriteLine("\nОшибка. Попытка нарушения целостности.\n");
                    return;
                }
            }
            string query = "UPDATE players SET Role='" + role + "', Team='" + team + "', Name='" + name + "' WHERE ID_player=" + id + ";";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }
        private static void Delete(MySqlConnection connection, string id, bool safe)
        {
            if (safe)
            {
                if (Shielding(id))
                {
                    Console.WriteLine("\nОшибка. Попытка нарушения целостности.\n");
                    return;
                }
            }
            string query = "DELETE FROM players WHERE ID_player=" + id + ";";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }
        static void Main()
        {
            MySqlConnection connection = new MySqlConnection("SERVER=localhost;DATABASE=hockey_by_matveev;UID=root;PASSWORD=21082002");
            connection.Open();
            while(true)
            {
                Menu();
                int a = Convert.ToInt32(Console.ReadLine());
                switch(a)
                {
                    case 1:
                        {
                            bool flag = Check_Security();
                            Console.WriteLine("Введите ID игрока:");
                            string id = Console.ReadLine();
                            var mass = Select(connection, id, flag);
                            if ((mass!=null)&&(mass[0].Count>0))
                            {
                                for (int i = 0; i < mass[0].Count; i++)
                                {
                                    Console.WriteLine("\nID: " + mass[0][i]);
                                    Console.WriteLine("Амплуа: " + mass[1][i]);
                                    Console.WriteLine("ФИО: " + mass[2][i]);
                                    Console.WriteLine("Команда: " + mass[3][i] + "\n");
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            bool flag = Check_Security();
                            Console.WriteLine("Введите данный игрока:");
                            Console.WriteLine("ID: ");
                            string id = Console.ReadLine();
                            Console.WriteLine("Амплуа: ");
                            string role = Console.ReadLine();
                            Console.WriteLine("ФИО: ");
                            string name = Console.ReadLine();
                            Console.WriteLine("Команда: ");
                            string team = Console.ReadLine();
                            Insert(connection, id, role, name, team, flag);
                            break;
                        }
                    case 3:
                        {
                            bool flag = Check_Security();
                            Console.WriteLine("Введите данные игрока:");
                            Console.WriteLine("ID для изменения: ");
                            string id = Console.ReadLine();
                            Console.WriteLine("Амплуа: ");
                            string role = Console.ReadLine();
                            Console.WriteLine("ФИО: ");
                            string name = Console.ReadLine();
                            Console.WriteLine("Команда: ");
                            string team = Console.ReadLine();
                            Update(connection, id, role, name, team, flag);
                            break;
                        }
                    case 4:
                        {
                            bool flag = Check_Security();
                            Console.WriteLine("Введите ID для удаления: ");
                            string id = Console.ReadLine();
                            Delete(connection, id, flag);
                            break;
                        }
                    case 0:
                        {
                            connection.Close();
                            return;
                        }
                    default: 
                        {
                            Console.WriteLine("\nОшибка ввода, введите значение из списка.\n");
                            break;
                        }
                }
            }
        
        }
        
    }
}

