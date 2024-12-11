using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject cardSelectionUI;
    [SerializeField] private List<Card> availableCards;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameManager gameManager;

    private List<Card> currentCards = new List<Card>();

    public void ShowCardSelection()
    {
        cardPrefab.SetActive(true);
        gameManager.CursorOn();
        // Clear previous cards
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }
        currentCards.Clear();

        // Display 3-5 random cards
        for (int i = 0; i < 3; i++) // Example: Show 3 cards
        {
            Card randomCard = availableCards[Random.Range(0, availableCards.Count)];
            GameObject cardInstance = Instantiate(cardPrefab, cardContainer);
            if (cardInstance == null)
        {
            Debug.LogError("cardPrefab is null or failed to instantiate.");
            return;
        }
            cardInstance.GetComponent<CardDisplay>().Setup(randomCard, this);
            currentCards.Add(randomCard);
        }
        ShowCardSelectionUI();
        SoundManager.PlaySound(SoundType.CARDSHUFFLE);
        cardSelectionUI.SetActive(true);
        cardPrefab.SetActive(false);
    }

    public void SelectCard(Card selectedCard)
    {
        playerStats.UpdateHealthDisplay();
        SoundManager.PlaySound(SoundType.CARDDEAL);
        ApplyCardEffect(selectedCard);
        gameManager.CursorOff();
        HideCardSelectionUI();
    }

    private void ApplyCardEffect(Card card)
    {
        // Example: Modify player stats based on card type
        switch (card.effectType)
        {
            case CardEffectType.HealthBoost:
                playerStats.maxHealth += card.effectValue;
                playerStats.Health += card.effectValue;
                break;
            case CardEffectType.DamageBoost:
                //playerStats.fpsController.walkSpeed += card.effectValue;
                break;
            case CardEffectType.ExperienceBoost:
                playerStats.experienceMultiplier += Mathf.RoundToInt(card.effectValue);
                break;
            case CardEffectType.DefenseBoost:
                playerStats.defense += Mathf.RoundToInt(card.effectValue);
                break;
            case CardEffectType.GunBoost:
                break;
        }

        Debug.Log($"Applied {card.cardName}: {card.effectType} +{card.effectValue}");
    }

    void ShowCardSelectionUI()
    {
        cardPrefab.SetActive(true);
        cardContainer.gameObject.SetActive(true);
        cardSelectionUI.SetActive(true);
    }

    void HideCardSelectionUI()
    {
        cardPrefab.SetActive(false);
        cardContainer.gameObject.SetActive(false);
    }
    
}
