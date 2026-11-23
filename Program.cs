using System;
using System.Text;

namespace GiocoDiRuolo
{
    // Punto di ingresso del programma
    class Program
    {
        static void Main(string[] args)
        {
            // Serve per visualizzare correttamente le lettere accentate
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Le Prigioni di Grimmorn";

            Gioco gioco = new Gioco();
            gioco.Avvia();

            Console.WriteLine();
            Console.WriteLine("Grazie per aver giocato! Premi un tasto per chiudere.");
            Console.ReadKey(true);
        }
    }
}
