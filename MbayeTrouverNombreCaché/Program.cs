double meilleureNote = 0;

AfficherMessage("Choisissez 'F'-Facile 'D'-Difficile 'Q'-Quitter");

while (true)
{
    string? reponse = LireSaisie();

    // Fin de saisie (Ctrl+Z) : on arrête le programme
    if (reponse is null)
        break;

    if (EstQuitter(reponse))
    {
        EffacerEcran();
        break;
    }

    if (reponse.Equals("F", StringComparison.OrdinalIgnoreCase))
    {
        if (JeuFacile(ref meilleureNote))
            break;
        AfficherMessage("Choisissez 'F'-Facile 'D'-Difficile 'Q'-Quitter");
    }
    else if (reponse.Equals("D", StringComparison.OrdinalIgnoreCase))
    {
        if (JeuDifficile(ref meilleureNote))
            break;
        AfficherMessage("Choisissez 'F'-Facile 'D'-Difficile 'Q'-Quitter");
    }
    else
    {
        AfficherMessage("Choix 'F', 'D' ou 'Q'");
    }
}

// Point 8-a : méthode pour afficher les messages
static void AfficherMessage(string message)
{
    Console.WriteLine(message);
}

// Lit une ligne trimée ; renvoie null si fin de saisie (Ctrl+Z / Ctrl+D)
static string? LireSaisie()
{
    string? ligne = Console.ReadLine();
    if (ligne is null)
        return null;
    return ligne.Trim();
}

// Vrai si la saisie signifie "quitter" (Q, q, quitter…)
static bool EstQuitter(string saisie)
{
    return saisie.Equals("Q", StringComparison.OrdinalIgnoreCase) ||
           saisie.Equals("Quitter", StringComparison.OrdinalIgnoreCase);
}

static void EffacerEcran()
{
    try { Console.Clear(); }
    catch (IOException) { }
    catch (InvalidOperationException) { }
}

static void AfficherMeilleureNote(double note)
{
    AfficherMessage($"Meilleure note : {note:0.#}%");
}

static void AfficherSaisieVide()
{
    AfficherMessage("Saisie vide. Entrez une valeur valide ou 'Q'-Quitter.");
}

// ------------------------------------------------------------------
// Point 6 : mode Facile [1, 10]
// Retours : 6e -> 6b, 6f -> 6a, 6g -> 6b
// Retourne true si le programme doit s'arrêter (fin de saisie)
// ------------------------------------------------------------------
static bool JeuFacile(ref double meilleureNote)
{
    while (true) // boucle nouveau jeu -> retour (6-a)
    {
        // 6-a
        AfficherMessage("choisissez un nombre entre [1, 10] ou 'Q'-Quitter");

        int nombreCache = Random.Shared.Next(1, 11);
        var reponses = new List<int>();

        while (true) // boucle essais -> retour (6-b)
        {
            // 6-b : lire la réponse
            string? saisie = LireSaisie();

            if (saisie is null)
                return true;

            // 6-c : Q -> meilleur note, retour menu (2)
            if (EstQuitter(saisie))
            {
                EffacerEcran();
                AfficherMeilleureNote(meilleureNote);
                return false;
            }

            if (string.IsNullOrWhiteSpace(saisie))
            {
                AfficherSaisieVide();
                continue; // reste en 6-b
            }

            // 6-e : réponse hors [1, 10] -> message, retour 6-b
            if (!int.TryParse(saisie, out int choix) || choix < 1 || choix > 10)
            {
                AfficherMessage("réponse doit être comprise entre 1 et 10");
                continue; // retour 6-b
            }

            reponses.Add(choix);

            // 6-f : gagné -> afficher, pause, effacer écran, retour 6-a
            if (choix == nombreCache)
            {
                AfficherMessage("vous avez gagné");

                // 8-b : note = (etendue - essais) / etendue en %
                int etendue = 10;
                double note = (etendue - reponses.Count) * 100.0 / etendue;
                if (note > meilleureNote)
                    meilleureNote = note;

                AfficherMessage($"Nombre d'essais : {reponses.Count}  Note de la partie : {note:0.#}%");
                AfficherMessage("Appuyez sur Entrée pour continuer...");
                Console.ReadLine();

                EffacerEcran();
                break; // retour 6-a
            }

            // 6-g : perdu -> liste des réponses, retour 6-b
            AfficherMessage("vous avez perdu");
            AfficherMessage($"Réponses déjà données : {string.Join(", ", reponses)}");
            // retour 6-b (sans réafficher 6-a)
        }
    }
}

