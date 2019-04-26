using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mic.Volo.DBGenerate
{
    class Program
    {
        private static readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=true;";
        private static readonly string dbName = "PFM_Test";
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                if (args.Length == 1)
                {
                    Console.WriteLine("create, drop, init");
                }
                else
                {
                    if (args[1].Trim() == "create")
                    {
                        CreateDb();
                    }
                    if (args[1].Trim() == "delete")
                    {
                        DeleteDb();
                    }
                    if (args[1].Trim() == "init")
                    {
                        if (args.Length == 3)
                        {
                            if (int.TryParse(args[2], out int n))
                            {
                                InitDb(n);
                            }
                            else
                            {
                                Console.WriteLine($"failed to parser \"{args[2]}\"int");
                            }
                        }
                        else
                        {
                            Console.WriteLine("number of rows required");
                        }
                    }
                }
            }
        }

        private static void InitDb(int n)
        {
            using (PersonalFinanceManagment pfm = new PersonalFinanceManagment(connectionString))
            {
                pfm.InitDbAsync(dbName, n).Wait();
            }
        }

        private static void DeleteDb()
        {
            using (PersonalFinanceManagment pfm = new PersonalFinanceManagment(connectionString))
            {
                try
                {
                    pfm.DeletDbAsync(dbName).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void CreateDb()
        {
            using (PersonalFinanceManagment pfm = new PersonalFinanceManagment(connectionString))
            {
                try
                {
                    pfm.CreateDBAsync(dbName).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}


