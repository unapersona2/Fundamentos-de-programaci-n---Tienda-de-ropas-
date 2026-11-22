using System;
using System.IO;

class Program
{
    static string[] nombres = {
        "Polo basico",
        "Camisa manga larga",
        "Jean clasico",
        "Short deportivo",
        "Gorra",
        "Vestido de fiesta largo",
        "Vestido casual",
        "Vestido de noche",
        "Falda plisada",
        "Blusa elegante",
        "Chompa de lana",
        "Casaca de jean",
        "Pantalon jogger",
        "Polera con capucha",
        "Bufanda"
    };

    static double[] precios = {
        35, 80, 120, 45, 25, 180, 90, 250, 60, 70, 110, 150, 95, 130, 40
    };

    static string[] colores = {
        "Negro", "Blanco", "Azul", "Rojo", "Verde", "Gris", "Beige"
    };

    static string[] tallas = {
        "XS", "S", "M", "L", "XL"
    };

    // Una sola tienda: un stock por prenda
    static int[] stock = new int[15];

    // Cantidad total comprada de cada prenda
    static int[] comprada = new int[15];

    static int numBoleta = 1001;
    static string carpeta = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TiendaRopa");
    static string archivoDatos = Path.Combine(carpeta, "datos.txt");
    static string archivoBoletas = Path.Combine(carpeta, "boletas.txt");

    // Copia en memoria de las boletas.
    // Asi todo funciona aunque el equipo no permita guardar archivos.
    static bool archivosDisponibles = true;
    static string boletasMem = "";

