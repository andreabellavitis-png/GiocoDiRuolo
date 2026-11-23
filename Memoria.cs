namespace GiocoDiRuolo
{
    // Tiene traccia delle scelte fatte dal giocatore e degli eventi accaduti.
    // Ogni scelta è una semplice parola chiave, es. "aiutato_aldo" o "porta_sfondata".
    // Più avanti nel gioco si può controllare se una scelta è stata fatta
    // e cambiare di conseguenza dialoghi, eventi e finale.
    public class Memoria
    {
        private const int MAX_SCELTE = 100;

        // Array di dimensione fissa: solo le prime "numeroScelte" celle sono usate
        private string[] scelte;
        private int numeroScelte;

        public Memoria()
        {
            scelte = new string[MAX_SCELTE];
            numeroScelte = 0;
        }

        public void Ricorda(string scelta)
        {
            if (!HaScelto(scelta) && numeroScelte < MAX_SCELTE)
            {
                scelte[numeroScelte] = scelta;
                numeroScelte++;
            }
        }

        public void Dimentica(string scelta)
        {
            int posizione = Cerca(scelta);
            if (posizione == -1)
            {
                return;
            }
            // Spostiamo indietro di una cella tutti gli elementi successivi
            for (int i = posizione; i < numeroScelte - 1; i++)
            {
                scelte[i] = scelte[i + 1];
            }
            numeroScelte--;
            scelte[numeroScelte] = null;
        }

        public bool HaScelto(string scelta)
        {
            return Cerca(scelta) != -1;
        }

        // Restituisce un array che contiene solo le scelte effettivamente salvate
        public string[] TutteLeScelte()
        {
            string[] risultato = new string[numeroScelte];
            for (int i = 0; i < numeroScelte; i++)
            {
                risultato[i] = scelte[i];
            }
            return risultato;
        }

        // Restituisce la posizione della scelta nell'array, oppure -1 se non c'è
        private int Cerca(string scelta)
        {
            for (int i = 0; i < numeroScelte; i++)
            {
                if (scelte[i] == scelta)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
