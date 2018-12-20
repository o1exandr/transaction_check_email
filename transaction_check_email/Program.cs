using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Transactions;
using System.Net.Mail;

namespace transaction_check_email
{
    class Program
    {
     

        static void Main(string[] args)
        {
            string strCon = ConfigurationManager.AppSettings["ConnectionString"];
            List<User> users = new List<User>
                {
                    new User
                    {
                        Email="ivan@mail.ru",
                        FirstName="Ivan",
                        LastName="Petrenko",
                    },
                    new User
                    {
                        Email="peter@mail.ru",
                        FirstName="Petro",
                        LastName="Gugle",
                    },
                    new User
                    {
                        Email="jana@gmail.com",
                        FirstName="Jana",
                        LastName="Stuart",
                    }
                };
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection con = new SqlConnection(strCon))
                    {
                        con.Open();
                        SqlCommand command = new SqlCommand();
                        command.Connection = con;
                        string query = "";
                        foreach (var user in users)
                        {

                            int statusId = 0;
                            query = "SELECT Id FROM tblStatus " +
                                $"WHERE Name='{doctor.Status}'";
                            command.CommandText = query;
                            SqlDataReader reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                statusId = int
                                    .Parse(reader["Id"].ToString());
                            }
                            reader.Close();
                            if (statusId == 0)
                            {
                                query = "INSERT INTO [dbo].[tblStatus] " +
                                    $"([Name]) VALUES ('{doctor.Status}')";
                                command.CommandText = query;
                                var count = command.ExecuteNonQuery();
                                if (count == 1)
                                {
                                    query = "SELECT SCOPE_IDENTITY() AS LastId";
                                    command.CommandText = query;
                                    reader = command.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        statusId = int
                                            .Parse(reader["LastId"].ToString());
                                        Console.WriteLine("LastId = {0}", statusId);
                                    }
                                    reader.Close();
                                }
                                else { throw new Exception($"Проблема при добавлені статуса {doctor.FirstName}"); }
                            }

                            if (string.IsNullOrEmpty(doctor.Email))
                                throw new Exception($"Помилка при добавелі лікаря {doctor.Email}");
                            query = "INSERT INTO [dbo].[tblDoctor] " +
                                "([StatusId],[FirstName],[LastName],[Email],[Kabinet]) " +
                                $"VALUES ({statusId},'{doctor.FirstName}'," +
                                $"'{doctor.LastName}','{doctor.Email}','{doctor.Kabinet}')";
                            command.CommandText = query;
                            var res = command.ExecuteNonQuery();
                            if (res != 1)
                                throw new Exception($"Помилка при добавелі лікаря {doctor.Email}");
                        }


                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }

    // метод валідності емейлу
    static bool EmailChecker(string email)
    {
        MailAddress mail = new MailAddress(email);
        if (mail.Address == email)
            return true;
        else
            return false;
    }

}
