using System;
using System.IO;
using System.Collections.Generic;
class Program
{
    static List<string> nombres = new List<string>
    {
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

    static List<double> precios = new List<double>
    {
        35, 80, 120, 45, 25, 180, 90, 250, 60, 70, 110, 150, 95, 130, 40
    };

    static string[] colores = {
        "Negro", "Blanco", "Azul", "Rojo", "Verde", "Gris", "Beige"
    };

    static string[] tallas = {
        "XS", "S", "M", "L", "XL"
    };

    // Una sola tienda: un stock por prenda
    static List<int> stock = new List<int>(new int[15]);

    // Cantidad total comprada de cada prenda
    static List<int> comprada = new List<int>(new int[15]);

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
        Console.Write("Numero de prenda (1-" + nombres.Count + "): ");
        int p = LeerEntero();

        while (p < 1 || p > nombres.Count)
        {
            Console.Write("Numero de prenda invalido. Ingrese un numero de 1 " + nombres.Count + ": ");
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

    static void VerCatalogoYStock()
    {
        int totalGeneral = 0;

        Console.WriteLine();
        Console.WriteLine("N".PadRight(4) + "Prenda".PadRight(26) +
                          "Precio sin IGV".PadLeft(16) + "Stock".PadLeft(8));
        Console.WriteLine(new string('-', 54));

        for (int p = 1; p <= nombres.Count; p++)
        {
            int cantidad = ObtenerStock(p);
            totalGeneral += cantidad;

            Console.WriteLine(p.ToString().PadRight(4) +
                              nombres[p - 1].PadRight(26) +
                              ("S/ " + precios[p - 1].ToString("0.00")).PadLeft(16) +
                              cantidad.ToString().PadLeft(8));
        }

        Console.WriteLine(new string('-', 54));
        Console.WriteLine("TOTAL".PadRight(46) + totalGeneral.ToString().PadLeft(8));
        Console.WriteLine("Tallas: XS, S, M, L, XL (se elige al comprar)");
        Console.WriteLine();
        Console.WriteLine("1. Agregar nueva prenda");
        Console.WriteLine("0. Volver al menu principal");
        Console.Write("Elija una opcion: ");
            if (LeerEntero() == 1) AgregarPrenda();
    
    }
static void AgregarPrenda()
{
    string nombre = LeerTexto("Nombre de la prenda: ");

    Console.Write("Precio sin IGV: S/ ");
    double precio;
    while (!double.TryParse(Console.ReadLine(), out precio) || precio <= 0)
        Console.Write("Precio invalido. Ingrese un numero mayor a 0: ");

    Console.Write("Stock inicial (0-50): ");
    int s = LeerEntero();
    while (s < 0 || s > 50)
    {
        Console.Write("Stock invalido. Ingrese de 0 a 50: ");
        s = LeerEntero();
    }

    nombres.Add(nombre);
    precios.Add(precio);
    stock.Add(s);
    comprada.Add(0);
    Console.WriteLine("Prenda agregada con el numero " + nombres.Count);
}
    static void VerColores()
    {
        Console.WriteLine("Colores disponibles:");

        for (int c = 0; c < colores.Length; c++)
        {
            Console.WriteLine("  " + (c + 1) + ". " + colores[c]);
        }
    }

    static void VerTallas()
    {
        Console.WriteLine("Tallas disponibles:");

        for (int i = 0; i < tallas.Length; i++)
        {
            Console.WriteLine("  " + (i + 1) + ". " + tallas[i]);
        }
    }

    // La gorra (5) y la bufanda (15) no llevan talla
    static bool UsaTalla(int p)
    {
        if (p == 5 || p == 15)
        {
            return false;
        }

        return true;
    }

    // ---------------------------------------------------------
    //  VENTAS
    // ---------------------------------------------------------

    static void EmitirBoleta()
    {
        if (numBoleta > 1100)
        {
            Console.WriteLine("Talonario agotado.");
            return;
        }

        string cliente = LeerTexto("Nombre del cliente: ");
        string dni = LeerDni();

        Console.Write("Forma de pago (1=Yape, 2=BCP): ");
        int pago = LeerEntero();

        while (pago < 1 || pago > 2)
        {
            Console.Write("Forma de pago invalida. Ingrese 1=Yape o 2=BCP: ");
            pago = LeerEntero();
        }

        string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        string formaPago = "Yape";

        if (pago == 2) formaPago = "BCP";

        double subtotal = 0;
        string detalle = "";
        int seguir = 1;

        while (seguir == 1)
        {
            // Si el numero de prenda no existe, solo se vuelve a pedir el numero
            int p = LeerPrenda();
            int cant = LeerCantidad();

            if (ObtenerStock(p) < cant)
            {
                Console.WriteLine("No se puede agregar: stock insuficiente. Solo hay " +
                                  ObtenerStock(p));
            }
            else
            {
                VerColores();
                Console.Write("Elija el color: ");
                int c = LeerEntero();

                while (c < 1 || c > colores.Length)
                {
                    Console.Write("Color invalido. Elija el color: ");
                    c = LeerEntero();
                }

                string talla = "Unica";

                if (UsaTalla(p))
                {
                    VerTallas();
                    Console.Write("Elija la talla: ");
                    int tl = LeerEntero();

                    while (tl < 1 || tl > tallas.Length)
                    {
                        Console.Write("Talla invalida. Elija la talla: ");
                        tl = LeerEntero();
                    }

                    talla = tallas[tl - 1];
                }

                PonerStock(p, ObtenerStock(p) - cant);
                comprada[p - 1] += cant;

                double importe = precios[p - 1] * cant;
                subtotal += importe;

                detalle += nombres[p - 1] + " | Talla " + talla +
                           " | " + colores[c - 1] +
                           " x" + cant + "  S/ " +
                           importe.ToString("0.00") + "\n";

                Console.WriteLine("Prenda agregada.");
            }

            Console.Write("Agregar otra prenda? (1=Si, 0=No): ");
            seguir = LeerEntero();

            while (seguir != 0 && seguir != 1)
            {
                Console.Write("Opcion invalida. Agregar otra prenda? (1=Si, 0=No): ");
                seguir = LeerEntero();
            }
        }

        if (subtotal == 0)
        {
            Console.WriteLine("No se agrego ninguna prenda. No se emite boleta.");
            return;
        }

        double igv = subtotal * 0.18;
        double total = subtotal + igv;

        string boleta = "BOLETA N " + numBoleta + "\n";
        boleta += "Fecha:    " + fecha + "\n";
        boleta += "Cliente:  " + cliente + "\n";
        boleta += "DNI:      " + dni + "\n";
        boleta += detalle;
        boleta += "Subtotal: S/ " + subtotal.ToString("0.00") + "\n";
        boleta += "IGV 18%:  S/ " + igv.ToString("0.00") + "\n";
        boleta += "TOTAL:    S/ " + total.ToString("0.00") + "\n";
        boleta += "Pago:     " + formaPago + "\n";
        boleta += "----------";

        Console.WriteLine();
        Console.WriteLine(boleta);

        boletasMem += boleta + "\n";

        if (archivosDisponibles)
        {
            try
            {
                File.AppendAllText(archivoBoletas, boleta + "\n");
                Console.WriteLine("Boleta guardada.");
            }
            catch (Exception)
            {
                archivosDisponibles = false;
            }
        }

        numBoleta++;
        GuardarDatos();
    }

    static void BuscarBoleta()
    {
        string[] lineas = LeerLineas(boletasMem);

        if (lineas.Length == 0)
        {
            Console.WriteLine("Todavia no hay boletas guardadas.");
            return;
        }

        Console.Write("Numero de boleta: ");
        int num = LeerEntero();

        bool mostrando = false;
        bool hallada = false;

        Console.WriteLine();

        for (int i = 0; i < lineas.Length; i++)
        {
            if (lineas[i] == "BOLETA N " + num)
            {
                mostrando = true;
                hallada = true;
            }

            if (mostrando)
            {
                Console.WriteLine(lineas[i]);

                if (lineas[i] == "----------")
                {
                    mostrando = false;
                }
            }
        }

        if (!hallada)
        {
            Console.WriteLine("Boleta no encontrada.");
        }
    }
    
    static void VerPrendasCompradas()
    {
        Console.WriteLine();
        Console.WriteLine("N".PadRight(4) + "Prenda".PadRight(26) + "Cantidad".PadLeft(10));
        Console.WriteLine(new string('-', 40));

        int totalComprado = 0;

        for (int p = 0; p < nombres.Count; p++)
        {
            totalComprado += comprada[p];

            Console.WriteLine((p + 1).ToString().PadRight(4) +
                              nombres[p].PadRight(26) +
                              comprada[p].ToString().PadLeft(10));
        }

        Console.WriteLine(new string('-', 40));
        Console.WriteLine("TOTAL".PadRight(30) + totalComprado.ToString().PadLeft(10));

        if (totalComprado == 0)
        {
            Console.WriteLine("Todavia no se han comprado prendas.");
        }
    }
}