namespace GiocoDiRuolo
{
    // Il personaggio controllato dal giocatore
    public class Giocatore : Personaggio
    {
        public ClassePersonaggio Classe { get; set; }

        // Statistiche
        public int Forza { get; set; }
        public int Intelligenza { get; set; }
        public int Destrezza { get; set; }

        // Posizione sulla mappa
        public int X { get; set; }
        public int Y { get; set; }

        // Inventario ed equipaggiamento
        public const int MAX_OGGETTI = 20;

        // Solo le prime "NumeroOggetti" celle dell'array sono usate
        public Oggetto[] Inventario { get; private set; }
        public int NumeroOggetti { get; private set; }
        public Arma ArmaEquipaggiata { get; set; }
        public Armatura ArmaturaEquipaggiata { get; set; }

        public Giocatore(string nome, ClassePersonaggio classe)
            : base(nome, 20)
        {
            Classe = classe;
            Inventario = new Oggetto[MAX_OGGETTI];
            NumeroOggetti = 0;

            Arma armaIniziale;
            Armatura armaturaIniziale;

            // In base alla classe scelta cambiano statistiche ed equipaggiamento
            if (classe == ClassePersonaggio.Guerriero)
            {
                Forza = 8;
                Intelligenza = 2;
                Destrezza = 4;
                PuntiVitaMax = 30;
                armaIniziale = new Arma("Spada", "Una spada robusta e affidabile.", 5, Statistica.Forza);
                armaturaIniziale = new Armatura("Cotta di maglia", "Pesante, ma protegge bene.", 3);
            }
            else if (classe == ClassePersonaggio.Mago)
            {
                Forza = 2;
                Intelligenza = 8;
                Destrezza = 4;
                PuntiVitaMax = 20;
                armaIniziale = new Arma("Bastone runico", "Un bastone che amplifica la magia.", 3, Statistica.Intelligenza);
                armaturaIniziale = new Armatura("Tunica", "Una tunica leggera da studioso.", 1);
            }
            else
            {
                Forza = 4;
                Intelligenza = 3;
                Destrezza = 8;
                PuntiVitaMax = 24;
                armaIniziale = new Arma("Pugnale", "Piccolo, veloce e silenzioso.", 4, Statistica.Destrezza);
                armaturaIniziale = new Armatura("Armatura di cuoio", "Leggera, non intralcia i movimenti.", 2);
            }

            PuntiVita = PuntiVitaMax;

            AggiungiOggetto(armaIniziale);
            AggiungiOggetto(armaturaIniziale);
            AggiungiOggetto(new Pozione("Pozione di cura", "Un liquido rosso dal sapore amaro.", 10));

            ArmaEquipaggiata = armaIniziale;
            ArmaturaEquipaggiata = armaturaIniziale;
        }

        // Restituisce il valore di una statistica
        public int GetStatistica(Statistica statistica)
        {
            if (statistica == Statistica.Forza)
            {
                return Forza;
            }
            else if (statistica == Statistica.Intelligenza)
            {
                return Intelligenza;
            }
            else
            {
                return Destrezza;
            }
        }

        // La statistica più importante per ogni classe
        public Statistica StatisticaPrincipale()
        {
            if (Classe == ClassePersonaggio.Guerriero)
            {
                return Statistica.Forza;
            }
            else if (Classe == ClassePersonaggio.Mago)
            {
                return Statistica.Intelligenza;
            }
            else
            {
                return Statistica.Destrezza;
            }
        }

        public void AumentaStatistica(Statistica statistica, int quantita)
        {
            if (statistica == Statistica.Forza)
            {
                Forza = Forza + quantita;
            }
            else if (statistica == Statistica.Intelligenza)
            {
                Intelligenza = Intelligenza + quantita;
            }
            else
            {
                Destrezza = Destrezza + quantita;
            }
        }

        // Danno base: danno dell'arma + metà della statistica usata dall'arma
        public int CalcolaDanno()
        {
            if (ArmaEquipaggiata == null)
            {
                return 1 + Forza / 2; // a mani nude
            }
            int bonus = GetStatistica(ArmaEquipaggiata.StatisticaUsata) / 2;
            return ArmaEquipaggiata.Danno + bonus;
        }

        public int CalcolaDifesa()
        {
            if (ArmaturaEquipaggiata == null)
            {
                return 0;
            }
            return ArmaturaEquipaggiata.Difesa;
        }

        // Nome dell'abilità speciale della classe
        public string NomeAbilita()
        {
            if (Classe == ClassePersonaggio.Guerriero)
            {
                return "Colpo possente";
            }
            else if (Classe == ClassePersonaggio.Mago)
            {
                return "Palla di fuoco";
            }
            else
            {
                return "Colpo furtivo";
            }
        }

        // Cerca la prima pozione nell'inventario. Restituisce null se non ce ne sono.
        public Pozione TrovaPozione()
        {
            for (int i = 0; i < NumeroOggetti; i++)
            {
                Oggetto oggetto = Inventario[i];
                if (oggetto is Pozione)
                {
                    return (Pozione)oggetto;
                }
            }
            return null;
        }

        public int ContaPozioni()
        {
            int conta = 0;
            for (int i = 0; i < NumeroOggetti; i++)
            {
                Oggetto oggetto = Inventario[i];
                if (oggetto is Pozione)
                {
                    conta++;
                }
            }
            return conta;
        }

        // Beve la pozione, la toglie dall'inventario e restituisce i PV recuperati
        public int BeviPozione(Pozione pozione)
        {
            RimuoviOggetto(pozione);
            return Cura(pozione.Cura);
        }

        // Aggiunge un oggetto in fondo all'inventario. Restituisce false se è pieno.
        public bool AggiungiOggetto(Oggetto oggetto)
        {
            if (NumeroOggetti >= MAX_OGGETTI)
            {
                return false;
            }
            Inventario[NumeroOggetti] = oggetto;
            NumeroOggetti++;
            return true;
        }

        // Toglie un oggetto dall'inventario, spostando indietro quelli successivi
        public void RimuoviOggetto(Oggetto oggetto)
        {
            for (int i = 0; i < NumeroOggetti; i++)
            {
                if (Inventario[i] == oggetto)
                {
                    for (int j = i; j < NumeroOggetti - 1; j++)
                    {
                        Inventario[j] = Inventario[j + 1];
                    }
                    NumeroOggetti--;
                    Inventario[NumeroOggetti] = null;
                    return;
                }
            }
        }

        public bool HaOggetto(string nome)
        {
            for (int i = 0; i < NumeroOggetti; i++)
            {
                Oggetto oggetto = Inventario[i];
                if (oggetto.Nome == nome)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
