using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    // Jugadores en el juego
    [SerializeField] Player humanPlayer;
    [SerializeField] Player player2; //AI

    // Referencia al administrador del tablero
    [SerializeField] Board_Manager boardManager;

    // Variables para controlar el juego
    [SerializeField] int startCards;
    [SerializeField] int cardsToSteal;
    [SerializeField] int maxCardsInHand;

    // Contador de rondas
    int totalRounds;

    // Jugador activo en el turno actual
    Player activePlayer;

    // Inicialización de variables
    void Awake()
    {
        startCards = 10;
        cardsToSteal = 6;
        maxCardsInHand = 10;
        totalRounds = 0;
    }

    // Comienza una nueva ronda con el primer jugador al inicio del juego
    void Start()
    {
        NewRound(startCards, maxCardsInHand);
    }

    // Actualiza el estado del juego en cada frame
    void Update()
    {

    }

    // Controla el cambio de turno entre los jugadores
    public void NextTurn()
    {
        // Comprueba si los jugadores se han rendido y la ronda ha terminado
        bool isNewRound = HavePlayersSurrended();

        // Si la ronda no ha terminado
        if (!isNewRound)
        {
            // Si hay un jugador activo y su turno ha terminado
            if (activePlayer != null)
            {
                if (!activePlayer.IsMyTurn())
                {
                    // Finaliza el turno del jugador activo
                    activePlayer.myTurn = false;
                    // Cambia el turno al siguiente jugador
                    ChangePlayerTurn();
                    // Inicia el turno del nuevo jugador activo
                    activePlayer.SetTurn();
                }
            }

        }
        else
        {
            // Si la ronda ha terminado y no se han jugado todas las rondas
            if (totalRounds < 3)
            {
                // Incrementa el contador de rondas
                totalRounds++;
                // Comienza una nueva ronda
                NewRound(cardsToSteal, maxCardsInHand);
            }
        }
    }


    /// <summary>
    /// Devuelve verdadero si la ronda ha terminado, comprobando si ambos jugadores se han rendido.
    /// </summary>
    /// <returns>Verdadero si ambos jugadores se han rendido, falso en caso contrario</returns>
    bool HavePlayersSurrended()
    {
        bool temp = false;

        // Si ambos jugadores se han rendido
        if (humanPlayer.HasSurrended() && player2.HasSurrended())
        {
            // Reinicia el estado de rendición de los jugadores y aumenta el contador de rondas ganadas del ganador de la ronda
            humanPlayer.givesUp = player2.givesUp = false;
            ++GetRoundWinner().winnedRounds;
            temp = true;
        }

        return temp;
    }

    /// <summary>
    /// Establece el primer turno al jugador recibido como entrada.
    /// </summary>
    /// <param name="player">El jugador al que se le asignará el primer turno</param>
    void SetFirstTurn(Player player)
    {
        // Establece el jugador activo y le da el turno
        activePlayer = player;
        player.SetTurn();
    }

    /// <summary>
    /// Cambia el turno con el siguiente jugador que tiene que jugar.
    /// </summary>
    void ChangePlayerTurn()
    {
        // Obtiene el siguiente jugador y le da el turno
        activePlayer = GetNextPlayer(activePlayer);
        activePlayer.SetTurn();
    }

    /// <summary>
    /// Devuelve el siguiente jugador disponible para jugar, si se ha rendido llama a la misma función (recursividad) hasta encontrarlo.
    /// </summary>
    /// <returns>El siguiente jugador disponible para jugar</returns>
    Player GetNextTurnPlayer()
    {
        // Obtiene el siguiente jugador
        Player tempTurnOwner = humanPlayer == activePlayer ? player2 : humanPlayer;

        // Si el jugador se ha rendido, obtiene el siguiente jugador
        if (tempTurnOwner.HasSurrended())
        {
            tempTurnOwner = GetNextPlayer(tempTurnOwner);
        }

        return tempTurnOwner;
    }



    /// <summary>
    /// Devuelve un jugador u otro dependiendo de quién sea el jugador activo.
    /// </summary>
    /// <param name="player">El jugador actual</param>
    /// <returns>El siguiente jugador</returns>
    Player GetNextPlayer(Player player)
    {
        // Si el jugador humano es el jugador actual, devuelve player2. De lo contrario, devuelve el jugador humano.
        return humanPlayer == player ? player2 : humanPlayer;
    }

    /// <summary>
    /// Devuelve el jugador ganador de una ronda.
    /// </summary>
    /// <returns>El jugador con la puntuación más alta</returns>
    Player GetRoundWinner()
    {
        // Compara las puntuaciones de humanPlayer y player2 y devuelve el jugador con la puntuación más alta.
        return humanPlayer.score > player2.score ? humanPlayer : player2;
    }

    /// <summary>
    /// Restablece todos los atributos de la ronda de los jugadores e indica que roben cartas con las instrucciones de las reglas del controlador del juego. 
    /// También si es la primera ronda da el primer turno al jugador humano y si no da el primer turno al perdedor de la ronda.
    /// </summary>
    /// <param name="nCardsToSteal">Número de cartas a robar</param>
    /// <param name="maxNCardsInHand">Número máximo de cartas en la mano</param>
    void NewRound(int nCardsToSteal, int maxNCardsInHand)
    {
        // Restablece las estadísticas de la ronda para ambos jugadores y les indica que roben cartas.
        humanPlayer.RoundReset(nCardsToSteal, maxNCardsInHand);
        player2.RoundReset(nCardsToSteal, maxNCardsInHand);

        // Si es la primera ronda, da el primer turno al jugador humano. Si no es la primera ronda, da el primer turno al perdedor de la ronda.
        if (activePlayer == null)
        {
            SetFirstTurn(humanPlayer);
        }
        else
        {
            SetFirstTurn(GetNextPlayer(GetRoundWinner()));
        }

        // Actualiza el tablero y las manos.
        boardManager.UpdateBoardAndHands();
    }
}