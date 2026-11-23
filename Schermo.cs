using System;

namespace GiocoDiRuolo
{
    // Si occupa di tutto quello che viene scritto sul terminale:
    // in alto la mappa, poi le statistiche, in basso i messaggi e le scelte.
    public class Schermo
    {
        private const int LARGHEZZA_TESTO = 72;
        private const int RIGHE_MESSAGGI = 12;

        // Teniamo solo gli ultimi RIGHE_MESSAGGI messaggi (gli unici che vengono mostrati).
        // Solo le prime "numeroMessaggi" celle dell'array sono usate.
        private string[] messaggi;
        private int numeroMessaggi;

        public Mappa Mappa { get; set; }
        public Giocatore Giocatore { get; set; }
        public Nemico NemicoInCombattimento { get; set; }

        public Schermo()
        {
            messaggi = new string[RIGHE_MESSAGGI];
            numeroMessaggi = 0;
        }

        // Aggiunge un messaggio al registro, andando a capo se è troppo lungo
        public void AggiungiMessaggio(string testo)
        {
            if (testo == "")
            {
                AggiungiRiga("");
                return;
            }

            string[] parole = testo.Split(' ');
            string riga = "";
            foreach (string parola in parole)
            {
                if (riga.Length + parola.Length + 1 > LARGHEZZA_TESTO)
                {
                    AggiungiRiga(riga);
                    riga = "  " + parola; // le righe successive sono rientrate
                }
                else if (riga == "")
                {
                    riga = parola;
                }
                else
                {
                    riga = riga + " " + parola;
                }
            }
            if (riga != "")
            {
                AggiungiRiga(riga);
            }
        }

        // Aggiunge una riga in fondo. Se l'array è pieno, la riga più vecchia
        // viene scartata e tutte le altre scalano di una posizione verso l'alto.
        private void AggiungiRiga(string riga)
        {
            if (numeroMessaggi == RIGHE_MESSAGGI)
            {
                for (int i = 0; i < RIGHE_MESSAGGI - 1; i++)
                {
                    messaggi[i] = messaggi[i + 1];
                }
                numeroMessaggi--;
            }
            messaggi[numeroMessaggi] = riga;
            numeroMessaggi++;
        }

        public void PulisciMessaggi()
        {
            for (int i = 0; i < RIGHE_MESSAGGI; i++)
            {
                messaggi[i] = null;
            }
            numeroMessaggi = 0;
        }

        public void Disegna(string[] opzioni)
        {
            Disegna(opzioni, "[W A S D] muovi   [INVIO] interagisci   [I] inventario   [Q] esci");
        }

        public void Disegna(string[] opzioni, string piede)
        {
            Console.Clear();

            if (Mappa != null)
            {
                DisegnaMappa();
                Console.WriteLine();
            }

            if (Giocatore != null)
            {
                DisegnaStatistiche();
            }

            Console.WriteLine(new string('=', LARGHEZZA_TESTO));

            // L'array contiene già solo gli ultimi messaggi
            int righeScritte = 0;
            for (int i = 0; i < numeroMessaggi; i++)
            {
                Console.WriteLine(messaggi[i]);
                righeScritte++;
            }
            // Righe vuote per tenere la parte bassa sempre della stessa altezza
            for (int i = righeScritte; i < RIGHE_MESSAGGI; i++)
            {
                Console.WriteLine();
            }

            Console.WriteLine(new string('-', LARGHEZZA_TESTO));

            if (opzioni != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                for (int i = 0; i < opzioni.Length; i++)
                {
                    Console.WriteLine("  " + (i + 1) + ") " + opzioni[i]);
                }
                Console.ResetColor();
                Console.WriteLine("Scegli un numero.");
            }
            else
            {
                Console.WriteLine(piede);
            }
        }

        private void DisegnaMappa()
        {
            for (int y = 0; y < Mappa.Altezza; y++)
            {
                for (int x = 0; x < Mappa.Larghezza; x++)
                {
                    char simbolo = Mappa.GetCella(x, y);
                    if (Giocatore != null && Giocatore.X == x && Giocatore.Y == y)
                    {
                        simbolo = '@';
                    }
                    Console.ForegroundColor = ColoreSimbolo(simbolo);
                    Console.Write(simbolo);
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        private ConsoleColor ColoreSimbolo(char simbolo)
        {
            switch (simbolo)
            {
                case '@': return ConsoleColor.Yellow;
                case '#': return ConsoleColor.DarkGray;
                case 'O': return ConsoleColor.Red;
                case 'G': return ConsoleColor.Red;
                case 'N': return ConsoleColor.Cyan;
                case 'C': return ConsoleColor.DarkYellow;
                case '+': return ConsoleColor.Magenta;
                case '>': return ConsoleColor.Green;
                default: return ConsoleColor.Gray;
            }
        }

        private void DisegnaStatistiche()
        {
            Console.WriteLine(Giocatore.Nome + " il " + Giocatore.Classe
                + "   PV " + Giocatore.PuntiVita + "/" + Giocatore.PuntiVitaMax
                + "   FOR " + Giocatore.Forza
                + "  INT " + Giocatore.Intelligenza
                + "  DES " + Giocatore.Destrezza
                + "   Pozioni: " + Giocatore.ContaPozioni());

            if (NemicoInCombattimento != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nemico: " + NemicoInCombattimento.Nome
                    + "   PV " + NemicoInCombattimento.PuntiVita + "/" + NemicoInCombattimento.PuntiVitaMax);
                Console.ResetColor();
            }
        }

        // Mostra le opzioni e aspetta che il giocatore prema un numero valido.
        // Restituisce l'indice della scelta partendo da 0. (Massimo 9 opzioni.)
        public int ChiediScelta(string[] opzioni)
        {
            while (true)
            {
                Disegna(opzioni);
                ConsoleKeyInfo tasto = Console.ReadKey(true);
                if (tasto.KeyChar >= '1' && tasto.KeyChar <= '9')
                {
                    int numero = tasto.KeyChar - '1';
                    if (numero < opzioni.Length)
                    {
                        return numero;
                    }
                }
            }
        }

        public void AttendiTasto()
        {
            Disegna(null, "Premi un tasto per continuare...");
            Console.ReadKey(true);
        }
    }
}
