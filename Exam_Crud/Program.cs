using MySql.Data.MySqlClient;
using System.Globalization;


class Program
{
    static string conn = "Server=localhost; Database=lenguaje_avanzado; UserId=root; Password=;";

    static void Main()
    {
        bool continuar = true;

        while (continuar)
        {
            Console.WriteLine("\nPlease select an option:");
            Console.WriteLine("1. View All Cars");
            Console.WriteLine("2. Add a New Car");
            Console.WriteLine("3. Update an Existing Car");
            Console.WriteLine("4. Exit");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    MostrarTodosLosCarros();
                    break;
                case "2":
                    AddNewCart();
                    break;
                case "3":
                    UpdateCar();
                    break;
                case "4":
                    continuar = false;
                    break;
                default:
                    Console.WriteLine("Opción no válida. Intente de nuevo.");
                    break;
            }
        }

    } //llave cierre del MAIN

    static void MostrarTodosLosCarros()
    {
        try
        {

            using (MySqlConnection cnx = new MySqlConnection(conn)){

            cnx.Open();                                                      //intentando abrir conexion
            string query = "SELECT * FROM Car";
            MySqlCommand cmd = new MySqlCommand(query, cnx);
            MySqlDataReader reader = cmd.ExecuteReader();
                                                                             // Separador
            Console.WriteLine(new string('-', 70));                                                               // Imprimir los encabezados de las columnas
            Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-5} {4,-10} {5,-12}", "ID", "Marca", "Modelo", "Año", "Precio", "Fecha Registro");
            Console.WriteLine(new string('-', 70));                          // Separador

            
            CultureInfo culture = new CultureInfo("en-US");                  // Crear un objeto CultureInfo para la cultura "en-US" (dólar)

            
            while (reader.Read())                                            // Imprimir los datos de cada carro
            { 
                DateTime dateAdded = Convert.ToDateTime(reader["DateAdded"]);
                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-5} {4,-10:C} {5,-12}",
                    reader["CarID"],
                    reader["Make"],
                    reader["Model"],
                    reader["Year"],
                    Convert.ToDecimal(reader["Price"]).ToString("C", culture), // Formatear el precio con la cultura especificada
                    dateAdded.ToString("yyyy-MM-dd"));
                } //while closing key
            }
        }catch (MySqlException ex)
        {
            Console.WriteLine($"Error de conexión a la base de datos: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error en el formato de los datos: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
        }

    } //lock key show all cars

    static void AddNewCart()
    {
        try
        {
            using (MySqlConnection cnx = new MySqlConnection(conn))
            {
                cnx.Open();

                // Solicitar al usuario que ingrese los datos del nuevo carro
                Console.WriteLine("Ingrese la Marca del carro:");
                string make = Console.ReadLine();

                Console.WriteLine("Ingrese el Modelo del carro:");
                string model = Console.ReadLine();

                Console.WriteLine("Ingrese el Año del carro:");
                int year = int.Parse(Console.ReadLine());

                Console.WriteLine("Ingrese el Precio del carro:");
                decimal price = decimal.Parse(Console.ReadLine());

                // Obtener la fecha actual automáticamente
                DateTime dateAdded = DateTime.Now;

                // Consulta SQL para insertar el nuevo carro
                string queryInsert = "INSERT INTO Car (Make, Model, Year, Price, DateAdded) " +
                                     "VALUES (@Make, @Model, @Year, @Price, @DateAdded)";  //parameters, to avoid SQL injection attacks and handle data securely.

                MySqlCommand cmdInsert = new MySqlCommand(queryInsert, cnx);
                cmdInsert.Parameters.AddWithValue("@Make", make);
                cmdInsert.Parameters.AddWithValue("@Model", model);
                cmdInsert.Parameters.AddWithValue("@Year", year);
                cmdInsert.Parameters.AddWithValue("@Price", price);
                cmdInsert.Parameters.AddWithValue("@DateAdded", dateAdded.ToString("yyyy-MM-dd"));

                // Ejecutar la inserción en la base de datos
                int filasAfectadas = cmdInsert.ExecuteNonQuery();

                // Notificar al usuario sobre el éxito de la operación
                if (filasAfectadas > 0)
                {
                    Console.WriteLine("¡El nuevo carro ha sido creado exitosamente!");
                }
                else
                {
                    Console.WriteLine("No se pudo insertar el carro en la base de datos.");
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Error de conexión a la base de datos: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error en el formato de los datos: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
        }
    }  //lock key AddNewCart


    static void UpdateCar(){

        try
        {

            using (MySqlConnection cnx = new MySqlConnection(conn))
        {
            cnx.Open();

            // Solicitar el CarID del carro a actualizar
            Console.WriteLine("Ingrese el ID del carro que desea actualizar:");
            int carID = int.Parse(Console.ReadLine());

            // Consultar los valores actuales del carro
            string querySelect = "SELECT * FROM Car WHERE CarID = @CarID";
            MySqlCommand cmdSelect = new MySqlCommand(querySelect, cnx);
            cmdSelect.Parameters.AddWithValue("@CarID", carID);
            MySqlDataReader reader = cmdSelect.ExecuteReader();

            if (reader.Read())
            {
                Console.WriteLine("Valores actuales del carro:");
                string makeActual = reader["Make"].ToString();
                string modelActual = reader["Model"].ToString();
                int yearActual = Convert.ToInt32(reader["Year"]);
                decimal priceActual = Convert.ToDecimal(reader["Price"]);
                DateTime dateAddedActual = Convert.ToDateTime(reader["DateAdded"]);

                Console.WriteLine($"Marca: {makeActual}");
                Console.WriteLine($"Modelo: {modelActual}");
                Console.WriteLine($"Año: {yearActual}");
                Console.WriteLine($"Precio: {priceActual}");
                Console.WriteLine($"Fecha de Registro: {dateAddedActual.ToString("yyyy-MM-dd")}");
            }
            else
            {
                Console.WriteLine("El carro con ese ID no existe.");
                return;
            }

            reader.Close();

            // Solicitar los nuevos valores o permitir que el usuario deje en blanco los campos para no actualizarlos
            Console.WriteLine("Ingrese la nueva Marca (o presione Enter para mantener la actual):");
            string newMake = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newMake)) newMake = null;

            Console.WriteLine("Ingrese el nuevo Modelo (o presione Enter para mantener el actual):");
            string newModel = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newModel)) newModel = null;

            Console.WriteLine("Ingrese el nuevo Año (o presione Enter para mantener el actual):");
            string newYearInput = Console.ReadLine();
            int? newYear = null;
            if (!string.IsNullOrWhiteSpace(newYearInput))
            {
                newYear = int.Parse(newYearInput);
            }

            Console.WriteLine("Ingrese el nuevo Precio (o presione Enter para mantener el actual):");
            string newPriceInput = Console.ReadLine();
            decimal? newPrice = null;
            if (!string.IsNullOrWhiteSpace(newPriceInput))
            {
                newPrice = decimal.Parse(newPriceInput);
            }

            // Construir la consulta SQL dinámica dependiendo de los campos que se deseen actualizar
            List<string> camposAActualizar = new List<string>();
            if (newMake != null) camposAActualizar.Add("Make = @Make");
            if (newModel != null) camposAActualizar.Add("Model = @Model");
            if (newYear != null) camposAActualizar.Add("Year = @Year");
            if (newPrice != null) camposAActualizar.Add("Price = @Price");

            if (camposAActualizar.Count > 0)
            {
                string queryUpdate = $"UPDATE Car SET {string.Join(", ", camposAActualizar)} WHERE CarID = @CarID";

                MySqlCommand cmdUpdate = new MySqlCommand(queryUpdate, cnx);

                // Asignar los valores actualizados solo si se ingresaron
                if (newMake != null) cmdUpdate.Parameters.AddWithValue("@Make", newMake);
                if (newModel != null) cmdUpdate.Parameters.AddWithValue("@Model", newModel);
                if (newYear != null) cmdUpdate.Parameters.AddWithValue("@Year", newYear);
                if (newPrice != null) cmdUpdate.Parameters.AddWithValue("@Price", newPrice);
                cmdUpdate.Parameters.AddWithValue("@CarID", carID);

                // Ejecutar la actualización
                int filasAfectadas = cmdUpdate.ExecuteNonQuery();
                if (filasAfectadas > 0)
                {
                    Console.WriteLine("¡Los cambios han sido guardados exitosamente!");
                }
                else
                {
                    Console.WriteLine("No se realizaron cambios.");
                }
            }
            else
            {
                Console.WriteLine("No se han actualizado los datos, ya que no se ingresaron nuevos valores.");
            }

            // Volver al menú principal
           // MostrarMenu();
        }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Error de conexión a la base de datos: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error en el formato de los datos: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
        }
    } //lock key updateCar
} //key close program class