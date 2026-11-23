using System;

namespace GiocoDiRuolo
{
    // Il "motore" del gioco: gestisce il ciclo principale, i movimenti,
    // le interazioni con la mappa, i dialoghi e gli eventi.
    public class Gioco
    {
        private Giocatore giocatore;
        private Mappa mappa;
        private Memoria memoria;
        private Schermo schermo;
        private Random casuale;

        private Nemico orco;
        private Nemico goblin;
        private Dialogo dialogoAldo;

        private bool inEsecuzione;

        public Gioco()
        {
            memoria = new Memoria();
            schermo = new Schermo();
            casuale = new Random();
        }

        public void Avvia()
        {
            CreaPersonaggio();
            PreparaMondo();
            MostraIntroduzione();
            CicloPrincipale();
        }

        // ------------------------------------------------------------
        // Preparazione
        // ------------------------------------------------------------

        private void CreaPersonaggio()
        {
            Console.Clear();
            Console.WriteLine("=== LE PRIGIONI DI GRIMMORN ===");
            Console.WriteLine();
            Console.Write("Come ti chiami, avventuriero? ");
            string nome = Console.ReadLine();
            if (nome == null || nome.Trim() == "")
            {
                nome = "Avventuriero";
            }
            nome = nome.Trim();

            schermo.AggiungiMessaggio("Benvenuto, " + nome + ". Scegli la tua classe:");
            schermo.AggiungiMessaggio("");
            schermo.AggiungiMessaggio("GUERRIERO: molto forte e resistente. Risolve i problemi con la forza bruta.");
            schermo.AggiungiMessaggio("MAGO: fragile ma dotato di potenti incantesimi. Usa l'intelligenza.");
            schermo.AggiungiMessaggio("LADRO: agile e preciso, colpisce spesso e di sorpresa. Usa la destrezza.");

            string[] opzioni =
            {
                "Guerriero  (FOR 8  INT 2  DES 4  PV 30)",
                "Mago       (FOR 2  INT 8  DES 4  PV 20)",
                "Ladro      (FOR 4  INT 3  DES 8  PV 24)"
            };
            int scelta = schermo.ChiediScelta(opzioni);

            ClassePersonaggio classe;
            if (scelta == 0)
            {
                classe = ClassePersonaggio.Guerriero;
            }
            else if (scelta == 1)
            {
                classe = ClassePersonaggio.Mago;
            }
            else
            {
                classe = ClassePersonaggio.Ladro;
            }

            giocatore = new Giocatore(nome, classe);
            schermo.Giocatore = giocatore;
            memoria.Ricorda("classe_" + classe.ToString().ToLower());
        }

        private void PreparaMondo()
        {
            mappa = new Mappa(Storia.CreaMappa());

            // Il simbolo '@' indica la posizione di partenza: lo sostituiamo con pavimento
            int x;
            int y;
            if (mappa.TrovaSimbolo('@', out x, out y))
            {
                mappa.SetCella(x, y, '.');
                giocatore.X = x;
                giocatore.Y = y;
            }

            orco = Storia.CreaOrco();
            goblin = Storia.CreaGoblin();
            dialogoAldo = Storia.CreaDialogoAldo();

            schermo.Mappa = mappa;
        }

        private void MostraIntroduzione()
        {
            schermo.PulisciMessaggi();
            string[] testo = Storia.TestoIntroduzione(giocatore.Classe);
            for (int i = 0; i < testo.Length; i++)
            {
                schermo.AggiungiMessaggio(testo[i]);
            }
            schermo.AttendiTasto();
        }

        // ------------------------------------------------------------
        // Ciclo principale
        // ------------------------------------------------------------

        private void CicloPrincipale()
        {
            inEsecuzione = true;
            while (inEsecuzione)
            {
                schermo.Disegna(null);
                ConsoleKeyInfo tasto = Console.ReadKey(true);

                if (tasto.Key == ConsoleKey.W)
                {
                    Muovi(0, -1);
                }
                else if (tasto.Key == ConsoleKey.S)
                {
                    Muovi(0, 1);
                }
                else if (tasto.Key == ConsoleKey.A)
                {
                    Muovi(-1, 0);
                }
                else if (tasto.Key == ConsoleKey.D)
                {
                    Muovi(1, 0);
                }
                else if (tasto.Key == ConsoleKey.Enter)
                {
                    Interagisci();
                }
                else if (tasto.Key == ConsoleKey.I)
                {
                    ApriInventario();
                }
                else if (tasto.Key == ConsoleKey.Q)
                {
                    ChiediUscita();
                }

                if (!giocatore.EVivo())
                {
                    GameOver();
                    inEsecuzione = false;
                }
            }
        }

