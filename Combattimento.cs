using System;

namespace GiocoDiRuolo
{
    // Gestisce un combattimento a turni tra il giocatore e un nemico
    public class Combattimento
    {
        private const int USI_ABILITA_PER_COMBATTIMENTO = 2;

        private Giocatore giocatore;
        private Nemico nemico;
        private Schermo schermo;
        private Random casuale;

        private bool inDifesa;   // true se il giocatore ha scelto "Difenditi" in questo turno
        private int usiAbilita;  // quante volte può ancora usare l'abilità speciale

        public Combattimento(Giocatore giocatore, Nemico nemico, Schermo schermo, Random casuale)
        {
            this.giocatore = giocatore;
            this.nemico = nemico;
            this.schermo = schermo;
            this.casuale = casuale;
        }

        // tutorial: mostra le spiegazioni e non permette la fuga
        // attaccoASorpresa: il giocatore colpisce per primo con danno doppio
        public RisultatoCombattimento Combatti(bool tutorial, bool attaccoASorpresa)
        {
            inDifesa = false;
            usiAbilita = USI_ABILITA_PER_COMBATTIMENTO;
            schermo.NemicoInCombattimento = nemico;

            schermo.AggiungiMessaggio("");
            schermo.AggiungiMessaggio("*** COMBATTIMENTO: " + giocatore.Nome + " contro " + nemico.Nome + " ***");

            if (tutorial)
            {
                MostraTutorial();
            }

            if (attaccoASorpresa)
            {
                int danno = giocatore.CalcolaDanno() * 2 - nemico.Difesa;
                if (danno < 1)
                {
                    danno = 1;
                }
                nemico.SubisciDanno(danno);
                schermo.AggiungiMessaggio("Attacco a sorpresa! Colpisci " + nemico.Nome + " nel sonno e infliggi " + danno + " danni!");
            }

            // Il combattimento continua finché uno dei due non è sconfitto
            while (giocatore.EVivo() && nemico.EVivo())
            {
                // Nel tutorial non si può fuggire, quindi c'è un'opzione in meno
                string[] opzioni;
                if (tutorial)
                {
                    opzioni = new string[4];
                }
                else
                {
                    opzioni = new string[5];
                    opzioni[4] = "Tenta la fuga";
                }
                opzioni[0] = "Attacca con " + giocatore.ArmaEquipaggiata.Nome;
                opzioni[1] = "Abilità: " + giocatore.NomeAbilita() + " (usi rimasti: " + usiAbilita + ")";
                opzioni[2] = "Bevi una pozione (ne hai " + giocatore.ContaPozioni() + ")";
                opzioni[3] = "Difenditi (dimezza il prossimo danno)";

                int scelta = schermo.ChiediScelta(opzioni);
                bool turnoUsato = true;

                if (scelta == 0)
                {
                    AttaccoNormale();
                }
                else if (scelta == 1)
                {
                    turnoUsato = UsaAbilita();
                }
                else if (scelta == 2)
                {
                    turnoUsato = UsaPozione();
                }
                else if (scelta == 3)
                {
                    inDifesa = true;
                    schermo.AggiungiMessaggio("Alzi la guardia e ti prepari a ricevere il colpo.");
                }
                else if (scelta == 4)
                {
                    if (TentaFuga())
                    {
                        schermo.NemicoInCombattimento = null;
                        return RisultatoCombattimento.Fuga;
                    }
                }

                // Se il nemico è ancora vivo e il giocatore ha agito, tocca al nemico
                if (nemico.EVivo() && turnoUsato)
                {
                    TurnoNemico();
                }
            }

            schermo.NemicoInCombattimento = null;

            if (giocatore.EVivo())
            {
                schermo.AggiungiMessaggio(nemico.Nome + " è stato sconfitto!");
                schermo.AttendiTasto();
                return RisultatoCombattimento.Vittoria;
            }

            schermo.AggiungiMessaggio("Sei stato sconfitto da " + nemico.Nome + "...");
            schermo.AttendiTasto();
            return RisultatoCombattimento.Sconfitta;
        }

        private void MostraTutorial()
        {
            schermo.AggiungiMessaggio("TUTORIAL - Il combattimento è a turni: scegli un'azione premendo il numero corrispondente.");
            schermo.AggiungiMessaggio("Attacca: la probabilità di colpire dipende dalla Destrezza, il danno dall'arma e dalla sua statistica.");
            schermo.AggiungiMessaggio("Abilità: la mossa speciale della tua classe, usabile " + USI_ABILITA_PER_COMBATTIMENTO + " volte per combattimento.");
            schermo.AggiungiMessaggio("Pozione: recupera punti vita. Difenditi: dimezza il danno del prossimo attacco nemico.");
            schermo.AggiungiMessaggio("Da questo combattimento non si può fuggire!");
        }

