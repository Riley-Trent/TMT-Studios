using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI cardDescriptionText;
    [SerializeField] private Image cardImage;
    private Card card;
    private CardSelectionManager manager;

    public void Setup(Card card, CardSelectionManager manager)
    {
    
    // Check if the components you're accessing are valid
    
        this.card = card;
        this.manager = manager;
        cardNameText.text = card.cardName;
        cardDescriptionText.text = card.description;
        
        cardImage.sprite = card.cardImage;  // Set the sprite only if it's valid


        GetComponent<Button>().onClick.AddListener(() => SelectCard());
        
    }

    public void SelectCard()
    {
        manager.SelectCard(card);
    }
}
