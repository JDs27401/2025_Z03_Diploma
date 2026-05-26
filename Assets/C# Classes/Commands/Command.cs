using System;
using Enemy.Scripts;
using UnityEngine;

namespace C__Classes.Commands
{
    public abstract class Command : ICommand
    {
        protected NpcBase npc;

        protected Command(NpcBase npc)
        {
            if (npc == null)
            {
                throw new ArgumentNullException();
            }
            this.npc = npc;
        }
        
        public virtual void Execute()
        {
        }
    }
}