// ------------------------------------------------------------------
// Point 7 : mode Difficile [min, max]
// Retours : 7d -> 7b, 7i -> 7f, 7j -> 7a, 7k -> 7f, 7h -> 7a
// Retourne true si le programme doit s'arrêter (fin de saisie)
// ------------------------------------------------------------------
static bool JeuDifficile(ref double meilleureNote)
{
    while (true) // boucle nouvel intervalle -> retour (7-a)
    {
        // 7-a
        AfficherMessage("Entrez les bornes min et max de l'intervalle, ou 'Q'-Quitter");

        int min = 0, max = 0;

        while (true) // saisie des bornes -> retour (7-b)
        {
            // 7-b
            string? saisie = LireSaisie();

            if (saisie is null)
                return true;

            // 7-c : Q -> meilleur note, retour menu (2)
            if (EstQuitter(saisie))
            {
                EffacerEcran();
                AfficherMeilleureNote(meilleureNote);
                return false;
            }

            if (string.IsNullOrWhiteSpace(saisie))
            {
                AfficherSaisieVide();
                continue; // reste en 7-b
            }

            string[] bornes = saisie.Split(new[] { ' ', ',', ';', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            // 7-d : bornes invalides -> message, retour 7-b
            if (bornes.Length != 2 ||
                !int.TryParse(bornes[0], out min) ||
                !int.TryParse(bornes[1], out max) ||
                min >= max || max - min <= 10)
            {
                AfficherMessage("Choisissez 2 nombres entiers et (max – min) doit être > à 10");
                continue; // retour 7-b
            }

            break; // bornes valides -> 7-e
        }

        // 7-e : générer le nombre caché
        int nombreCache = Random.Shared.Next(min, max + 1);
        int etendue = max - min + 1;
        var reponses = new List<int>();

        while (true) // boucle de jeu -> retour (7-f)
        {
            // 7-f
            AfficherMessage($"choisissez un nombre entre [{min}, {max}] ou 'Q'-Quitter");

            // 7-g
            string? choixSaisie = LireSaisie();

            if (choixSaisie is null)
                return true;

            // 7-h : Q -> effacer écran, retour 7-a
            if (EstQuitter(choixSaisie))
            {
                EffacerEcran();
                break; // retour 7-a
            }

            if (string.IsNullOrWhiteSpace(choixSaisie))
            {
                AfficherSaisieVide();
                continue; // retour 7-f
            }

            // 7-i : hors intervalle -> message, retour 7-f
            if (!int.TryParse(choixSaisie, out int choix) || choix < min || choix > max)
            {
                AfficherMessage($"Choix doit être compris entre {min} et {max}");
                continue; // retour 7-f
            }

            reponses.Add(choix);

            // 7-j : gagné -> afficher, pause, effacer écran, retour 7-a
            if (choix == nombreCache)
            {
                AfficherMessage("vous avez gagné");

                double note = (etendue - reponses.Count) * 100.0 / etendue;
                if (note > meilleureNote)
                    meilleureNote = note;

                AfficherMessage($"Nombre d'essais : {reponses.Count}  Note de la partie : {note:0.#}%");
                AfficherMessage("Appuyez sur Entrée pour continuer...");
                Console.ReadLine();

                EffacerEcran();
                break; // retour 7-a
            }

            // 7-k : perdu -> retour 7-f
            AfficherMessage("vous avez perdu");
            // retour 7-f (réaffiche le prompt de choix)
        }
    }
}
