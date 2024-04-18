using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour // Define una clase llamada "Deck" que hereda de "MonoBehaviour"
{
    [SerializeField] private List<GameObject> deck; // Lista para almacenar el resto de las cartas del mazo que no están en la mano

    void Awake() // Método que se llama cuando se crea una instancia de la clase
    {
       deck = ShuffleDeck(deck); // Baraja el mazo
    }

    /// <summary>
    /// Baraja una lista de entrada de objetos de juego.
    /// </summary>
    List<GameObject> ShuffleDeck(List<GameObject> deckTemp) // Método para barajar el mazo
    {
        for (int i = 0; i < deckTemp.Count; i++) // Para cada carta en el mazo
        {
            GameObject temp = deckTemp[i]; // Almacena la carta actual en una variable temporal
            int randomIndex = Random.Range(i, deckTemp.Count); // Genera un índice aleatorio entre el índice actual y el número total de cartas
            deckTemp[i] = deckTemp[randomIndex]; // Coloca la carta en el índice aleatorio en la posición actual
            deckTemp[randomIndex] = temp; // Coloca la carta temporal en la posición del índice aleatorio
        }
        return deckTemp; // Devuelve el mazo barajado
    }

    /// <summary>
    /// Roba el número correspondiente de cartas de este mazo y las añade a la mano.
    /// </summary>
    public void StealNCardsFromDeck(int nCardsToSteal, int nHandCardsLimit, GameObject hand) // Método para robar cartas del mazo
    {
        int limit = nHandCardsLimit - hand.transform.childCount; // Calcula el límite de cartas que se pueden robar

        if(limit > deck.Count) // Si el límite es mayor que el número de cartas en el mazo
        {
            limit = deck.Count; // Establece el límite al número de cartas en el mazo
        }
        for (int i = 0; i < nCardsToSteal && i < limit ; i++) { // Para cada carta hasta el número de cartas a robar o el límite
            GameObject temp = Instantiate(deck[0], hand.transform); // Crea una instancia de la primera carta del mazo y la coloca como hija de la mano
            temp.GetComponent<Card>().player = GetComponent<Player>(); // Establece el jugador de la carta a este jugador
            deck.RemoveAt(0); // Elimina la primera carta del mazo
        }   
    }
}

