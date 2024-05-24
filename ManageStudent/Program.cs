using Npgsql;

namespace StudentManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionDB = "Host=localhost;Username=postgres;Password=123;Database=postgres";

            using var connection = new NpgsqlConnection(connectionDB);
            connection.Open();

            bool exit = false;

            do
            {
                Console.WriteLine("************ Studnet Management ***********");
                Console.WriteLine("1. Insert student");
                Console.WriteLine("2. Select student by ID");
                Console.WriteLine("3. Select all students");
                Console.WriteLine("4. Delete student by ID");
                Console.WriteLine("5. Update student ");
                Console.WriteLine("6. Exit");
                Console.WriteLine("************ Studnet Management ***********");

                string? choice;
                Console.WriteLine("--------------------");
                Console.Write("Choose an option : ");
                choice = Console.ReadLine();
                Console.WriteLine("--------------------");

                switch (choice)
                {
                    case "1":
                        InsertStudent(connection);
                        break;
                    case "2":
                        SelectStudentById(connection);
                        break;
                    case "3":
                        SelectAllStudents(connection);
                        break;
                    case "4":
                        DeleteStudentById(connection);
                        break;
                    case "5":
                        UpdateStudent(connection);
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            } while (!exit);
        }

        static void InsertStudent(NpgsqlConnection connection)
        {
            Console.Write("Enter student name: ");
            string ?stu_name = Console.ReadLine();

            Console.Write("Enter student sex (M/F): ");
            string ?stu_sex = Console.ReadLine();

            Console.Write("Enter student position: ");
            string ?stu_position = Console.ReadLine();

            Console.Write("Enter student description: ");
            string ?stu_description = Console.ReadLine();

            using var cmd = new NpgsqlCommand("INSERT INTO students (stu_name, stu_sex, stu_position, stu_description) VALUES (@stu_name, @stu_sex, @stu_position, @stu_description)", connection);
            cmd.Parameters.AddWithValue("@stu_name", stu_name);
            cmd.Parameters.AddWithValue("@stu_sex", stu_sex);
            cmd.Parameters.AddWithValue("@stu_position", stu_position);
            cmd.Parameters.AddWithValue("@stu_description", stu_description);

            cmd.ExecuteNonQuery();

            Console.WriteLine("Student inserted successfully.");
        }

        static void SelectStudentById(NpgsqlConnection connection)
        {
            Console.Write("Enter student ID: ");
            int id = int.Parse(Console.ReadLine());

            using var cmd = new NpgsqlCommand("SELECT * FROM students WHERE stu_id = @id", connection);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())

            {
                Console.WriteLine("=======================================================================================================================");
                Console.WriteLine($"ID :|{reader["stu_id"],-10}| Name :|{reader["stu_name"],-10}| Sex :|{reader["stu_sex"],-10}| Position :|{reader["stu_position"],-10}| Description :|{reader["stu_description"],-10}|");
                Console.WriteLine("=======================================================================================================================");
            }
            else
            {
                Console.WriteLine("Student not found.");
            }
        }

        static void SelectAllStudents(NpgsqlConnection connection)
        {
            using var cmd = new NpgsqlCommand("SELECT * FROM students", connection);

            using var reader = cmd.ExecuteReader();
            Console.WriteLine("=======================================================================================================================");
            while (reader.Read())
            {
                Console.WriteLine($"ID :|{reader["stu_id"],-10}| Name :|{reader["stu_name"],-10}| Sex :|{reader["stu_sex"],-10}| Position :|{reader["stu_position"],-10}| Description :|{reader["stu_description"],-10}|");
            }
            Console.WriteLine("=======================================================================================================================");
        }

        static void DeleteStudentById(NpgsqlConnection connection)
        {
            Console.Write("Enter student ID: ");
            int id = int.Parse(Console.ReadLine());

            using var cmd = new NpgsqlCommand("DELETE FROM students WHERE stu_id = @id", connection);
            cmd.Parameters.AddWithValue("id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                Console.WriteLine("Student deleted successfully.");
            }
            else
            {
                Console.WriteLine("Student not found.");
            }
        }

        static void UpdateStudent(NpgsqlConnection connection)
        {
            Console.Write("Enter student ID to update: ");
            int stu_id = int.Parse(Console.ReadLine());

            bool exit = false;
            string ?stu_name = null;
            string ?stu_sex = null;
            string ?stu_position = null;
            string ?stu_description = null;

            do
            {
                Console.WriteLine("1. Update Name");
                Console.WriteLine("2. Update Sex");
                Console.WriteLine("3. Update Position");
                Console.WriteLine("4. Update Description");
                Console.WriteLine("5. Exit and Save");

                Console.WriteLine("--------------------");
                Console.Write("Choose an option: ");
                string ?choice = Console.ReadLine();
                Console.WriteLine("--------------------");

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter new student name: ");
                        stu_name = Console.ReadLine();
                        break;
                    case "2":
                        Console.Write("Enter new student sex (M/F): ");
                        stu_sex = Console.ReadLine();
                        break;
                    case "3":
                        Console.Write("Enter new student position: ");
                        stu_position = Console.ReadLine();
                        break;
                    case "4":
                        Console.Write("Enter new student description: ");
                        stu_description = Console.ReadLine();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            } while (!exit);

            var query = "UPDATE students SET ";
            var parameters = new List<NpgsqlParameter>();
            bool hasPreviousField = false;

            if (!string.IsNullOrWhiteSpace(stu_name))
            {
                query += "stu_name = @stu_name";
                parameters.Add(new NpgsqlParameter("@stu_name", stu_name));
                hasPreviousField = true;
            }

            if (!string.IsNullOrWhiteSpace(stu_sex))
            {
                if (hasPreviousField)
                {
                    query += ", ";
                }
                query += "stu_sex = @stu_sex";
                parameters.Add(new NpgsqlParameter("@stu_sex", stu_sex));
                hasPreviousField = true;
            }

            if (!string.IsNullOrWhiteSpace(stu_position))
            {
                if (hasPreviousField)
                {
                    query += ", ";
                }
                query += "stu_position = @stu_position";
                parameters.Add(new NpgsqlParameter("@stu_position", stu_position));
                hasPreviousField = true;
            }

            if (!string.IsNullOrWhiteSpace(stu_description))
            {
                if (hasPreviousField)
                {
                    query += ", ";
                }
                query += "stu_description = @stu_description";
                parameters.Add(new NpgsqlParameter("@stu_description", stu_description));
            }

            query += " WHERE stu_id = @stu_id";
            parameters.Add(new NpgsqlParameter("@stu_id", stu_id));

            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddRange(parameters.ToArray());

            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                Console.WriteLine("Student updated successfully.");
            }
            else
            {
                Console.WriteLine("Student not found or no changes made.");
            }
        }

    }
}
