using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace MinionsDB
{
    class Program
    {
        static string connectionString = "Server=.;Database=Minions;Trusted_Connection=True";

        static void Main(string[] args)
        {
            //VillainsNames();
            //MinionsNames();
            //AddMinions();
            DeleteEvil();
            //YearPassed();

        }
        /// <summary>
        /// Самый простой запрос, выборка всех без фильтров
        /// </summary>
        static void VillainsNames()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string selectionCommandString = "SELECT v.Name, " +
                    "COUNT(mv.MinionId) AS TotalMinions " +
                    "FROM MinionsVillains AS mv " +
                    "INNER JOIN Villains AS v ON mv.VillainId = v.Id " +
                    "GROUP BY v.Name  " +
                    "HAVING COUNT(mv.MinionId) > 3 " +
                    "ORDER BY TotalMinions DESC";
                SqlCommand command = new SqlCommand(selectionCommandString, connection);
                SqlDataReader reader = command.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader[i]} ");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        static void MinionsNames()
        {
            int Id = Convert.ToInt32(Console.ReadLine());
            GetVillainNameById(Id);
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string selectionCommandString =
                   "SELECT Minions.Name as mn, Minions.Age as ma " +
                    "FROM MinionsVillains " +
                    "JOIN Villains ON Villains.Id=VillainId " +
                    "JOIN Minions ON Minions.Id=MinionId " +
                    "WHERE VillainId=@id " +
                    "ORDER BY mn ASC";
                SqlCommand command = new SqlCommand(selectionCommandString, connection);
                command.Parameters.AddWithValue("@Id", Id);
                SqlDataReader reader = command.ExecuteReader();
                int count = 1;
                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{count++}. {reader["mn"]} {reader["ma"]}");
                    }
                }
            }
        }

        static string GetVillainNameById(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string selectionCommandString =
                    "SELECT Name FROM Villains WHERE Id=@id";
                SqlCommand command = new SqlCommand(selectionCommandString, connection);
                command.Parameters.AddWithValue("@id", id);

                string result = (string)command.ExecuteScalar();

                if (result == null)
                {
                    Console.WriteLine($"No villain with Id {id} exists in the database.");
                }
                else
                {
                    Console.WriteLine($"Villain: {result}");
                }

                return result;
            }

        }
        static void AddMinions()
        {
            string[] minion = Console.ReadLine().Split(' ');

            string minionName = minion[1];
            string minionAge = minion[2];
            string minionTown = minion[3];
            int townId;

            string[] villain = Console.ReadLine().Split(' ');

            string villainName = villain[1];

            if (IsTownAvailable(minionTown) == 0)
            {
                InsertTown(minionTown);
                Console.Write($"Город {minionTown} был добавлен в базу данных");
            }

            townId = GetTownIdByName(minionTown);
            if (IsVillainAvailable(villainName) == 0)
            {
                InsertVillain(villainName);
                Console.Write($"Злодей {villainName} был добавлен в базу данных");
            }
            InsertMinion(minionName, minionAge, townId);
            assignMinionToVillain(minionName, villainName);
            Console.WriteLine($"Успешно добавлен {minionName}, чтобы стать миньоном {villainName}");
        }

        static void InsertMinion(string name, string age, int townId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string insertCommand =
                    $"INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";
                SqlCommand command = new SqlCommand(insertCommand, connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@age", age);
                command.Parameters.AddWithValue("@townId", townId);
                command.ExecuteNonQuery();
            }
        }

        static int IsTownAvailable(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string selectCommand = $"SELECT Count(*) FROM Towns WHERE Name=@name";

                SqlCommand command = new SqlCommand(selectCommand, connection);
                command.Parameters.AddWithValue("@name", name);
                var result = (int)command.ExecuteScalar();
                return result;
            }
        }

        static int IsVillainAvailable(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string selectCommand = $"SELECT Count(*) FROM Villains WHERE Name=@name";

                SqlCommand command = new SqlCommand(selectCommand, connection);

                command.Parameters.AddWithValue("@name", name);
                var result = (int)command.ExecuteScalar();
                return result;
            }
        }

        static int GetTownIdByName(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string selectCommand = $"SELECT Id FROM Towns WHERE Name=@name";
                SqlCommand command = new SqlCommand(selectCommand, connection);
                command.Parameters.AddWithValue("@name", name);
                int id = Convert.ToInt32(command.ExecuteScalar());
                return id;
            }
        }

        static void InsertTown(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string insertCommand = $"INSERT INTO Towns (Name, CountryId) VALUES (@name, 1)";
                SqlCommand command = new SqlCommand(insertCommand, connection);
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
                Console.WriteLine($"Добавлен город {name}");
            }
        }


        static void InsertVillain(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string insertCommandString = $"INSERT INTO Villains (Name, EvilnessFactorId) VALUES (@name, 2)";
                SqlCommand command = new SqlCommand(insertCommandString, connection);
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
                Console.WriteLine($"Добавлен злодей {name}");
            }
        }

        private static int GetMinionIdByName(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string insertCommandString = $"SELECT Id FROM Minions WHERE Name=@name";
                SqlCommand command = new SqlCommand(insertCommandString, connection);
                command.Parameters.AddWithValue("@name", name);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private static int GetVillainIdByName(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string insertCommandString = $"SELECT Id FROM Villains WHERE Name=@name";
                SqlCommand command = new SqlCommand(insertCommandString, connection);
                command.Parameters.AddWithValue("@name", name);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        static void assignMinionToVillain(string minionName, string villainName)
        {
            int minionId = GetMinionIdByName(minionName);
            int villainId = GetVillainIdByName(villainName);

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string insertCommandString =
                    $"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";
                SqlCommand command = new SqlCommand(insertCommandString, connection);
                command.Parameters.AddWithValue("@minionId", minionId);
                command.Parameters.AddWithValue("@villainId", villainId);
                command.ExecuteNonQuery();
            }
        }

        static void DeleteEvil()
        {
            int id = Convert.ToInt32(Console.ReadLine());
            string villainName = GetVillainNameById(id);

            int count = GetVillainMinionsCount(id);
            DeleteFromMinionsVillainsByVillainId(id);

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string deleteCommandString = $"DELETE FROM Villains WHERE Id = @id";
                SqlCommand command = new SqlCommand(deleteCommandString, connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                Console.WriteLine($"Злодей {villainName} был удален");
                Console.WriteLine($"{count} миньонов было освобождено");
            }
        }

        static int GetVillainMinionsCount(int villainId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string insertCommandString = $"SELECT COUNT(*) FROM MinionsVillains WHERE VillainId = @id";
                SqlCommand command = new SqlCommand(insertCommandString, connection);
                command.Parameters.AddWithValue("@id", villainId);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count;
            }
        }

        static void DeleteFromMinionsVillainsByVillainId(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string deleteCommandString = $"DELETE FROM MinionsVillains WHERE VillainId = @id";
                SqlCommand command = new SqlCommand(deleteCommandString, connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }

        static void YearPassed()
        {
            string[] id = Console.ReadLine().Split(' ');

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                for (int i = 0; i < id.Length; i++)
                {
                    string updateCommandString = $"UPDATE Minions SET Age = Age + 1 WHERE Id = @id";
                    SqlCommand command = new SqlCommand(updateCommandString, connection);
                    command.Parameters.AddWithValue("@id", id[i]);
                    command.ExecuteNonQuery();
                }
            }

            showListOfMinions();
        }

        static void showListOfMinions()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string selectCommandString = $"SELECT Name, Age FROM Minions";
                SqlCommand command = new SqlCommand(selectCommandString, connection);
                SqlDataReader reader = command.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader[i]}");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}