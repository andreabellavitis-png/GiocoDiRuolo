namespace GiocoDiRuolo
{
    // Classe base per tutti gli oggetti che il giocatore può avere nell'inventario
    public class Oggetto
    {
        public string Nome { get; set; }
        public string Descrizione { get; set; }

        public Oggetto(string nome, string descrizione)
        {
            Nome = nome;
            Descrizione = descrizione;
        }

        // Breve descrizione delle caratteristiche dell'oggetto.
        // Le classi figlie la ridefiniscono con "override".
        public virtual string Info()
        {
            return Descrizione;
        }
    }

    // Un'arma: il danno dipende anche dalla statistica indicata
    // (es. la spada usa la Forza, il bastone l'Intelligenza)
    public class Arma : Oggetto
    {
        public int Danno { get; set; }
        public Statistica StatisticaUsata { get; set; }

        public Arma(string nome, string descrizione, int danno, Statistica statisticaUsata)
            : base(nome, descrizione)
        {
            Danno = danno;
            StatisticaUsata = statisticaUsata;
        }

        public override string Info()
        {
            return "arma, danno " + Danno + ", bonus da " + StatisticaUsata;
        }
    }

    // Un'armatura: riduce il danno ricevuto
    public class Armatura : Oggetto
    {
        public int Difesa { get; set; }

        public Armatura(string nome, string descrizione, int difesa)
            : base(nome, descrizione)
        {
            Difesa = difesa;
        }

        public override string Info()
        {
            return "armatura, difesa " + Difesa;
        }
    }

    // Una pozione: cura il giocatore
    public class Pozione : Oggetto
    {
        public int Cura { get; set; }

        public Pozione(string nome, string descrizione, int cura)
            : base(nome, descrizione)
        {
            Cura = cura;
        }

        public override string Info()
        {
            return "pozione, cura " + Cura + " PV";
        }
    }
}
