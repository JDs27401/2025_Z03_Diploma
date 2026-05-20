using Enemy.Scripts;
using UnityEngine;

namespace C__Classes.Commands
{
    public abstract class Command : MonoBehaviour
    {
        public static void MoveToTarget(NpcBase n)
        {
            if (!n.PlayerTarget) return;

            n.PathUpdateTimer += Time.deltaTime;
            if (n.PathUpdateTimer < n.PathUpdateDelay)
            {
                return;
            }
            n.Agent.SetDestination(n.PlayerTarget.position);
            n.PathUpdateTimer = 0f;
        }
    }
}