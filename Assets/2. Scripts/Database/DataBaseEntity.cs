namespace DataBaseEntity
{
    [System.Serializable]
    public class AchieveDBEntity
    {
        public string achieveName;
        public string achieveDescription;
        public int maxAchieveLevel;
        public string maxCondition;
        public string reward;
    }

    [System.Serializable]
    public class WaveDBEntity
    {
        public int enemyACount;
        public float enemyAHp;
        public float enemyADamage;
        public int enemyAPrice;
        public int enemyAJem;

        public int enemyBCount;
        public float enemyBHp;
        public float enemyBDamage;
        public int enemyBPrice;
        public int enemyBJem;

        public int enemyCCount;
        public float enemyCHp;
        public float enemyCDamage;
        public int enemyCPrice;
        public int enemyCJem;

        public int enemyDCount;
        public float enemyDHp;
        public float enemyDDamage;
        public int enemyDPrice;
        public int enemyDJem;

        public float spawnCoolTime;
    }

    [System.Serializable]
    public class MissileDBEntity
    {
        public float damage;
        public float attackSpeed;
        public float expertSkill;
        public int damageUpgradeJem;
        public int attackSpeedUpgradeJem;
        public int expertSkillUpgradeJem;
    }

    [System.Serializable]
    public class BarrierDBEntity
    {
        public float damage;
        public float attackSpeed;
        public float expertSkill;
        public int damageUpgradeJem;
        public int attackSpeedUpgradeJem;
        public int expertSkillUpgradeJem;
    }

    [System.Serializable]
    public class LaserDBEntity
    {
        public float damage;
        public float attackSpeed;
        public float expertSkill;
        public int damageUpgradeJem;
        public int attackSpeedUpgradeJem;
        public int expertSkillUpgradeJem;
    }

    [System.Serializable]
    public class EmpDBEntity
    {
        public float damage;
        public float attackSpeed;
        public float expertSkill;
        public int damageUpgradeJem;
        public int attackSpeedUpgradeJem;
        public int expertSkillUpgradeJem;
    }
}
