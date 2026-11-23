namespace GiocoDiRuolo
{
    // Classe base per tutti i personaggi del gioco (giocatore e nemici).
    // Contiene le cose che hanno in comune: nome e punti vita.
    public class Personaggio
    {
        public string Nome { get; set; }
        public int PuntiVita { get; set; }
        public int PuntiVitaMax { get; set; }

        public Personaggio(string nome, int puntiVitaMax)
        {
            Nome = nome;
            PuntiVitaMax = puntiVitaMax;
            PuntiVita = puntiVitaMax;
        }

        public bool EVivo()
        {
            return PuntiVita > 0;
        }

        public void SubisciDanno(int danno)
        {
            PuntiVita = PuntiVita - danno;
            if (PuntiVita < 0)
            {
                PuntiVita = 0;
            }
        }

        // Restituisce quanti punti vita sono stati effettivamente recuperati
        public int Cura(int quantita)
        {
            int prima = PuntiVita;
            PuntiVita = PuntiVita + quantita;
            if (PuntiVita > PuntiVitaMax)
            {
                PuntiVita = PuntiVitaMax;
            }
            return PuntiVita - prima;
        }
    }
}