    static void Main()
    {
        int opcion;

        PrepararArchivos();
        LlenarStockInicial();
        CargarDatos();

        do
        {
            MostrarMenu();
            opcion = LeerEntero();

            try
            {
                if (opcion == 1) VerCatalogoYStock();
                else if (opcion == 2) EmitirBoleta();
                else if (opcion == 3) BuscarBoleta();
                else if (opcion == 4) VerPrendasCompradas();
                else if (opcion != 0) Console.WriteLine("Opcion invalida.");
            }
            catch (Exception ex)
            {
                // Muestra el error real para poder ubicarlo
                Console.WriteLine();
                Console.WriteLine("ERROR: " + ex.GetType().Name + " - " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

        } while (opcion != 0);

        GuardarDatos();
        Console.WriteLine("FIN DEL PROGRAMA");
    }

    static void MostrarMenu()
    {
        Console.WriteLine();
        Console.WriteLine("       TIENDA DE ROPA");
        Console.WriteLine("    CATALOGO Y STOCK");
        Console.WriteLine("  1. Ver catalogo y stock");
        Console.WriteLine("          VENTAS");
        Console.WriteLine("  2. Emitir boleta");
        Console.WriteLine("  3. Buscar boleta");
        Console.WriteLine("  4. Ver prendas compradas");
        Console.WriteLine("  0. Salir");
        Console.Write("Elija una opcion: ");
    }

    // Revisa si se pueden guardar archivos. Si no, el programa trabaja solo en memoria.
    static void PrepararArchivos()
    {
        try
        {
            Directory.CreateDirectory(carpeta);
            File.AppendAllText(archivoBoletas, "");

            boletasMem = File.ReadAllText(archivoBoletas);
        }
        catch (Exception)
        {
            archivosDisponibles = false;
            Console.WriteLine("Aviso: aqui no se pueden guardar archivos.");
            Console.WriteLine("Los datos se conservan solo mientras el programa este abierto.");
        }
    }

    // Convierte un texto en lineas (ignora lineas vacias)
    static string[] LeerLineas(string texto)
    {
        string[] partes = texto.Replace("\r", "").Split('\n');
        int cuenta = 0;

        for (int i = 0; i < partes.Length; i++)
        {
            if (partes[i] != "") cuenta++;
        }

        string[] resultado = new string[cuenta];
        int k = 0;

        for (int i = 0; i < partes.Length; i++)
        {
            if (partes[i] != "")
            {
                resultado[k] = partes[i];
                k++;
            }
        }

        return resultado;
    }

    static int LeerEntero()
    {
        int n;
        string texto = Console.ReadLine();

        while (!int.TryParse(texto, out n))
        {
            Console.Write("Ingrese un numero valido: ");
            texto = Console.ReadLine();
        }

        return n;
    }

    // Pide el numero de prenda hasta que este entre 1 y 15
    static int LeerPrenda()
    {
        Console.Write("Numero de prenda (1-15): ");
        int p = LeerEntero();

        while (p < 1 || p > 15)
        {
            Console.Write("Numero de prenda invalido. Ingrese un numero de 1 a 15: ");
            p = LeerEntero();
        }

        return p;
    }

    // Pide la cantidad hasta que este entre 1 y 50
    static int LeerCantidad()
    {
        Console.Write("Cantidad (1-50): ");
        int c = LeerEntero();

        while (c < 1 || c > 50)
        {
            Console.Write("Cantidad invalida. Ingrese un numero de 1 a 50: ");
            c = LeerEntero();
        }

        return c;
    }

    // Pide un texto que no puede quedar vacio
    static string LeerTexto(string mensaje)
    {
        Console.Write(mensaje);
        string texto = Console.ReadLine();

        while (texto == null || texto.Trim() == "")
        {
            Console.Write("No puede estar vacio. " + mensaje);
            texto = Console.ReadLine();
        }

        return texto.Trim();
    }

    // Pide un DNI de 8 digitos
    static string LeerDni()
    {
        Console.Write("DNI del cliente (8 digitos): ");
        string dni = Console.ReadLine();

        while (dni == null || dni.Trim().Length != 8 || !SoloNumeros(dni.Trim()))
        {
            Console.Write("DNI invalido. Ingrese 8 digitos: ");
            dni = Console.ReadLine();
        }

        return dni.Trim();
    }

    static bool SoloNumeros(string texto)
    {
        for (int i = 0; i < texto.Length; i++)
        {
            if (texto[i] < '0' || texto[i] > '9')
            {
                return false;
            }
        }

        return true;
    }


    static void LlenarStockInicial()
    {
        for (int p = 0; p < 15; p++)
        {
            stock[p] = 50;
        }
    }

    static int ObtenerStock(int p)
    {
        return stock[p - 1];
    }

    static void PonerStock(int p, int valor)
    {
        stock[p - 1] = valor;
    }


    static void GuardarDatos()
    {
        string[] lineas = new string[31];
        lineas[0] = numBoleta.ToString();

        // Guardar stock (una linea por prenda)
        for (int p = 0; p < 15; p++)
        {
            lineas[p + 1] = stock[p].ToString();
        }

        // Guardar prendas compradas
        for (int p = 0; p < 15; p++)
        {
            lineas[p + 16] = comprada[p].ToString();
        }

        if (!archivosDisponibles)
        {
            return;
        }

        try
        {
            File.WriteAllLines(archivoDatos, lineas);
        }
        catch (Exception)
        {
            archivosDisponibles = false;
        }
    }


    // Carga los datos guardados. Solo acepta valores validos;
    // si el archivo es viejo o esta danado, se quedan los datos iniciales.
    static void CargarDatos()
    {
        if (!archivosDisponibles || !File.Exists(archivoDatos))
        {
            return;
        }

        try
        {
            string[] lineas = File.ReadAllLines(archivoDatos);

            // Solo se acepta el formato exacto de esta version (31 lineas, sin comas).
            // Un archivo de una version anterior se ignora por completo.
            if (lineas.Length != 31)
            {
                return;
            }

            for (int i = 0; i < lineas.Length; i++)
            {
                if (lineas[i].Contains(","))
                {
                    return;
                }
            }

            int numeroGuardado;
            if (int.TryParse(lineas[0].Trim(), out numeroGuardado) &&
                numeroGuardado >= 1001 && numeroGuardado <= 1101)
            {
                numBoleta = numeroGuardado;
            }

            // Cargar stock (solo valores validos entre 0 y 50)
            for (int p = 0; p < 15; p++)
            {
                string[] partes = lineas[p + 1].Split(',');

                int cantidad;
                if (int.TryParse(partes[0].Trim(), out cantidad) &&
                    cantidad >= 0 && cantidad <= 50)
                {
                    stock[p] = cantidad;
                }
            }

            // Cargar compras si el archivo tiene el formato completo
            if (lineas.Length >= 31)
            {
                for (int p = 0; p < 15; p++)
                {
                    string[] partes = lineas[p + 16].Split(',');

                    int cantidadComprada;
                    if (int.TryParse(partes[0].Trim(), out cantidadComprada) &&
                        cantidadComprada >= 0)
                    {
                        comprada[p] = cantidadComprada;
                    }
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("No se pudo leer " + archivoDatos + ". Se usan los datos iniciales.");
        }
    }