using System;
using System.Data.SqlClient;
using Nano.Net;

namespace FillDatabaseAddresses
{
    class Program
    {
        static void Main(string[] args)
        {
            var targets = new
            {
                Addresses = new { Table = "Address", Seed = args[1] },
                WelcomeAddresses = new { Table = "welcome.Address", Seed = args[2] }
            };

            var target = targets.WelcomeAddresses;

            using (var connection = new SqlConnection(args[0]))
            {
                connection.Open();

                uint max = 0;
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT MAX(AddressId) FROM {target.Table}";
                    var result = command.ExecuteScalar();
                    if (!(result is DBNull))
                    {
                        max = Convert.ToUInt32(result);
                    }
                }

                for (uint outer = 0; outer < 1000; outer++)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"INSERT INTO {target.Table} (AddressId, Address) VALUES ";
                        for (uint inner = 1; inner <= 1000; inner++)
                        {
                            var id = max + (1000 * outer) + inner;
                            var account = new Account(target.Seed, id, "ban");
                            command.CommandText += $"({id}, '{account.Address}'), ";
                        }
                        command.CommandText = command.CommandText.TrimEnd(' ', ',');
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
