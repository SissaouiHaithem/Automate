using System;

namespace TPPIF1006
{

    class Program
    {
        public static async Task Main(string[] args)
        {
            // Remonter deux niveaux pour atteindre la racine du projet afin de pouvoir utiliser le chemin relative du fichier
            string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
            
            //Automate automate = new Automate(fullPath);
            Automate automate = null; // Automate chargé

            Console.WriteLine("\t\t\t=== Bienvenue dans notre Programme ===\n\n");
            
            Console.Write("Tapez ENTER pour charger le fichier conforme par defaut, OU\nEntrez le nom du fichier contenant votre automate : ");
            string fileName = Console.ReadLine();

            // Utiliser le fichier par défaut si aucun nom n'est fourni
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "automate.txt";    //si vous voulez testez un autre fichier changez le nom ici
            }
            
            // Charger l'automate
            try
            {
                // Construire le chemin complet à partir de la racine du projet
                string relativePath = fileName;
                string fullPath = Path.Combine(projectRoot, relativePath);
                
                 automate = new Automate(fullPath);// le constructeur gère le chargement du fichier
                 
            }catch (Exception ex) {
                Console.WriteLine($"Erreur : {ex.Message}");
                return;
            }
            
            //si l'automate est invalide on arrête le program et affiche un messsage
            if(!automate.IsValid)
                return;
            
            // Boucle après chargement du fichier
            while (true)
            {
                
                Console.WriteLine("\n");
                Console.WriteLine("1. Afficher l'automate chargé");
                Console.WriteLine("2. Valider une expression sur l'automate chargé");
                Console.WriteLine("3. Quitter");
                Console.Write("Votre choix : ");
            
                string choix = Console.ReadLine();
                Console.Clear();

                switch (choix)
                {
                    case "1":
                        // Afficher l'automate
                        Console.WriteLine(automate.ToString());
                        Console.WriteLine("TAPEZ ENTREZ POUR CONRINUER.");
                        Console.ReadKey();
                        break;

                    case "2":
                        // Valider une expression
                        while (true)
                        {
                            Console.Write("Entrez une expression à valider : ");
                            string input = Console.ReadLine();
                            bool result =  await automate.Validate(input);
                            Console.WriteLine(result ? "Expression acceptée !" : "Expression rejetée !");
                            Console.WriteLine("voulez vous validez une autre expression ? (y/n) : ");
                            char reponse = Console.ReadLine().ToLower()[0];
                            if (reponse != 'y')
                                break;
                            
                        }
                        break;

                    case "3":
                        Console.WriteLine("l'application va se fermer après avoir appuyé sur ENTER. !");
                        Console.ReadKey();
                        Console.WriteLine("Merci d'avoir utilisé ce programme. À bientôt !");
                        return;

                    default:
                        Console.WriteLine("Choix invalide, veuillez réessayer.");
                        Console.ReadKey();
                        break;
                }
                Console.Clear();
            }
            
        }
    }
}