        private void Muovi(int dx, int dy)
        {
            int nuovaX = giocatore.X + dx;
            int nuovaY = giocatore.Y + dy;

            if (mappa.ECalpestabile(nuovaX, nuovaY))
            {
                giocatore.X = nuovaX;
                giocatore.Y = nuovaY;
                ControllaEventi();
            }
            else
            {
                char simbolo = mappa.GetCella(nuovaX, nuovaY);
                if (simbolo != '#')
                {
                    schermo.AggiungiMessaggio("Davanti a te: " + DescriviSimbolo(simbolo) + ". Premi INVIO per interagire.");
                }
            }
        }

        private string DescriviSimbolo(char simbolo)
        {
            switch (simbolo)
            {
                case 'N': return "Aldo, il vecchio prigioniero";
                case 'O': return "l'orco carceriere";
                case 'G': return "il goblin";
                case 'C': return "un forziere";
                case '+': return "una porta chiusa";
                case '>': return "la scala per l'uscita";
                default: return "qualcosa";
            }
        }

        // Controlla se il giocatore si trova accanto a un certo simbolo (nelle 4 direzioni)
        private bool VicinoA(char simbolo)
        {
            return mappa.GetCella(giocatore.X, giocatore.Y - 1) == simbolo
                || mappa.GetCella(giocatore.X, giocatore.Y + 1) == simbolo
                || mappa.GetCella(giocatore.X - 1, giocatore.Y) == simbolo
                || mappa.GetCella(giocatore.X + 1, giocatore.Y) == simbolo;
        }

        // Eventi che succedono automaticamente quando il giocatore si muove
        private void ControllaEventi()
        {
            if (VicinoA('O') && !memoria.HaScelto("avviso_orco"))
            {
                memoria.Ricorda("avviso_orco");
                schermo.AggiungiMessaggio("Un enorme orco ti sbarra la strada brandendo una clava! Premi INVIO per combattere.");
            }

            if (VicinoA('C') && !memoria.HaScelto("avviso_forziere"))
            {
                memoria.Ricorda("avviso_forziere");
                schermo.AggiungiMessaggio("Vedi un vecchio forziere di legno. Premi INVIO per aprirlo.");
            }

            if (VicinoA('+') && !memoria.HaScelto("avviso_porta"))
            {
                memoria.Ricorda("avviso_porta");
                schermo.AggiungiMessaggio("Una pesante porta di legno, chiusa a chiave. Premi INVIO per provare ad aprirla.");
            }

            // Primo ingresso nella stanza del goblin (a destra della porta)
            if (giocatore.X >= 20 && !memoria.HaScelto("entrato_stanza_goblin"))
            {
                memoria.Ricorda("entrato_stanza_goblin");
                if (goblin.EVivo())
                {
                    if (memoria.HaScelto("goblin_sveglio"))
                    {
                        schermo.AggiungiMessaggio("Il goblin, svegliato dal frastuono, ti aspettava. Ti carica urlando!");
                        schermo.AttendiTasto();
                        int gx;
                        int gy;
                        mappa.TrovaSimbolo('G', out gx, out gy);
                        CombattiGoblin(gx, gy);
                    }
                    else
                    {
                        schermo.AggiungiMessaggio("Un goblin dorme russando. Alla sua cintura pende una chiave. "
                            + "Puoi attaccarlo di sorpresa (INVIO accanto a lui) oppure sgattaiolare fino all'uscita.");
                    }
                }
            }

            if (VicinoA('>') && !memoria.HaScelto("avviso_uscita"))
            {
                memoria.Ricorda("avviso_uscita");
                schermo.AggiungiMessaggio("Ecco la scala che porta fuori! Premi INVIO per salire.");
            }
        }

        // ------------------------------------------------------------
        // Interazioni
        // ------------------------------------------------------------

