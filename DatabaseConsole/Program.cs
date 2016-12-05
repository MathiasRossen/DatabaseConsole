using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseConsole
{
    class Program
    {
        private static string connectionString = "Server=ealdb1.eal.local; Database=ejl10_db; User Id=ejl10_usr; Password=Baz1nga10;";
        static void Main(string[] args)
        {
            Console.Title = "Grill's vilde database program";
            Program prog = new Program();
            prog.Run();
        }

        private void Run()
        {
            bool running = true;
            string input;

            do
            {
                Console.Clear();
                DisplayMenu();
                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        InsertPetLoop();
                        break;

                    case "2":
                        InsertUserLoop();
                        break;

                    case "3":
                        DisplayPets();
                        break;

                    case "4":
                        Console.WriteLine("Owner last name: ");
                        input = Console.ReadLine();
                        DisplayOwnerByLastName(input);
                        break;

                    case "5":
                        Console.WriteLine("Owner e-mail: ");
                        input = Console.ReadLine();
                        DisplayOwnerByEmail(input);
                        break;

                    case "6":
                        Console.WriteLine("Owner id: ");
                        input = Console.ReadLine();
                        DisplayPetsByUserId(int.Parse(input));
                        break;

                    case "9":
                        Console.Clear();
                        Console.WriteLine("Query:");
                        Console.WriteLine();
                        input = Console.ReadLine();
                        Console.WriteLine();

                        if(input != "")
                            DestroyDatabase(input);
                        break;
                    default:
                        break;
                }
            }
            while (running);
        }

        private void DisplayMenu()
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine(" 1. Insert new pet");
            Console.WriteLine(" 2. Insert new owner");
            Console.WriteLine(" 3. Show all pets");
            Console.WriteLine(" 4. Show owner by last name");
            Console.WriteLine(" 5. Show owner by e-mail");
            Console.WriteLine(" 6. Show pets by owner id");
        }

        private void InsertPetLoop()
        {
            string[] petPars = new string[6];

            Console.Clear();
            Console.WriteLine("Pet name: ");
            petPars[0] = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Type: ");
            petPars[1] = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Breed: ");
            petPars[2] = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Weight: ");
            petPars[3] = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Adoption date: ");
            petPars[4] = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Pet owner's id: ");
            petPars[5] = Console.ReadLine();

            InsertPet(petPars[0], petPars[1], petPars[2], petPars[3], petPars[4], petPars[5]);
        }

        private void InsertUserLoop()
        {
            string[] userPars = new string[4];

            Console.Clear();

            Console.WriteLine("First name: ");
            userPars[0] = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Last name: ");
            userPars[1] = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("E-mail: ");
            userPars[2] = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Phone: ");
            userPars[3] = Console.ReadLine();
            Console.WriteLine();

            InsertOwner(userPars[0], userPars[1], userPars[2], userPars[3]);
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private void InsertPet(string petName, string petType, string petBreed, string petWeight, string petDOB, string petOwnerId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    SqlCommand insertPetCmd = new SqlCommand("InsertPet", con);
                    insertPetCmd.CommandType = CommandType.StoredProcedure;
                    insertPetCmd.Parameters.Add(new SqlParameter("petName", petName));
                    insertPetCmd.Parameters.Add(new SqlParameter("petType", petType));
                    insertPetCmd.Parameters.Add(new SqlParameter("petBreed", petBreed));
                    insertPetCmd.Parameters.Add(new SqlParameter("petWeight", petWeight));
                    insertPetCmd.Parameters.Add(new SqlParameter("petDOB", petDOB));
                    insertPetCmd.Parameters.Add(new SqlParameter("petOwnerId", petOwnerId));

                    insertPetCmd.ExecuteNonQuery();
                }
                catch(SqlException e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }

        private void InsertOwner(string firstName, string lastName, string email, string phone)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    SqlCommand createUserCmd = new SqlCommand("CreateOwner", con);
                    createUserCmd.CommandType = CommandType.StoredProcedure;
                    createUserCmd.Parameters.Add(new SqlParameter("ownerFirstName", firstName));
                    createUserCmd.Parameters.Add(new SqlParameter("ownerLastName", lastName));
                    createUserCmd.Parameters.Add(new SqlParameter("ownerEmail", email));
                    createUserCmd.Parameters.Add(new SqlParameter("ownerPhone", phone));

                    createUserCmd.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }

        private void DisplayPets()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("GetPets", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reader = cmd.ExecuteReader();

                    Console.Clear();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string id = reader["PetId"].ToString();
                            string name = reader["PetName"].ToString();
                            string type = reader["PetType"].ToString();
                            string breed = reader["PetBreed"].ToString();
                            string dob = reader["PetDOB"].ToString();
                            dob = dob.Substring(0, 10);
                            string weight = reader["PetWeight"].ToString();
                            string ownerFirstName = reader["OwnerFirstName"].ToString();
                            string ownerLastName = reader["OwnerLastName"].ToString();

                            if (breed.Length <= 7)
                                breed += "\t";

                            Console.WriteLine(id + "\t" + name + "\t" + type + "\t" + breed + "\t" + dob + "\t" + weight + "\t" + ownerFirstName + "\t" + ownerLastName);
                        }
                    }

                    Console.ReadLine();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }

        private void DisplayOwnerByLastName(string lastName)
        {
            using(SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {

                    con.Open();

                    SqlCommand cmd = new SqlCommand("GetOwnersByLastName", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("lastName", lastName));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Console.Clear();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string id = reader["OwnerId"].ToString();
                            string firstName = reader["OwnerFirstName"].ToString();
                            string ownerLastName = reader["OwnerLastName"].ToString();
                            string email = reader["OwnerEmail"].ToString();
                            string phone = reader["OwnerPhone"].ToString();

                            Console.WriteLine(id + " " + firstName + " " + ownerLastName + " " + email + " " + phone);
                        }
                    }

                    Console.ReadLine();
                }
                catch(SqlException e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }

        private void DisplayOwnerByEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("GetOwnersByEmail", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("email", email));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Console.Clear();

                    while (reader.Read())
                    {
                        string id = reader["OwnerId"].ToString();
                        string firstName = reader["OwnerFirstName"].ToString();
                        string lastName = reader["OwnerLastName"].ToString();
                        string ownerEmail = reader["OwnerEmail"].ToString();
                        string phone = reader["OwnerPhone"].ToString();

                        Console.WriteLine(id + " " + firstName + " " + lastName + " " + ownerEmail + " " + phone);
                    }

                    Console.ReadLine();
                }
                catch(SqlException e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }

        private void DisplayPetsByUserId(int id)
        {
            using(SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("GetAllPetsByOwnerId", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("ownerId", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Console.Clear();

                    while (reader.Read())
                    {
                        string firstName = reader["OwnerFirstName"].ToString();
                        string lastName = reader["OwnerLastName"].ToString();
                        string petName = reader["PetName"].ToString();
                        string petType = reader["PetType"].ToString();
                        string petBreed = reader["PetBreed"].ToString();
                        string avarageLife = reader["AvarageLifeExpectancy"].ToString();

                        Console.WriteLine(petName + " " + petType + " " + petBreed + " " + avarageLife + " " + firstName + " " + lastName);
                    }

                    Console.ReadLine();
                }
                catch(SqlException e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }

        private void DestroyDatabase(string input)
        {
            using(SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand("NewQuery", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = input;

                    cmd.ExecuteNonQuery();

                    try
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            string currentRow = "";

                            for(int i = 0; i < reader.VisibleFieldCount; i++)
                            {
                                currentRow += string.Format("{0,-10}", reader[i].ToString());
                            }

                            Console.WriteLine(currentRow);
                        }
                    }
                    catch(Exception)
                    {
                        Console.WriteLine("Query executed!");
                    }

                    Console.ReadLine();
                }
                catch(SqlException e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
        }
    }
}
