using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObjects/Bullet", order = 2)]
public class BulletSO : ScriptableObject
{
    public float maxDistance = 10f;
    public float knockbackForce = 10f;
    public int damage = 10;
    public float speed = 10f;
    public bool canPierce = false;
}
