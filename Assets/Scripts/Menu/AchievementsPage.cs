using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsPage : MonoBehaviour
{
    [field: SerializeField]
    public List<AchievementHolder> Holders {  get; private set; }
}
