namespace GiocoDiRuolo
{
    // La griglia su cui si muove il giocatore.
    // Ogni cella è un carattere:
    //   '#' muro     '.' pavimento    '+' porta chiusa
    //   'N' personaggio con cui parlare   'C' forziere
    //   'O' orco     'G' goblin       '>' uscita
    public class Mappa
    {
        private char[,] celle;

        public int Larghezza { get; set; }
        public int Altezza { get; set; }

        // Crea la mappa a partire da un array di stringhe (una stringa per riga)
        public Mappa(string[] righe)
        {
            Altezza = righe.Length;
            Larghezza = 0;
            for (int i = 0; i < righe.Length; i++)
            {
                if (righe[i].Length > Larghezza)
                {
                    Larghezza = righe[i].Length;
                }
            }

            celle = new char[Larghezza, Altezza];
            for (int y = 0; y < Altezza; y++)
            {
                for (int x = 0; x < Larghezza; x++)
                {
                    if (x < righe[y].Length)
                    {
                        celle[x, y] = righe[y][x];
                    }
                    else
                    {
                        celle[x, y] = '#';
                    }
                }
            }
        }

        public char GetCella(int x, int y)
        {
            // Fuori dalla mappa consideriamo che ci sia un muro
            if (x < 0 || y < 0 || x >= Larghezza || y >= Altezza)
            {
                return '#';
            }
            return celle[x, y];
        }

        public void SetCella(int x, int y, char simbolo)
        {
            if (x >= 0 && y >= 0 && x < Larghezza && y < Altezza)
            {
                celle[x, y] = simbolo;
            }
        }

        // Il giocatore può camminare solo sul pavimento
        public bool ECalpestabile(int x, int y)
        {
            return GetCella(x, y) == '.';
        }

        // Cerca la prima cella con il simbolo indicato.
        // Restituisce true se lo trova e mette le coordinate in x e y.
        public bool TrovaSimbolo(char simbolo, out int x, out int y)
        {
            for (int cy = 0; cy < Altezza; cy++)
            {
                for (int cx = 0; cx < Larghezza; cx++)
                {
                    if (celle[cx, cy] == simbolo)
                    {
                        x = cx;
                        y = cy;
                        return true;
                    }
                }
            }
            x = -1;
            y = -1;
            return false;
        }
    }
}
