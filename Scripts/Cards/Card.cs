using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour // Define una clase llamada "Card" que hereda de "MonoBehaviour"
{
    [SerializeField] int value = 0; // Variable para almacenar el valor de la carta
    [SerializeField] FieldEnum.fields field; // Variable para almacenar el campo de la carta
    [SerializeField] ScoreEffects.effect ambientalEffect; // Variable para almacenar el efecto ambiental de la carta

    [System.NonSerialized] public Player player; // Referencia al jugador que posee la carta

    /// <summary>
    /// Devuelve el valor de esta carta.
    /// </summary>
    public int GetValue() // Método para obtener el valor de la carta
    {
        return value; // Devuelve el valor de la carta
    }

    /// <summary>
    /// Devuelve el campo de destino de esta carta. Si no tiene destino devuelve indefinido.
    /// </summary>
    public FieldEnum.fields GetField() // Método para obtener el campo de la carta
    {
        return field; // Devuelve el campo de la carta
    }

    /// <summary>
    /// Establece el campo de esta carta. Utilizado para la carta del cuerno del comandante.
    /// </summary>
    public void SetField(FieldEnum.fields field) // Método para establecer el campo de la carta
    {
        this.field = field; // Establece el campo de la carta
    }

    /// <summary>
    /// Devuelve el efecto de esta carta.
    /// </summary>
    public ScoreEffects.effect GetEffect() // Método para obtener el efecto de la carta
    {
        return ambientalEffect; // Devuelve el efecto de la carta
    }

    /// <summary>
    /// Hace interactuable esta carta por la condición de interactuabilidad.
    /// </summary>
    public void MakeInteractable(bool interactability) // Método para hacer interactuable la carta
    {
        gameObject.GetComponent<Button>().interactable = interactability; // Establece la interactuabilidad del botón de la carta
    }

    /// <summary>
    /// Método que se activa al pulsar el botón.
    /// </summary>
    public void CardButton() // Método que se activa al pulsar el botón de la carta
    {
        player.SetSelectedCard(this); // Establece esta carta como la carta seleccionada del jugador
    }
}

