using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossB : AbstractBoss
{
    void Update()
    {
        CheckPlayer(0.2f);
    }

    protected override void BossPattern() { }
}
