using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class TargetManager : Singleton<TargetManager>
{
   public List<Transform> enemyTransforms = new List<Transform>();
}
