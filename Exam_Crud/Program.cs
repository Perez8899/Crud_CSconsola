using MySql.Data.MySqlClient;
using System.Globalization;


class Program
{
    static void Main()
    {
        bool keepRunning = true;

        while (keepRunning)
        {
            Console.WriteLine("\nPlease select an option:");
            Console.WriteLine("1. View All Cars");
            Console.WriteLine("2. Add a New Car");
            Console.WriteLine("3. Update an Existing Car");
            Console.WriteLine("4. Exit \n");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    ShowAllCars();                
                    break;
                case "2":
                    AddNewCar();                 
                    break;
                case "3":
                    UpdateCar();
                    break;
                case "4":
                    keepRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.! ");
                    break;
            }
        }

    } // MAIN lock key

    // Method that returns a MySQL connection--------------------------------------------------------
    static MySqlConnection GetDatabaseConnection()
    {
        string connection = "Server=localhost; Database=lenguaje_avanzado; UserId=root; Password=;";
        MySqlConnection cnx = new MySqlConnection(connection);
        return cnx;
    }
    // ---------------------------------------------------------------------------------------------

    static void ShowAllCars()
    {
        try
        {
            using (MySqlConnection cnx = GetDatabaseConnection())                   //We call the function for the Connection
            {
                cnx.Open();                                                          //trying to open connection
                string query = "SELECT * FROM Car";
                MySqlCommand cmd = new MySqlCommand(query, cnx);

                MySqlDataReader reader = cmd.ExecuteReader();                       
                Console.WriteLine(new string('-', 67));  // Separator               // Print column headers
                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-5} {4,-12} {5,-12}", "ID", "Make", "Model", "Year", "Price", "Date Add");
                Console.WriteLine(new string('-', 67));                            // Separator

                CultureInfo culture = new CultureInfo("en-US");                 // Create a CultureInfo object for the culture "en-US" (dollar)


                while (reader.Read())                                            // Print the data of each car
                {
                    DateTime dateAdded = Convert.ToDateTime(reader["DateAdded"]);
                    Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-5} {4,-12:C} {5,-12}",
                        reader["CarID"],
                        reader["Make"],
                        reader["Model"],
                        reader["Year"],
                        Convert.ToDecimal(reader["Price"]).ToString("C", culture), // Format the price with the specified culture
                        dateAdded.ToString("yyyy-MM-dd"));
                } //while closing key
            
             }
          
        }catch (MySqlException ex)
        {
            Console.WriteLine($" Database connection error: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($" Error in data format: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }

    } //lock key show all cars

    static void AddNewCar()
    {
        try
        {
            using (MySqlConnection cnx = GetDatabaseConnection())//We call the function for the Connection
            {
                cnx.Open();

                                                               // Ask the user to enter the new cart details
                Console.WriteLine("\nEnter the brand of the car:");
                string make = Console.ReadLine();

                Console.WriteLine("Enter the car Model:");
                string model = Console.ReadLine();

                Console.WriteLine("Enter the Year of the car:");
                int year = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter the Price of the car:");
                decimal price = decimal.Parse(Console.ReadLine());

                                                           // Get current date automatically
                DateTime dateAdded = DateTime.Now;

                                                           // SQL query to insert the new cart
                string queryInsert = "INSERT INTO Car (Make, Model, Year, Price, DateAdded) " +
                                     "VALUES (@Make, @Model, @Year, @Price, @DateAdded)";  //parameters, to avoid SQL injection attacks and handle data securely.

                MySqlCommand cmdInsert = new MySqlCommand(queryInsert, cnx);
                cmdInsert.Parameters.AddWithValue("@Make", make);
                cmdInsert.Parameters.AddWithValue("@Model", model);
                cmdInsert.Parameters.AddWithValue("@Year", year);
                cmdInsert.Parameters.AddWithValue("@Price", price);
                cmdInsert.Parameters.AddWithValue("@DateAdded", dateAdded.ToString("yyyy-MM-dd"));

                                                         // Run insert into database
                int affectedRows = cmdInsert.ExecuteNonQuery();

                                                        // Notify the user about the success of the operation
                if (affectedRows > 0)
                {
                    Console.WriteLine("\n¡The new car has been created successfully!");
                }
                else
                {
                    Console.WriteLine(" Could not insert cart into database.");
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Database connection error: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error in data format: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }  //lock key AddNewCart


    static void UpdateCar(){

        try
        {

            using (MySqlConnection cnx = GetDatabaseConnection()) //We call the function for the Connection
            {
            cnx.Open();

                                // Request the CarID of the car to update
            Console.WriteLine("\nEnter the ID of the cart you want to UPDATE:");
            int carID = int.Parse(Console.ReadLine());

                                // Check current car values
            string querySelect = "SELECT * FROM Car WHERE CarID = @CarID";

            MySqlCommand cmdSelect = new MySqlCommand(querySelect, cnx);

            cmdSelect.Parameters.AddWithValue("@CarID", carID);

            MySqlDataReader reader = cmdSelect.ExecuteReader();

            if (reader.Read())
            {
                Console.WriteLine("Current car values:");
                string makeActual = reader["Make"].ToString();
                string modelActual = reader["Model"].ToString();
                int yearActual = Convert.ToInt32(reader["Year"]);
                decimal priceActual = Convert.ToDecimal(reader["Price"]);
                DateTime dateAddedActual = Convert.ToDateTime(reader["DateAdded"]);

                Console.WriteLine($"Make: {makeActual}");
                Console.WriteLine($"Model: {modelActual}");
                Console.WriteLine($"Year: {yearActual}");
                Console.WriteLine($"Price: {priceActual}");
                Console.WriteLine($"Date Add: {dateAddedActual.ToString("yyyy-MM-dd")}");
            }
            else
            {
                Console.WriteLine("The car with that ID does not exist.");
                return;
            }

            reader.Close();

                                // Prompt for new values ​​or allow the user to leave fields blank so as not to update them
             Console.WriteLine("\n Enter the new Brand (or press Enter to keep the current one):");
            string newMake = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newMake)) newMake = null;

            Console.WriteLine("Enter the new Model (or press Enter to keep the current one):");
            string newModel = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newModel)) newModel = null;

            Console.WriteLine("Enter the new Year (or press Enter to keep the current one):");
            string newYearInput = Console.ReadLine();
            int? newYear = null;
            if (!string.IsNullOrWhiteSpace(newYearInput))
            {
                newYear = int.Parse(newYearInput);
            }

            Console.WriteLine("Enter the new Price (or press Enter to keep the current one):");
            string newPriceInput = Console.ReadLine();
            decimal? newPrice = null;
            if (!string.IsNullOrWhiteSpace(newPriceInput))
            {
                newPrice = decimal.Parse(newPriceInput);
            }

                             // Build the dynamic SQL query depending on the fields you want to update
            List<string> updateFields = new List<string>();
            if (newMake != null) updateFields.Add("Make = @Make");
            if (newModel != null) updateFields.Add("Model = @Model");
            if (newYear != null) updateFields.Add("Year = @Year");
            if (newPrice != null) updateFields.Add("Price = @Price");

            if (updateFields.Count > 0){

                string queryUpdate = $"UPDATE Car SET {string.Join(", ", updateFields)} WHERE CarID = @CarID";

                MySqlCommand cmdUpdate = new MySqlCommand(queryUpdate, cnx);

                               // Assign updated values ​​only if they were entered
                if (newMake != null) cmdUpdate.Parameters.AddWithValue("@Make", newMake);
                if (newModel != null) cmdUpdate.Parameters.AddWithValue("@Model", newModel);
                if (newYear != null) cmdUpdate.Parameters.AddWithValue("@Year", newYear);
                if (newPrice != null) cmdUpdate.Parameters.AddWithValue("@Price", newPrice);
                cmdUpdate.Parameters.AddWithValue("@CarID", carID);

                    // Execute the update
                int affectedRows = cmdUpdate.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    Console.WriteLine("\n¡The changes have been saved Successfully!");
                }
                else
                {
                    Console.WriteLine("\n No changes were made.");
                }
            }
            else
            {
                Console.WriteLine("\n Data has not been updated, as no new values ​​were entered.");
            }

           }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Database connection error: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error in data format: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    } //lock key updateCar


} //key close program class