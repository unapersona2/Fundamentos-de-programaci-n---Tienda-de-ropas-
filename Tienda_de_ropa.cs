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
    static string carpeta = "TiendaRopa";
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
                else if (opcion == 4) MenuPrendasCompradas();
                else if (opcion != 0) Console.WriteLine("Opcion invalida.");
            }
            catch (Exception ex)
            {
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

    static int LeerPrenda()
    {
        Console.Write("Numero de prenda (1-" + nombres.Count + "): ");
        int p = LeerEntero();

        while (p < 1 || p > nombres.Count)
        {
            Console.Write("Numero de prenda invalido. Ingrese un numero de 1 a " + nombres.Count + ": ");
            p = LeerEntero();
        }

        return p;
    }

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
        for (int p = 0; p < stock.Count; p++)
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

        for (int p = 0; p < 15; p++)
        {
            lineas[p + 1] = stock[p].ToString();
        }

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

    static void CargarDatos()
    {
        if (!archivosDisponibles || !File.Exists(archivoDatos))
        {
            return;
        }

        try
        {
            string[] lineas = File.ReadAllLines(archivoDatos);

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

    // ---------------------------------------------------------
    //  1. CATALOGO Y STOCK
    // ---------------------------------------------------------

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

        if (LeerEntero() == 1)
        {
            AgregarPrenda();
        }
    }

    static void AgregarPrenda()
    {
        string nombre = LeerTexto("Nombre de la prenda: ");

        Console.Write("Precio sin IGV: S/ ");
        double precio;
        while (!double.TryParse(Console.ReadLine(), out precio) || precio <= 0)
        {
            Console.Write("Precio invalido. Ingrese un numero mayor a 0: ");
        }

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

    static bool UsaTalla(int p)
    {
        if (p == 5 || p == 15)
        {
            return false;
        }

        return true;
    }

    // ---------------------------------------------------------
    //  2. EMITIR BOLETA
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

        string pais = LeerTexto("Pais: ");
        string distrito = LeerTexto("Distrito: ");
        string calle = LeerTexto("Nombre de la calle: ");

        Console.Write("Sexo del cliente (1=Hombre, 2=Mujer): ");
        int sexo = LeerEntero();

        while (sexo < 1 || sexo > 2)
        {
            Console.Write("Opcion invalida. Ingrese 1=Hombre o 2=Mujer: ");
            sexo = LeerEntero();
        }

        string textoSexo = "Hombre";
        if (sexo == 2) textoSexo = "Mujer";

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

        List<int> stockAntes = new List<int>(stock);
        List<int> compradaAntes = new List<int>(comprada);

        double subtotal = 0;
        string detalle = "";
        int seguir = 1;

        while (seguir == 1)
        {
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
        boleta += "Sexo:     " + textoSexo + "\n";
        boleta += "Pais:     " + pais + "\n";
        boleta += "Distrito: " + distrito + "\n";
        boleta += "Calle:    " + calle + "\n";
        boleta += detalle;
        boleta += "Subtotal: S/ " + subtotal.ToString("0.00") + "\n";
        boleta += "IGV 18%:  S/ " + igv.ToString("0.00") + "\n";
        boleta += "TOTAL:    S/ " + total.ToString("0.00") + "\n";
        boleta += "Pago:     " + formaPago + "\n";
        boleta += "----------";

        Console.WriteLine();
        Console.WriteLine(boleta);

        Console.WriteLine();
        Console.Write("Esta de acuerdo con hacer la compra? (1=Si, 0=No): ");
        int acuerdo = LeerEntero();

        while (acuerdo != 0 && acuerdo != 1)
        {
            Console.Write("Opcion invalida. Esta de acuerdo? (1=Si, 0=No): ");
            acuerdo = LeerEntero();
        }

        if (acuerdo == 0)
        {
            for (int i = 0; i < stock.Count; i++)
            {
                stock[i] = stockAntes[i];
                comprada[i] = compradaAntes[i];
            }

            Console.WriteLine("Compra cancelada. No se emitio la boleta.");
            return;
        }

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

    // ---------------------------------------------------------
    //  3. BUSCAR BOLETA
    // ---------------------------------------------------------

    static void BuscarBoleta()
    {
        string[] lineas = LeerLineas(boletasMem);

        if (lineas.Length == 0)
        {
            Console.WriteLine("Todavia no hay boletas guardadas.");
            return;
        }

        int tipo = 0;

        do
        {
            Console.WriteLine("\n=== BUSCAR BOLETA ===");
            Console.WriteLine("1. Buscar por numero de boleta");
            Console.WriteLine("2. Buscar por DNI del cliente");
            Console.WriteLine("0. Volver al menu principal");
            Console.Write("Seleccione una opcion: ");
            tipo = LeerEntero();

            if (tipo == 1)
            {
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
            else if (tipo == 2)
            {
                string dni = LeerDni();

                bool enBoleta = false;
                bool coincide = false;
                bool hallada = false;
                string bloque = "";

                Console.WriteLine();

                for (int i = 0; i < lineas.Length; i++)
                {
                    if (lineas[i].StartsWith("BOLETA N "))
                    {
                        enBoleta = true;
                        coincide = false;
                        bloque = "";
                    }

                    if (enBoleta)
                    {
                        bloque += lineas[i] + "\n";

                        if (lineas[i].StartsWith("DNI:") && lineas[i].EndsWith(dni))
                        {
                            coincide = true;
                        }

                        if (lineas[i] == "----------")
                        {
                            if (coincide)
                            {
                                Console.Write(bloque);
                                Console.WriteLine();
                                hallada = true;
                            }

                            enBoleta = false;
                        }
                    }
                }

                if (!hallada)
                {
                    Console.WriteLine("No se encontraron boletas para ese DNI.");
                }
            }
            else if (tipo != 0)
            {
                Console.WriteLine("Opcion no valida. Intente de nuevo.");
            }

        } while (tipo != 0);
    }

    // ---------------------------------------------------------
    //  4. VER PRENDAS COMPRADAS Y EXPORTAR ARCHIVOS
    // ---------------------------------------------------------

    static void MenuPrendasCompradas()
    {
        int opcion = 0;

        do
        {
            Console.WriteLine("\n=== MENU DE COMPRAS ===");
            Console.WriteLine("1. Ver prendas compradas");
            Console.WriteLine("2. Guardar en archivo de texto (.txt)");
            Console.WriteLine("3. Guardar en archivo binario (.bin)");
            Console.WriteLine("0. Volver al menu principal");
            Console.Write("Seleccione una opcion: ");

            opcion = LeerEntero();

            switch (opcion)
            {
                case 1:
                    VerPrendasCompradas();
                    break;
                case 2:
                    GuardarEnTxt();
                    break;
                case 3:
                    GuardarEnBinario();
                    break;
                case 0:
                    break;
                default:
                    Console.WriteLine("Opcion no valida. Intente de nuevo.");
                    break;
            }

        } while (opcion != 0);
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

    static void GuardarEnTxt()
    {
        string rutaArchivo = Path.Combine(carpeta, "prendas_compradas.txt");

        using (StreamWriter writer = new StreamWriter(rutaArchivo))
        {
            writer.WriteLine("N".PadRight(4) + "Prenda".PadRight(26) + "Cantidad".PadLeft(10));
            writer.WriteLine(new string('-', 40));

            int totalComprado = 0;

            for (int p = 0; p < nombres.Count; p++)
            {
                totalComprado += comprada[p];
                writer.WriteLine((p + 1).ToString().PadRight(4) +
                                 nombres[p].PadRight(26) +
                                 comprada[p].ToString().PadLeft(10));
            }

            writer.WriteLine(new string('-', 40));
            writer.WriteLine("TOTAL".PadRight(30) + totalComprado.ToString().PadLeft(10));
        }

        Console.WriteLine("\nDatos guardados correctamente en '" + rutaArchivo + "'.");
    }

    static void GuardarEnBinario()
    {
        string rutaArchivo = Path.Combine(carpeta, "prendas_compradas.bin");

        using (BinaryWriter writer = new BinaryWriter(File.Open(rutaArchivo, FileMode.Create)))
        {
            for (int p = 0; p < nombres.Count; p++)
            {
                writer.Write(nombres[p] ?? "");
                writer.Write(comprada[p]);
            }
        }

        Console.WriteLine("\nDatos guardados correctamente en '" + rutaArchivo + "'.");
    }
}