namespace GiocoDiRuolo
{
    // Una possibile risposta del giocatore all'interno di un dialogo
    public class SceltaDialogo
    {
        public string Testo { get; set; }

        // Id del nodo a cui si passa dopo questa scelta. "" = il dialogo finisce.
        public string ProssimoNodo { get; set; }

        // Parola chiave da salvare nella Memoria quando si sceglie questa risposta ("" = niente)
        public string DaRicordare { get; set; }

        // La scelta compare solo se nella Memoria c'è questa parola chiave ("" = nessun vincolo)
        public string RichiedeScelta { get; set; }

        // La scelta NON compare se nella Memoria c'è questa parola chiave ("" = nessun vincolo)
        public string EscludeScelta { get; set; }

        // La scelta compare solo per questa classe ("" = per tutte le classi)
        public string ClasseRichiesta { get; set; }

        // Azione speciale gestita dal codice del gioco, es. "DAI_POZIONE" ("" = nessuna)
        public string Azione { get; set; }

        public SceltaDialogo(string testo, string prossimoNodo)
        {
            Testo = testo;
            ProssimoNodo = prossimoNodo;
            DaRicordare = "";
            RichiedeScelta = "";
            EscludeScelta = "";
            ClasseRichiesta = "";
            Azione = "";
        }
    }

    // Una "battuta" del dialogo: chi parla, cosa dice e le risposte possibili
    public class NodoDialogo
    {
        public string Id { get; set; }
        public string ChiParla { get; set; }
        public string Testo { get; set; }
        // Al massimo 9 risposte, perché si scelgono con i tasti da 1 a 9
        public const int MAX_SCELTE = 9;

        // Solo le prime "NumeroScelte" celle dell'array sono usate
        public SceltaDialogo[] Scelte { get; private set; }
        public int NumeroScelte { get; private set; }

        public NodoDialogo(string id, string chiParla, string testo)
        {
            Id = id;
            ChiParla = chiParla;
            Testo = testo;
            Scelte = new SceltaDialogo[MAX_SCELTE];
            NumeroScelte = 0;
        }

        public void AggiungiScelta(SceltaDialogo scelta)
        {
            if (NumeroScelte < MAX_SCELTE)
            {
                Scelte[NumeroScelte] = scelta;
                NumeroScelte++;
            }
        }
    }

    // Un dialogo completo: un insieme di nodi collegati tra loro tramite le scelte
    public class Dialogo
    {
        private const int MAX_NODI = 50;

        // Solo le prime "numeroNodi" celle dell'array sono usate
        private NodoDialogo[] nodi;
        private int numeroNodi;

        public string NodoIniziale { get; set; }

        public Dialogo(string nodoIniziale)
        {
            NodoIniziale = nodoIniziale;
            nodi = new NodoDialogo[MAX_NODI];
            numeroNodi = 0;
        }

        // Aggiunge un nodo. Se esiste già un nodo con lo stesso Id, viene sostituito.
        public void AggiungiNodo(NodoDialogo nodo)
        {
            int posizione = Cerca(nodo.Id);
            if (posizione != -1)
            {
                nodi[posizione] = nodo;
            }
            else if (numeroNodi < MAX_NODI)
            {
                nodi[numeroNodi] = nodo;
                numeroNodi++;
            }
        }

        public NodoDialogo GetNodo(string id)
        {
            int posizione = Cerca(id);
            if (posizione != -1)
            {
                return nodi[posizione];
            }
            return null;
        }

        // Restituisce la posizione del nodo con questo Id, oppure -1 se non c'è
        private int Cerca(string id)
        {
            for (int i = 0; i < numeroNodi; i++)
            {
                if (nodi[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
