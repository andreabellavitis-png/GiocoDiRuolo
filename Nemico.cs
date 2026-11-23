namespace GiocoDiRuolo
{
    // Un nemico che il giocatore può combattere
    public class Nemico : Personaggio
    {
        public int Attacco { get; set; }     // danno base dei suoi colpi
        public int Difesa { get; set; }      // riduce il danno ricevuto
        public int Precisione { get; set; }  // probabilità (in %) di colpire
        public int Schivata { get; set; }    // riduce la probabilità del giocatore di colpirlo

        public Nemico(string nome, int puntiVita, int attacco, int difesa, int precisione, int schivata)
            : base(nome, puntiVita)
        {
            Attacco = attacco;
            Difesa = difesa;
            Precisione = precisione;
            Schivata = schivata;
        }
    }
}