        private void Interagisci()
        {
            // Controlliamo le 4 celle intorno al giocatore: su, giù, sinistra, destra
            int[] spostamentoX = { 0, 0, -1, 1 };
            int[] spostamentoY = { -1, 1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                int x = giocatore.X + spostamentoX[i];
                int y = giocatore.Y + spostamentoY[i];
                char simbolo = mappa.GetCella(x, y);

                if (simbolo == 'N')
                {
                    AvviaDialogo(dialogoAldo);
                    return;
                }
                else if (simbolo == 'O')
                {
                    CombattiOrco(x, y);
                    return;
                }
                else if (simbolo == 'G')
                {
                    CombattiGoblin(x, y);
                    return;
                }
                else if (simbolo == 'C')
                {
                    ApriForziere(x, y);
                    return;
                }
                else if (simbolo == '+')
                {
                    ApriPorta(x, y);
                    return;
                }
                else if (simbolo == '>')
                {
                    Finale();
                    return;
                }
            }

            schermo.AggiungiMessaggio("Non c'è niente con cui interagire qui vicino.");
        }

        private void CombattiOrco(int x, int y)
        {
            Combattimento combattimento = new Combattimento(giocatore, orco, schermo, casuale);
            RisultatoCombattimento risultato = combattimento.Combatti(true, false);

            if (risultato == RisultatoCombattimento.Vittoria)
            {
                mappa.SetCella(x, y, '.');
                memoria.Ricorda("orco_sconfitto");
                schermo.PulisciMessaggi();
                schermo.AggiungiMessaggio("L'orco crolla a terra con un tonfo. Tutorial completato!");
                schermo.AggiungiMessaggio("La strada verso est ora è libera.");
            }
        }

        private void CombattiGoblin(int x, int y)
        {
            // Se il goblin dorme, il giocatore lo attacca di sorpresa
            bool sorpresa = !memoria.HaScelto("goblin_sveglio");
            memoria.Ricorda("goblin_sveglio");

            Combattimento combattimento = new Combattimento(giocatore, goblin, schermo, casuale);
            RisultatoCombattimento risultato = combattimento.Combatti(false, sorpresa);

            if (risultato == RisultatoCombattimento.Vittoria)
            {
                mappa.SetCella(x, y, '.');
                memoria.Ricorda("goblin_sconfitto");
                memoria.Ricorda("chiave_presa");
                giocatore.AggiungiOggetto(new Oggetto("Chiave delle catene", "Una chiave di ferro con strane rune incise."));
                giocatore.AggiungiOggetto(new Pozione("Pozione di cura", "Un liquido rosso dal sapore amaro.", 10));
                schermo.PulisciMessaggi();
                schermo.AggiungiMessaggio("Frughi il corpo del goblin: trovi la Chiave delle catene e una Pozione di cura.");
                if (memoria.HaScelto("promessa_aldo"))
                {
                    schermo.AggiungiMessaggio("Ti ricordi della promessa fatta ad Aldo...");
                }
            }
            else if (risultato == RisultatoCombattimento.Fuga)
            {
                schermo.AggiungiMessaggio("Il goblin ti tiene d'occhio, pronto a colpire di nuovo.");
            }
        }

        private void ApriForziere(int x, int y)
        {
            mappa.SetCella(x, y, '.');
            memoria.Ricorda("forziere_aperto");
            giocatore.AggiungiOggetto(new Pozione("Pozione di cura", "Un liquido rosso dal sapore amaro.", 10));
            giocatore.AggiungiOggetto(new Arma("Ascia arrugginita", "Pesante e brutale. Serve molta forza.", 7, Statistica.Forza));
            schermo.AggiungiMessaggio("Apri il forziere: dentro ci sono una Pozione di cura e un'Ascia arrugginita!");
            schermo.AggiungiMessaggio("(Premi I per aprire l'inventario ed equipaggiare gli oggetti.)");
        }