        private void AttaccoNormale()
        {
            int probabilita = 60 + giocatore.Destrezza * 4 - nemico.Schivata;
            if (probabilita > 95)
            {
                probabilita = 95;
            }

            if (casuale.Next(100) < probabilita)
            {
                int danno = giocatore.CalcolaDanno() + casuale.Next(0, 3) - nemico.Difesa;
                if (danno < 1)
                {
                    danno = 1;
                }

                // Colpo critico: più alta la Destrezza, più è probabile
                if (casuale.Next(100) < giocatore.Destrezza * 3)
                {
                    danno = danno * 2;
                    schermo.AggiungiMessaggio("Colpo critico!");
                }

                nemico.SubisciDanno(danno);
                schermo.AggiungiMessaggio("Colpisci " + nemico.Nome + " con " + giocatore.ArmaEquipaggiata.Nome + " e infliggi " + danno + " danni.");
            }
            else
            {
                schermo.AggiungiMessaggio("Il tuo attacco va a vuoto!");
            }
        }

        // Restituisce false se l'abilità non è stata usata (così il turno non passa)
        private bool UsaAbilita()
        {
            if (usiAbilita <= 0)
            {
                schermo.AggiungiMessaggio("Sei troppo stanco per usare di nuovo " + giocatore.NomeAbilita() + ".");
                return false;
            }
            usiAbilita--;

            int danno;
            if (giocatore.Classe == ClassePersonaggio.Guerriero)
            {
                // Tanto danno, ma può mancare il bersaglio
                if (casuale.Next(100) < 70)
                {
                    danno = giocatore.CalcolaDanno() + giocatore.Forza - nemico.Difesa;
                    nemico.SubisciDanno(danno);
                    schermo.AggiungiMessaggio("COLPO POSSENTE! Abbatti la tua arma su " + nemico.Nome + ": " + danno + " danni!");
                }
                else
                {
                    schermo.AggiungiMessaggio("Carichi il colpo possente... ma " + nemico.Nome + " si sposta all'ultimo!");
                }
            }
            else if (giocatore.Classe == ClassePersonaggio.Mago)
            {
                // Colpisce sempre e ignora la difesa
                danno = giocatore.Intelligenza * 2 + casuale.Next(1, 5);
                nemico.SubisciDanno(danno);
                schermo.AggiungiMessaggio("PALLA DI FUOCO! Le fiamme avvolgono " + nemico.Nome + ": " + danno + " danni!");
            }
            else
            {
                // Colpisce sempre, il danno cresce con la Destrezza
                danno = giocatore.CalcolaDanno() + giocatore.Destrezza - nemico.Difesa;
                nemico.SubisciDanno(danno);
                schermo.AggiungiMessaggio("COLPO FURTIVO! Scivoli alle spalle di " + nemico.Nome + " e colpisci: " + danno + " danni!");
            }
            return true;
        }

        private bool UsaPozione()
        {
            Pozione pozione = giocatore.TrovaPozione();
            if (pozione == null)
            {
                schermo.AggiungiMessaggio("Non hai pozioni!");
                return false;
            }
            int curati = giocatore.BeviPozione(pozione);
            schermo.AggiungiMessaggio("Bevi " + pozione.Nome + " e recuperi " + curati + " PV.");
            return true;
        }

        private bool TentaFuga()
        {
            int probabilita = 30 + giocatore.Destrezza * 5;
            if (casuale.Next(100) < probabilita)
            {
                schermo.AggiungiMessaggio("Riesci a sfuggire a " + nemico.Nome + "!");
                schermo.AttendiTasto();
                return true;
            }
            schermo.AggiungiMessaggio("Provi a scappare, ma " + nemico.Nome + " ti blocca la strada!");
            return false;
        }

        private void TurnoNemico()
        {
            int probabilita = nemico.Precisione - giocatore.Destrezza * 3;
            if (probabilita < 10)
            {
                probabilita = 10;
            }

            if (casuale.Next(100) < probabilita)
            {
                int danno = nemico.Attacco + casuale.Next(0, 3) - giocatore.CalcolaDifesa();
                if (inDifesa)
                {
                    danno = danno / 2;
                }
                if (danno < 1)
                {
                    danno = 1;
                }
                giocatore.SubisciDanno(danno);
                schermo.AggiungiMessaggio(nemico.Nome + " ti colpisce e ti infligge " + danno + " danni.");
            }
            else
            {
                schermo.AggiungiMessaggio(nemico.Nome + " attacca, ma riesci a schivare!");
            }

            inDifesa = false;
        }
    }
}
