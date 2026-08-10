using UnityEngine;

public abstract class CharacterStatModifierSO : ScriptableObject
{
    public abstract bool AffectCharacter(GameObject character, float val);
}