        // La porta si può aprire in tre modi, ognuno legato a una statistica.
        // Ogni classe è più brava in un metodo, ma può tentare anche gli altri.
        private void ApriPorta(int x, int y)
        {
            schermo.AggiungiMessaggio("La porta è chiusa a chiave. Come vuoi aprirla?");
            if (memoria.HaScelto("avvertito_del_goblin"))
            {
                schermo.AggiungiMessaggio("(Ricordi le parole di Aldo: dietro la porta dorme un goblin, meglio non fare rumore...)");
            }

            string[] opzioni =
            {
                "Sfondala con la forza bruta    (Forza " + giocatore.Forza + ")" + Consigliato(ClassePersonaggio.Guerriero),
                "Scassina la serratura          (Destrezza " + giocatore.Destrezza + ")" + Consigliato(ClassePersonaggio.Ladro),
                "Lancia un incantesimo di apertura (Intelligenza " + giocatore.Intelligenza + ")" + Consigliato(ClassePersonaggio.Mago),
                "Lascia stare per ora"
            };
            int scelta = schermo.ChiediScelta(opzioni);

            if (scelta == 3)
            {
                schermo.AggiungiMessaggio("Ti allontani dalla porta.");
                return;
            }

            // Prova: dado da 1 a 10 + statistica, deve raggiungere la difficoltà
            int statistica;
            if (scelta == 0)
            {
                statistica = giocatore.Forza;
            }
            else if (scelta == 1)
            {
                statistica = giocatore.Destrezza;
            }
            else
            {
                statistica = giocatore.Intelligenza;
            }

            int dado = casuale.Next(1, 11);
            int totale = dado + statistica;
            bool riuscito = totale >= Storia.DIFFICOLTA_PORTA;
            schermo.AggiungiMessaggio("Prova: dado " + dado + " + statistica " + statistica + " = " + totale
                + " (serve almeno " + Storia.DIFFICOLTA_PORTA + ")");

            if (scelta == 0)
            {
                // La forza bruta fa sempre rumore: il goblin si sveglia!
                memoria.Ricorda("goblin_sveglio");
                if (riuscito)
                {
                    memoria.Ricorda("porta_sfondata");
                    schermo.AggiungiMessaggio("Con una spallata possente sfondi la porta! Il frastuono rimbomba in tutta la prigione...");
                }
                else
                {
                    giocatore.SubisciDanno(2);
                    schermo.AggiungiMessaggio("La porta non cede e ti fai male alla spalla (-2 PV). Il rumore echeggia nei corridoi...");
                }
            }
            else if (scelta == 1)
            {
                if (riuscito)
                {
                    memoria.Ricorda("porta_scassinata");
                    schermo.AggiungiMessaggio("Click! La serratura si apre senza il minimo rumore.");
                }
                else
                {
                    schermo.AggiungiMessaggio("Il grimaldello scivola via. Puoi riprovare.");
                }
            }
            else
            {
                if (riuscito)
                {
                    memoria.Ricorda("porta_magia");
                    schermo.AggiungiMessaggio("Pronunci la formula: la serratura si illumina di blu e scatta silenziosamente.");
                }
                else
                {
                    giocatore.SubisciDanno(2);
                    schermo.AggiungiMessaggio("L'incantesimo ti si ritorce contro con una scossa (-2 PV).");
                }
            }

            if (riuscito)
            {
                mappa.SetCella(x, y, '.');
            }
        }

        private string Consigliato(ClassePersonaggio classe)
        {
            if (giocatore.Classe == classe)
            {
                return "  <- la tua specialità";
            }
            return "";
        }

        // ------------------------------------------------------------
        // Dialoghi
        // ------------------------------------------------------------

