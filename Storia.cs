namespace GiocoDiRuolo
{
    // Contiene i "contenuti" dell'avventura: mappa, nemici, dialoghi e testi.
    // Per creare una nuova storia basta modificare (o aggiungere) i metodi di questa classe,
    // senza toccare il "motore" del gioco (Gioco, Combattimento, Schermo...).
    public static class Storia
    {
        // Difficoltà della prova per aprire la porta (dado da 1 a 10 + statistica)
        public const int DIFFICOLTA_PORTA = 12;

        // Legenda: # muro, . pavimento, @ partenza, N Aldo, O orco, C forziere,
        //          + porta chiusa, G goblin, > uscita
        public static string[] CreaMappa()
        {
            string[] righe = new string[]
            {
                "################################",
                "#.......#..........#...........#",
                "#..@....#..........#.....G.....#",
                "#.......#..........#...........#",
                "#.N......O.........+.........>.#",
                "#.......#......C...#...........#",
                "################################"
            };
            return righe;
        }

        public static Nemico CreaOrco()
        {
            // nome, PV, attacco, difesa, precisione, schivata
            return new Nemico("Orco carceriere", 18, 5, 1, 70, 0);
        }

        public static Nemico CreaGoblin()
        {
            return new Nemico("Goblin guardiano", 22, 6, 2, 75, 10);
        }

        public static string[] TestoIntroduzione(ClassePersonaggio classe)
        {
            string fraseClasse;
            if (classe == ClassePersonaggio.Guerriero)
            {
                fraseClasse = "La tua mano cerca d'istinto l'elsa della spada: per fortuna i carcerieri non te l'hanno tolta.";
            }
            else if (classe == ClassePersonaggio.Mago)
            {
                fraseClasse = "Senti ancora l'energia magica scorrerti tra le dita. Non sono riusciti a spegnerla.";
            }
            else
            {
                fraseClasse = "Nello stivale ci sono ancora i tuoi grimaldelli. I carcerieri sono stati sbadati.";
            }

            string[] testo = new string[]
            {
                "Ti risvegli sul pavimento gelido delle Prigioni di Grimmorn. Non ricordi come ci sei finito.",
                fraseClasse,
                "In un angolo un vecchio incatenato ti osserva in silenzio. Da est arriva un pesante odore di orco.",
                "",
                "Comandi: W A S D per muoverti, INVIO per interagire con ciò che hai accanto, I per l'inventario."
            };
            return testo;
        }

        // Dialogo con Aldo, il vecchio prigioniero.
        // Le scelte vengono salvate nella Memoria e cambiano il finale.
        public static Dialogo CreaDialogoAldo()
        {
            Dialogo dialogo = new Dialogo("inizio");
            NodoDialogo nodo;
            SceltaDialogo scelta;

            // --- Nodo iniziale ---
            nodo = new NodoDialogo("inizio", "Aldo",
                "Ehi, tu! Finalmente qualcuno che non è un orco. Mi chiamo Aldo, sono prigioniero qui da mesi.");

            nodo.AggiungiScelta(new SceltaDialogo("Come si esce da questo posto?", "uscita"));

            scelta = new SceltaDialogo("Hai bisogno di aiuto?", "aiuto");
            scelta.EscludeScelta = "aiutato_aldo";
            nodo.AggiungiScelta(scelta);

            // Scelte diverse a seconda della classe del giocatore
            scelta = new SceltaDialogo("[Guerriero] Provo a spezzare le tue catene.", "catene_guerriero");
            scelta.ClasseRichiesta = "Guerriero";
            scelta.EscludeScelta = "sa_della_chiave";
            nodo.AggiungiScelta(scelta);

            scelta = new SceltaDialogo("[Mago] Esamino le catene con la magia.", "catene_mago");
            scelta.ClasseRichiesta = "Mago";
            scelta.EscludeScelta = "sa_della_chiave";
            nodo.AggiungiScelta(scelta);

            scelta = new SceltaDialogo("[Ladro] Do un'occhiata alla serratura delle catene.", "catene_ladro");
            scelta.ClasseRichiesta = "Ladro";
            scelta.EscludeScelta = "sa_della_chiave";
            nodo.AggiungiScelta(scelta);

            nodo.AggiungiScelta(new SceltaDialogo("Devo andare.", ""));
            dialogo.AggiungiNodo(nodo);

            // --- Informazioni sull'uscita ---
            nodo = new NodoDialogo("uscita", "Aldo",
                "Oltre questa cella c'è un orco di guardia, e dopo di lui una porta chiusa a chiave. "
                + "Dietro la porta dorme un goblin: se fai troppo rumore si sveglierà! L'uscita è subito dopo.");
            scelta = new SceltaDialogo("Grazie, starò attento.", "inizio");
            scelta.DaRicordare = "avvertito_del_goblin";
            nodo.AggiungiScelta(scelta);
            dialogo.AggiungiNodo(nodo);

            // --- Richiesta di aiuto ---
            nodo = new NodoDialogo("aiuto", "Aldo",
                "Ho una ferita che non vuole guarire... e queste maledette catene. Una pozione mi salverebbe la vita.");

            scelta = new SceltaDialogo("Prendi la mia pozione.", "grazie");
            scelta.Azione = "DAI_POZIONE";
            nodo.AggiungiScelta(scelta);

            scelta = new SceltaDialogo("Tornerò a liberarti, te lo prometto.", "promessa");
            scelta.EscludeScelta = "promessa_aldo";
            scelta.DaRicordare = "promessa_aldo";
            nodo.AggiungiScelta(scelta);

            scelta = new SceltaDialogo("Mi dispiace, devo pensare a me stesso.", "rifiuto");
            scelta.DaRicordare = "rifiutato_aldo";
            nodo.AggiungiScelta(scelta);
            dialogo.AggiungiNodo(nodo);

            // --- Risposte di Aldo ---
            nodo = new NodoDialogo("grazie", "Aldo",
                "Grazie, straniero! Mi sento già meglio. In cambio ti ho insegnato un vecchio trucco del mestiere.");
            nodo.AggiungiScelta(new SceltaDialogo("Di nulla.", "inizio"));
            dialogo.AggiungiNodo(nodo);

            nodo = new NodoDialogo("senza_pozione", "Aldo",
                "Non hai pozioni con te... Forse nel forziere nella stanza dell'orco ne troverai qualcuna.");
            nodo.AggiungiScelta(new SceltaDialogo("Va bene, tornerò.", "inizio"));
            dialogo.AggiungiNodo(nodo);

            nodo = new NodoDialogo("promessa", "Aldo",
                "Conto su di te. La chiave delle catene ce l'ha il goblin, dietro la porta.");
            scelta = new SceltaDialogo("La recupererò.", "inizio");
            scelta.DaRicordare = "sa_della_chiave";
            nodo.AggiungiScelta(scelta);
            dialogo.AggiungiNodo(nodo);

            nodo = new NodoDialogo("rifiuto", "Aldo",
                "Capisco... Che gli dei ti accompagnino, allora.");
            nodo.AggiungiScelta(new SceltaDialogo("Addio.", ""));
            dialogo.AggiungiNodo(nodo);

            // --- Esame delle catene (uno per classe) ---
            nodo = new NodoDialogo("catene_guerriero", "",
                "Tiri con tutta la tua forza, ma le catene non si piegano nemmeno. Aldo sospira: \"Serve la chiave, ce l'ha il goblin.\"");
            scelta = new SceltaDialogo("Capito.", "inizio");
            scelta.DaRicordare = "sa_della_chiave";
            nodo.AggiungiScelta(scelta);
            dialogo.AggiungiNodo(nodo);

            nodo = new NodoDialogo("catene_mago", "",
                "Le catene sono protette da un incantesimo: solo la loro chiave può aprirle. Aldo annuisce: \"Ce l'ha il goblin.\"");
            scelta = new SceltaDialogo("Capito.", "inizio");
            scelta.DaRicordare = "sa_della_chiave";
            nodo.AggiungiScelta(scelta);
            dialogo.AggiungiNodo(nodo);

            nodo = new NodoDialogo("catene_ladro", "",
                "La serratura è troppo arrugginita per i tuoi grimaldelli. Aldo sussurra: \"La chiave ce l'ha il goblin.\"");
            scelta = new SceltaDialogo("Capito.", "inizio");
            scelta.DaRicordare = "sa_della_chiave";
            nodo.AggiungiScelta(scelta);
            dialogo.AggiungiNodo(nodo);

            return dialogo;
        }

        // Dialogo all'uscita della prigione: le scelte disponibili dipendono dalla Memoria
        public static Dialogo CreaDialogoFinale()
        {
            Dialogo dialogo = new Dialogo("uscita");
            NodoDialogo nodo;
            SceltaDialogo scelta;

            nodo = new NodoDialogo("uscita", "",
                "Davanti a te una scala sale verso la superficie. Senti l'aria fresca della notte.");

            scelta = new SceltaDialogo("Torno indietro a liberare Aldo con la chiave.", "liberato");
            scelta.RichiedeScelta = "chiave_presa";
            scelta.DaRicordare = "aldo_liberato";
            nodo.AggiungiScelta(scelta);

            scelta = new SceltaDialogo("Salgo la scala ed esco.", "fine");
            nodo.AggiungiScelta(scelta);

            nodo.AggiungiScelta(new SceltaDialogo("Non sono ancora pronto, torno indietro.", ""));
            dialogo.AggiungiNodo(nodo);

            nodo = new NodoDialogo("liberato", "Aldo",
                "Sei tornato davvero! Le catene cadono a terra con un tonfo. Andiamocene da qui, amico mio.");
            scelta = new SceltaDialogo("Usciamo insieme.", "");
            scelta.DaRicordare = "gioco_finito";
            nodo.AggiungiScelta(scelta);
            dialogo.AggiungiNodo(nodo);

            nodo = new NodoDialogo("fine", "",
                "Sali gli ultimi gradini e ti lasci la prigione alle spalle.");
            scelta = new SceltaDialogo("Continua.", "");
            scelta.DaRicordare = "gioco_finito";
            nodo.AggiungiScelta(scelta);
            dialogo.AggiungiNodo(nodo);

            return dialogo;
        }
    }
}
