using UnityEngine;

[CreateAssetMenu]
public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override bool AffectCharacter(GameObject character, float val)
    {
        Health health = character.GetComponent<Health>();
        if (health != null)
        {
            return health.AddHealth((int)val) > 0;
        }

        return false;
    }
}