        private void AvviaDialogo(Dialogo dialogo)
        {
            string idCorrente = dialogo.NodoIniziale;

            while (idCorrente != "")
            {
                NodoDialogo nodo = dialogo.GetNodo(idCorrente);
                if (nodo == null)
                {
                    break; // errore nella scrittura del dialogo: usciamo
                }

                schermo.AggiungiMessaggio("");
                if (nodo.ChiParla == "")
                {
                    schermo.AggiungiMessaggio(nodo.Testo);
                }
                else
                {
                    schermo.AggiungiMessaggio(nodo.ChiParla + ": \"" + nodo.Testo + "\"");
                }

                // Mostriamo solo le scelte permesse in questo momento
                // Primo passaggio: contiamo le scelte disponibili per sapere quanto fare grandi gli array
                int quanteDisponibili = 0;
                for (int i = 0; i < nodo.NumeroScelte; i++)
                {
                    if (SceltaDisponibile(nodo.Scelte[i]))
                    {
                        quanteDisponibili++;
                    }
                }

                if (quanteDisponibili == 0)
                {
                    schermo.AttendiTasto();
                    break;
                }

                // Secondo passaggio: riempiamo gli array
                SceltaDialogo[] disponibili = new SceltaDialogo[quanteDisponibili];
                string[] testi = new string[quanteDisponibili];
                int posizione = 0;
                for (int i = 0; i < nodo.NumeroScelte; i++)
                {
                    SceltaDialogo scelta = nodo.Scelte[i];
                    if (SceltaDisponibile(scelta))
                    {
                        disponibili[posizione] = scelta;
                        testi[posizione] = scelta.Testo;
                        posizione++;
                    }
                }

                int indice = schermo.ChiediScelta(testi);
                SceltaDialogo sceltaFatta = disponibili[indice];
                schermo.AggiungiMessaggio("> " + sceltaFatta.Testo);

                if (sceltaFatta.DaRicordare != "")
                {
                    memoria.Ricorda(sceltaFatta.DaRicordare);
                }

                idCorrente = sceltaFatta.ProssimoNodo;
                if (sceltaFatta.Azione != "")
                {
                    idCorrente = EseguiAzione(sceltaFatta.Azione, idCorrente);
                }
            }
        }

        private bool SceltaDisponibile(SceltaDialogo scelta)
        {
            if (scelta.ClasseRichiesta != "" && scelta.ClasseRichiesta != giocatore.Classe.ToString())
            {
                return false;
            }
            if (scelta.RichiedeScelta != "" && !memoria.HaScelto(scelta.RichiedeScelta))
            {
                return false;
            }
            if (scelta.EscludeScelta != "" && memoria.HaScelto(scelta.EscludeScelta))
            {
                return false;
            }
            return true;
        }

        // Azioni speciali dei dialoghi. Restituisce il nodo a cui andare
        // (può essere diverso da quello previsto, ad esempio se l'azione fallisce).
        private string EseguiAzione(string azione, string prossimoNodo)
        {
            if (azione == "DAI_POZIONE")
            {
                Pozione pozione = giocatore.TrovaPozione();
                if (pozione == null)
                {
                    return "senza_pozione";
                }
                giocatore.RimuoviOggetto(pozione);
                memoria.Ricorda("aiutato_aldo");

                Statistica statistica = giocatore.StatisticaPrincipale();
                giocatore.AumentaStatistica(statistica, 1);
                schermo.AggiungiMessaggio("Aldo ti insegna un trucco: la tua " + statistica + " aumenta di 1!");
            }
            return prossimoNodo;
        }

        // ------------------------------------------------------------
        // Inventario
        // ------------------------------------------------------------

        private void ApriInventario()
        {
            schermo.AggiungiMessaggio("");
            schermo.AggiungiMessaggio("INVENTARIO - scegli un oggetto da usare o equipaggiare.");

            bool aperto = true;
            while (aperto)
            {
                // Al massimo 8 oggetti + "Chiudi", perché si sceglie con i tasti da 1 a 9
                int quanti = giocatore.NumeroOggetti;
                if (quanti > 8)
                {
                    quanti = 8;
                }
                string[] opzioni = new string[quanti + 1];
                for (int i = 0; i < quanti; i++)
                {
                    Oggetto oggetto = giocatore.Inventario[i];
                    string riga = oggetto.Nome + " (" + oggetto.Info() + ")";
                    if (oggetto == giocatore.ArmaEquipaggiata || oggetto == giocatore.ArmaturaEquipaggiata)
                    {
                        riga = riga + " [equipaggiato]";
                    }
                    opzioni[i] = riga;
                }
                opzioni[quanti] = "Chiudi inventario";

                int scelta = schermo.ChiediScelta(opzioni);
                if (scelta == opzioni.Length - 1)
                {
                    aperto = false;
                }
                else
                {
                    UsaOggetto(giocatore.Inventario[scelta]);
                }
            }
        }

