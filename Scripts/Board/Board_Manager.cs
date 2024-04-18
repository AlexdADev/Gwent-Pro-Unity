using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Es un ayudante para ambos gestores de tablero de jugadores. Aplica efectos climáticos globales a ambos tableros.
/// </summary>
public class Board_Manager : MonoBehaviour
{
    [SerializeField] PJBoard_Manager managerHuman; // Referencia al gestor del tablero del jugador humano
    [SerializeField] PJBoard_Manager managerAI; // Referencia al gestor del tablero del jugador AI

    Transform effectLayout; // Referencia a un objeto Transform para organizar los efectos visuales

    Virtual_Board virtualBoard; // Representación virtual de los tableros de los jugadores

    private void Awake()
    {
        effectLayout = transform.GetChild(transform.childCount - 1); // Inicializa effectLayout con el último hijo del objeto Transform

        virtualBoard = new Virtual_Board(managerHuman,3); // Crea una nueva instancia de Virtual_Board
    }

    public void ApplyGlobalEffect(PJBoard_Manager sender, ScoreEffects.effect effect, GameObject card)
    {
        if (sender == managerAI) // Si el emisor es el jugador AI
        {
            managerHuman.ApplyEffect(effect); // Aplica el efecto al tablero del jugador humano
        }
        else // Si el emisor es el jugador humano
        {
            managerAI.ApplyEffect(effect); // Aplica el efecto al tablero del jugador AI
        }

        if (effect == ScoreEffects.effect.sun) // Si el efecto es "sun"
        {
            for (int i = 0; i < effectLayout.childCount; i++) // Para cada hijo de effectLayout
            {
                Destroy(effectLayout.GetChild(i).gameObject); // Destruye el objeto hijo
            }
            Destroy(card); // Destruye la tarjeta que causó el efecto
        }
        else // Si el efecto no es "sun"
        {
            card.transform.SetParent(transform.GetChild(transform.childCount - 1)); // Cambia el padre de la tarjeta al último hijo del objeto Transform
        }
    }

    public void UpdateBoardAndHands() // Actualiza los tableros y las manos de los jugadores
    {
        UpdateSubBoardsBoard(); // Actualiza los tableros de los jugadores
        UpdateVirtualBoardHands(); // Actualiza las manos de los jugadores en el tablero virtual
    }

    public void UpdateSubBoardsBoard() // Actualiza los tableros de los jugadores
    {
        Field[] temp = managerHuman.GetAllBoardPlayerInfo(); // Obtiene la información del tablero del jugador humano
        virtualBoard.SetSubBoard(temp, true); // Establece la información en el tablero virtual

        temp = managerAI.GetAllBoardPlayerInfo(); // Obtiene la información del tablero del jugador AI
        virtualBoard.SetSubBoard(temp, false); // Establece la información en el tablero virtual
    }

    public void UpdateVirtualBoardHands() // Actualiza las manos de los jugadores en el tablero virtual
    {
        virtualBoard.SetHand(managerHuman.fieldsParent.transform.GetChild(3), true); // Actualiza la mano del jugador humano
        virtualBoard.SetHand(managerAI.fieldsParent.transform.GetChild(3), false); // Actualiza la mano del jugador AI
    }
}
