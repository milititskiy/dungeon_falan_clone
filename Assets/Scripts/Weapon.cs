public class Weapon
{
    public string type;
    public int damage;
    public string range;
    public string specialEffect;

    public Weapon(string type, int damage, string range, string specialEffect)
    {
        this.type = type;
        this.damage = damage;
        this.range = range;
        this.specialEffect = specialEffect;
    }
}
