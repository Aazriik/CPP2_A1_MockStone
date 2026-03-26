public interface IDamageable
{
    // Any script that uses this interface MUST have a TakeDamage method
    void TakeDamage(int damageAmount);
}