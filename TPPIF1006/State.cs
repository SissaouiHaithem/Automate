using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//moi meme
namespace TPPIF1006
{
    public class State
    {
        public bool IsFinal {get; set;}
        public string Name { get; private set; }
        public bool IsInitial { get; set; }
        public List<Transition> Transitions { get; private set; }

        public State (string name, bool isFinal)
        {
            Name = name;
            IsFinal = isFinal;
            IsInitial = false;
            Transitions = new List<Transition>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
                
            if(IsFinal && IsInitial)            //etat final et etat initial en même temps [(s)]
                sb.AppendLine($"[({Name})] ");  
            else if(IsFinal && !IsInitial)      //etat final seulement (s)
                sb.AppendLine($"({Name}) ");
            else if(!IsFinal && IsInitial)      //etat initial seulement [s]
                sb.AppendLine($"[{Name}] ");   
            else
            {
                sb.AppendLine($"{Name} ");     //c'est juste un etat simple
            }
            
            if (Transitions == null || Transitions.Count == 0)
            {
                sb.AppendLine("  Aucune transition.");
            }
            else
            {
                // Parcourt les transitions associées à l'état
                foreach (var transition in Transitions)
                {
                    sb.AppendLine(transition.ToString());
                }
            }
            
            return sb.ToString(); 
        }

    }
}