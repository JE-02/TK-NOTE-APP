using Microsoft.Data.SqlClient;

internal class Program
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public User(string userName, string password)
        {
            this.Name = userName;
            this.Password = password;
        }

        public void CreateNote(SqlConnection connection)
        {
            try
            {
                Console.Write("Insert Note Title: ");
                string noteTitle = Console.ReadLine();
                Console.WriteLine("Insert Note Content");
                string noteContent = Console.ReadLine();

                string sql = $"INSERT INTO notes(noteTitle, noteContent, creatorId) VALUES ('{noteTitle}','{noteContent}', '{this.Id}')";
                SqlCommand createNote = new SqlCommand(sql, connection);
                createNote.ExecuteNonQuery();

                Console.WriteLine("THE NOTE HAS BEEN SUCCESFULLY CREATED!");
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("ERROR!");
                this.CreateNote(connection);
            }
        }

        public void ShowNote(SqlConnection connection)
        {
            
                Console.Write("Insert Note Title: ");
                string Title = Console.ReadLine();
                string sql = $"SELECT noteTitle FROM notes WHERE noteTitle = '{Title}' AND creatorId = {this.Id}";

                SqlCommand selectTitle = new SqlCommand(sql, connection);

                string noteTitle = Convert.ToString(selectTitle.ExecuteScalar());

               

                sql = $"SELECT noteContent FROM notes WHERE noteTitle = '{noteTitle}' AND creatorId = {this.Id}";

                SqlCommand selectNote = new SqlCommand(sql, connection);

               

                string noteContent = Convert.ToString(selectNote.ExecuteScalar());

            if (noteTitle != "")
            {
                Console.WriteLine(noteTitle);
                Console.WriteLine(noteContent);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("INVALID NOTE TITLE!");
                this.ShowNote(connection);
            }


        }

        public void DeleteNote(SqlConnection connection)
        {
            try
            {
                Console.WriteLine("Insert title of the note that you want to DELETE: ");
                string noteTitle = Console.ReadLine();

                string sql = $"DELETE FROM notes WHERE noteTitle = '{noteTitle}'";

                SqlCommand deleteNote = new SqlCommand(sql, connection);

                deleteNote.ExecuteNonQuery();

                Console.WriteLine($"NOTE: {noteTitle} SUCCESFULLY DELETED!");
            }
            catch
            {
                Console.Clear();
                Console.WriteLine("INVALID TITLE!");
                this.DeleteNote(connection);
            }
        }

        public void ShowListOfNotes(SqlConnection connection)
        {
            try
            {
                List<string> titles = new List<string>();

                string countOfNotes = $"SELECT COUNT(*) FROM notes";

                SqlCommand countNotes = new SqlCommand(countOfNotes, connection);

                int numberOfNotes = Convert.ToInt32(countNotes.ExecuteScalar());

                string selectTitles = $"SELECT noteTitle FROM notes WHERE creatorId = {this.Id}";
                SqlCommand showTitle = new SqlCommand(selectTitles, connection);

                using (SqlDataReader reader = showTitle.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        titles.Add(reader["noteTitle"].ToString());
                    }
                }

                int i = 1;

                foreach (string title in titles)
                {
                    Console.WriteLine($"{i} - {title}");
                    i++;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ExitSesion()
        {
            System.Environment.Exit(1);
        }
    }

    public static void Main()
    {
        // Creates Sql connection
        SqlConnectionStringBuilder connectionStringBuilder = new()
        {
            // Server name
            DataSource = "localhost",
            // Database name
            InitialCatalog = "notes",
            // Integrated security se usa para windows authentic
            IntegratedSecurity = true,

            TrustServerCertificate = true
        };
        var cs = connectionStringBuilder.ConnectionString;

        using SqlConnection connection = new(cs);
        connection.Open();

        Console.WriteLine("Log in(1) or Sign up(2)?");

        bool exit = false;

        int option = Convert.ToInt32(Console.ReadLine());

        if (option == 1)
        {
            Console.WriteLine("user: ");
            string name = Console.ReadLine();
            Console.WriteLine("password: ");
            string password = Console.ReadLine();

            User userLogged = new User(name, password);

            Console.Clear();

            Program.LogIn(connection, userLogged);

            Console.WriteLine();

            do
            {
                Program.UserOptions(connection, userLogged);
                Console.WriteLine("Another action? Y/N");
                exit = (Convert.ToChar(Console.ReadLine()) == 'Y') ? true : (Convert.ToChar(Console.ReadLine()) == 'N') ? false : false;
                Console.Clear();
            } while (exit == true);
        }
        else if (option == 2)
        {
            Console.WriteLine("new user name: ");
            string user = Console.ReadLine();
            Console.WriteLine("new password: ");
            string password = Console.ReadLine();

            Console.Clear();

            // Creates a new user
            User newUser = new User(user, password);

            Program.SignUp(connection, newUser);

            Console.WriteLine();
            Console.Clear();
            do
            {
                Program.UserOptions(connection, newUser);
                Console.WriteLine("Another action? Y/N");
                exit = (Convert.ToChar(Console.ReadLine()) == 'Y') ? true : (Convert.ToChar(Console.ReadLine()) == 'N') ? false : false;
                Console.Clear();
            } while (exit == true);
        }
        else
        {
            Console.WriteLine("Invalid Input");
        }
    }

    public static void UserOptions(SqlConnection connection, User userLogged)
    {
        Console.WriteLine("Select an option: ");
        Console.WriteLine("1 - Show list of notes");
        Console.WriteLine("2 - Create a new note");
        Console.WriteLine("3 - show note");
        Console.WriteLine("4 - Delete a note");
        Console.WriteLine("5 - Exit sesion");

        int option = Convert.ToInt32(Console.ReadLine());

        Console.Clear();

        if (option == 1)
        {
            userLogged.ShowListOfNotes(connection);
        }
        else if (option == 2)
        {
            userLogged.CreateNote(connection);
        }
        else if (option == 3)
        {
            userLogged.ShowNote(connection);
        }
        else if (option == 4)
        {
            userLogged.DeleteNote(connection);
        }
        else if (option == 5)
        {
            userLogged.ExitSesion();
        }
        else
        {
            Console.WriteLine("Invalid Input");
        }
    }

    // Logs into an existing User
    public static void LogIn(SqlConnection connection, User userLogged)
    {
        // Gets creator id
        string sql = $"SELECT userId FROM usersInfo WHERE userName = '{userLogged.Name}'";

        SqlCommand getID = new SqlCommand(sql, connection);

        userLogged.Id = Convert.ToInt32(getID.ExecuteScalar());

        // Counts number of users with this user name and password
        sql = $"SELECT COUNT(*) FROM usersInfo WHERE userName = '{userLogged.Name}' AND userPassword = '{userLogged.Password}'";

        SqlCommand selectUser = new SqlCommand(sql, connection);

        int count = Convert.ToInt32(selectUser.ExecuteScalar());

        // If this count is greater than 0 there is a user in the database with that information
        if (count > 0)
        {
            // User exist in Database
            Console.WriteLine("YOU SUCCESFULLY LOGGED");
        }
        else
        {
            // User does not exist in Database
            Console.WriteLine("INCORRECT VALUES");
        }
    }

    // Creates an User
    public static void SignUp(SqlConnection connection, User newUser)
    {
        // Inserts user in the Database
        string sql = $"INSERT INTO usersInfo(userName, userPassword) VALUES ('{newUser.Name}','{newUser.Password}')";

        SqlCommand createUser = new SqlCommand(sql, connection);

        createUser.ExecuteNonQuery();

        // Gets creator id
        sql = $"SELECT userId FROM usersInfo WHERE userName = '{newUser.Name}'";

        SqlCommand getID = new SqlCommand(sql, connection);

        newUser.Id = Convert.ToInt32(getID.ExecuteScalar());

        Console.WriteLine("SUCCESFULLY CREATED");
    }
}