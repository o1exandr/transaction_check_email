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
                        Email="peter.mail.ru", //peter@mail.ru
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
                            //якщо імейл валідний - додаємо в базу
                            if (EmailChecker(user.Email))
                            {
                                query = "INSERT INTO [dbo].[tblUsers] ([Firstname],[Lastname],[Email]) " +
                                $"VALUES ('{user.FirstName}','{user.LastName}','{user.Email}')";
                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }
                            else //інакше скасовуємо і жоден запис з масиву не зявиться в базі
                            {
                                Console.WriteLine("\nWrong Email!");
                            }

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

        // метод валідності емейлу
        public static bool EmailChecker(string email)
        {
            MailAddress mail = new MailAddress(email);
            if (mail.Address == email)
                return true;
            else
                return false;
        }
    }

    

}
