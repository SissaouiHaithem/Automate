using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//moi meme

namespace TPPIF1006
{
    public class Automate
    {
        public State InitialState { get; private set; }
        public State CurrentState { get; private set; }
        public List<State> States { get; private set; }
        public bool IsValid { get; private set; }
        //constructeur
        public Automate(string filePath)
        {
            States = new List<State>();
            LoadFromFile(filePath);
        }
        //pour rechercher un etat
        State FindStateByName(string name)
        {
            return States.FirstOrDefault(s => s.Name == name);
        }
        
        //methode pour charger et lire le fichier ligne par ligne 
        private void LoadFromFile(string filePath)
        {   //verifie s'il s'agit d'un automate deterministe ou non
            Transition checkTransition;
            List<State> stateAvecTransitionMlutiples = new List<State>();
            
            if(!File.Exists(filePath))
            {
                Console.WriteLine("\nFichier non trouvé.");
                IsValid = false;
                return;
            }
            Console.WriteLine("\nFichier chargé avec succès !");
            //lire chaque fichier ligne par lignes
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {   //supprime tous les espaces vides dans une ligne et retourne un tableau de string 
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 1) continue;  //si on obtient une ligne vide, on passe à la ligne suiv
                
                //Dans le cas où il s'agit d'un ETAT
                if (parts[0] == "state" && parts.Length >= 4)
                {
                    string stateName = parts[1];     //le nom de l'etat est enreigistré
                    bool isFinal = parts[2] == "1";  //si part[2] == 1. isFinal est true(l'etat est dit final) dans le cas contraire l'etat n'est pas final
                    bool isInitial = parts[3] == "1";  //même chose que la ligne précédente mais il sera etat initial ou pas

                    if(FindStateByName(stateName) != null) continue;  //verifie si l'etat exite déjà si c'est le cas on saute à l'iteration suivante
                    
                    //création de l'etat et ajout de l'etat dans notre liste states
                    var state = new State(stateName, isFinal);
                    States.Add(state);       
                    
                    //si un etat initial est trouvé
                    if (isInitial)
                    {
                        //si un etat initial existe déjà, on ne peut rajouter un autre etat initial.
                        if (InitialState != null)
                        {
                            Console.WriteLine($"Automate non determinste car Plusieurs états initiaux définis : ({InitialState.Name} et {stateName}).");
                            Console.WriteLine("\n\tAutomate invalide");
                            IsValid = false;
                            return;
                        }
                        else
                            InitialState = state; 
                        
                        state.IsInitial = true; //marqué l'etat comme un etat initial 
                    }
                }
                
                //s'il s'agit d'une TRANSITION au lieu d'un etat
                else if (parts[0] == "transition" && parts.Length >= 4)
                {
                    //récupère le nom de l'etat qui va transité appellé ici "sourceNameState" (etat source)
                    string sourceNameState = parts[1];
                    
                    //conversion de la valeur de la transition en char
                    char.TryParse(parts[2], out char input);
                    //on a une erreur si l'input n'est pas une valeur 0 ou 1
                    if (input != '1' && input != '0')
                    {
                        Console.WriteLine($"Erreur: Entrée invalide dans transition. Transition ignoré ! : {sourceNameState} -> {parts[2]}.");
                        continue;
                    }
                    
                    //destination de la transition où "etat source" va transité. appellé ici target name (etat ciblé) 
                    string targetName = parts[3];
                    
                    // on verifie si les deux etats existe avant de faire la transition
                    var sourceState = FindStateByName(sourceNameState);
                    var targetState = FindStateByName(targetName);
                    
                    if (sourceState != null && targetState != null)
                    {  
                        //verifi s'il s'agit d'un automate deterministe ou pas
                        checkTransition = sourceState.Transitions.FirstOrDefault(t=>t.Input == input);
                        
                        if (checkTransition != null)
                        {
                            stateAvecTransitionMlutiples.Add(sourceState);
                        }
                        // Ajoute la transition si les deux états sont trouvés
                        sourceState.Transitions.Add(new Transition(input, targetState));
                    }
                    else
                    {
                        Console.WriteLine($"Erreur: État source ou cible manquant pour la transition {sourceNameState} -> {targetName}.");
                    }
                }
            }
            
            //si on a aucun etat initial ou si notre liste d'etat est vide alors l'automate est invalide
            if (InitialState == null || !States.Any())
            {
                Console.WriteLine("Erreur: Automate invalide car sans état initial ou sans états.");
                IsValid = false;
                return;
            }
            //si tout est conforme alors on a un automate valide
            IsValid = true;
            
            //S'il s'agit d'un automate non deterministe
            if(stateAvecTransitionMlutiples.Count() != 0)
            {
                Console.WriteLine("\n\t\tAutomate non determinste ! L'etat et la transition en defaut : ");
                foreach (var s in stateAvecTransitionMlutiples)
                {
                    Console.WriteLine($"{s.ToString()}");
                }
                //si l'automate est non deterministe on arrete le programme
                Console.WriteLine("\n\tAutomate invalide");
                IsValid = false;
                return;
            }
            
            if (stateAvecTransitionMlutiples.Count() == 0 && InitialState != null && States.Count() != 0)
            {
                Console.WriteLine("\n\t\tAutomate Deterministe !");
            }
            
        }
        
        public async Task<bool> Validate(string input)
        {
            bool isValid = true;
            Reset();
            //
            if (!IsValid)
            {
                Console.WriteLine("Automate invalide.");
                return false;
            }
            
            foreach (char c in input)
            {
                //cherche une transition qui correspond à la valeur courante "c" et nous retourne cette transition
                var transition = CurrentState.Transitions.Find(t => t.Input == c);
                //var st = States.Find(s => s.Transitions.Find(t => t == transition));
                if (transition == null)
                {
                    Console.WriteLine($"Aucune transition trouvée pour l'entrée '{c}'.");
                    CurrentState.IsFinal = false;
                    break;
                }
                else
                {
                    Console.WriteLine($"etat actuel {CurrentState.Name} {transition}");
                    CurrentState = transition.TransiteTo;  //on met a jour le current state qui devient l'etat suivant
                }

                await Task.Delay(1000);
            }

             isValid = CurrentState.IsFinal;
            Console.WriteLine(isValid
                ? $"La chaîne est acceptée. État final atteint: {CurrentState.Name}"
                : "La chaîne est rejetée. État final non atteint.");
            return isValid;
        }

        public void Reset()
        {
            //on remet l'automate a l'etat initail
            CurrentState = InitialState;
            Console.WriteLine("Automate réinitialisé.");
        }

        public override string ToString()
        {
            // Vérifie s'il n'y a aucun état dans l'automate
            if (States == null || States.Count == 0)
            {
                return "L'automate ne contient aucun état.";
            }

            // on uilise un StringBuilder pour construire le résultat efficacement
            var sb = new StringBuilder();
            sb.AppendLine("=== Automate ===");

            // Parcourt tous les états
            foreach (var state in States)
            {
                //chaque etats dans la liste appelle la methode toString
                sb.AppendLine(state.ToString());
            }

            return sb.ToString();
        }
    }
}
