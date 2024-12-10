using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public string description;
    public CardEffectType effectType;
    public float effectValue; // Value of the card's effect
    public Sprite cardImage;
}

public enum CardEffectType
{
    HealthBoost,
    DamageBoost,
    ExperienceBoost,
    SpeedBoost,
    DefenseBoost
}