        private void UsaOggetto(Oggetto oggetto)
        {
            if (oggetto is Arma)
            {
                giocatore.ArmaEquipaggiata = (Arma)oggetto;
                schermo.AggiungiMessaggio("Impugni: " + oggetto.Nome + ". Danno attuale: " + giocatore.CalcolaDanno());
            }
            else if (oggetto is Armatura)
            {
                giocatore.ArmaturaEquipaggiata = (Armatura)oggetto;
                schermo.AggiungiMessaggio("Indossi: " + oggetto.Nome + ".");
            }
            else if (oggetto is Pozione)
            {
                if (giocatore.PuntiVita == giocatore.PuntiVitaMax)
                {
                    schermo.AggiungiMessaggio("Sei già in perfetta salute.");
                }
                else
                {
                    int curati = giocatore.BeviPozione((Pozione)oggetto);
                    schermo.AggiungiMessaggio("Bevi la pozione e recuperi " + curati + " PV.");
                }
            }
            else
            {
                schermo.AggiungiMessaggio(oggetto.Nome + ": " + oggetto.Descrizione);
            }
        }

        // ------------------------------------------------------------
        // Fine del gioco
        // ------------------------------------------------------------

        private void Finale()
        {
            AvviaDialogo(Storia.CreaDialogoFinale());
            if (memoria.HaScelto("gioco_finito"))
            {
                MostraEpilogo();
                inEsecuzione = false;
            }
        }

        // Il finale cambia in base alle scelte salvate nella Memoria
        private void MostraEpilogo()
        {
            schermo.PulisciMessaggi();
            schermo.AggiungiMessaggio("========== EPILOGO ==========");

            if (memoria.HaScelto("aldo_liberato"))
            {
                if (memoria.HaScelto("promessa_aldo"))
                {
                    schermo.AggiungiMessaggio("Hai mantenuto la tua promessa: tu e Aldo uscite insieme sotto le stelle.");
                }
                else
                {
                    schermo.AggiungiMessaggio("Anche senza averglielo promesso, sei tornato a liberare Aldo. Non lo dimenticherà mai.");
                }
            }
            else if (memoria.HaScelto("promessa_aldo"))
            {
                schermo.AggiungiMessaggio("Hai infranto la promessa fatta ad Aldo. Le sue grida ti seguono mentre sali la scala...");
            }
            else if (memoria.HaScelto("aiutato_aldo"))
            {
                schermo.AggiungiMessaggio("La pozione che hai donato ad Aldo gli darà la forza di resistere ancora un po'.");
            }
            else if (memoria.HaScelto("rifiutato_aldo"))
            {
                schermo.AggiungiMessaggio("Del vecchio prigioniero non ti importa nulla. Non ti volti nemmeno.");
            }
            else
            {
                schermo.AggiungiMessaggio("Del vecchio nella cella ti resta solo un vago ricordo.");
            }

            if (memoria.HaScelto("porta_sfondata"))
            {
                schermo.AggiungiMessaggio("Il frastuono della porta sfondata ha messo in allarme la prigione: presto ti daranno la caccia.");
            }
            else if (memoria.HaScelto("porta_scassinata") || memoria.HaScelto("porta_magia"))
            {
                schermo.AggiungiMessaggio("Hai aperto la porta in silenzio: nessuno si accorgerà della tua fuga fino all'alba.");
            }

            if (goblin.EVivo() && !memoria.HaScelto("goblin_sveglio"))
            {
                schermo.AggiungiMessaggio("Il goblin sta ancora dormendo. Sei sgattaiolato via come un'ombra.");
            }
            else if (goblin.EVivo())
            {
                schermo.AggiungiMessaggio("Sei riuscito a seminare il goblin, ma lui ha visto bene la tua faccia...");
            }

            schermo.AggiungiMessaggio("");
            schermo.AggiungiMessaggio("FINE. " + giocatore.Nome + " il " + giocatore.Classe + " è libero, con "
                + giocatore.PuntiVita + "/" + giocatore.PuntiVitaMax + " PV.");
            schermo.AttendiTasto();
        }

        private void GameOver()
        {
            schermo.AggiungiMessaggio("");
            schermo.AggiungiMessaggio("========== GAME OVER ==========");
            schermo.AggiungiMessaggio("La tua avventura nelle Prigioni di Grimmorn finisce qui.");
            schermo.AttendiTasto();
        }

        private void ChiediUscita()
        {
            string[] opzioni = { "Sì, esci dal gioco", "No, continua a giocare" };
            schermo.AggiungiMessaggio("Vuoi davvero uscire?");
            if (schermo.ChiediScelta(opzioni) == 0)
            {
                inEsecuzione = false;
            }
        }
    }
}
