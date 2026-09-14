using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class CharacterEventsHandler
{
    public static UnityAction<GameObject, int> damageTaken;
    public static UnityAction<GameObject, int> healed;
    public static UnityAction<GameObject> characterDied;
}
