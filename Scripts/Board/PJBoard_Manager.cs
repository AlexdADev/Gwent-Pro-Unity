using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PJBoard_Manager : MonoBehaviour // Define una clase llamada "PJBoard_Manager" que hereda de "MonoBehaviour"
{
    [SerializeField] Board_Manager boardInstance; // Referencia a una instancia de "Board_Manager"

    public GameObject fieldsParent; // Objeto padre de los campos

    Field[] fields; // Array para almacenar los campos

    List<int> unaffectedRows; // Lista para almacenar las filas no afectadas
    #region effect instances
    const ScoreEffects.effect noEffect = ScoreEffects.effect.no; // Constante para el efecto "no"
    const ScoreEffects.effect fog = ScoreEffects.effect.fog; // Constante para el efecto "fog"
    const ScoreEffects.effect rain = ScoreEffects.effect.rain; // Constante para el efecto "rain"
    const ScoreEffects.effect snow = ScoreEffects.effect.snow; // Constante para el efecto "snow"
    const ScoreEffects.effect x2 = ScoreEffects.effect.x2; // Constante para el efecto "x2"
    const ScoreEffects.effect sun = ScoreEffects.effect.sun; // Constante para el efecto "sun"
    #endregion

    #region field instances
    FieldEnum.fields selectedRowCH = FieldEnum.fields.undefined; // Campo seleccionado en la selección del cuerno del comandante
    const FieldEnum.fields blade = FieldEnum.fields.CloseCombat; // Constante para el campo "CloseCombat"
    const FieldEnum.fields bow = FieldEnum.fields.RangedCombat; // Constante para el campo "RangedCombat"
    const FieldEnum.fields catapult = FieldEnum.fields.SiegeCombat; // Constante para el campo "SiegeCombat"
    #endregion

    /// <summary>
    /// Creación de los campos de este jugador e inicialización.
    /// </summary>
    void Awake() // Método que se llama cuando se crea una instancia de la clase
    {
        unaffectedRows = new List<int>(); // Inicializa la lista de filas no afectadas
        fields = new Field[] // Inicializa el array de campos
        {
            new Field(blade, 0, noEffect), // Crea un nuevo campo "blade" con puntuación 0 y sin efecto
            new Field(bow, 0, noEffect), // Crea un nuevo campo "bow" con puntuación 0 y sin efecto
            new Field(catapult, 0, noEffect), // Crea un nuevo campo "catapult" con puntuación 0 y sin efecto
        };
        UpdateScore(); // Actualiza la puntuación
    }

    /// <summary>
    /// Añade una carta a este tablero de jugador.
    /// </summary>
    public void AddCard(GameObject card) // Método para añadir una carta
    {
        switch (card.GetComponent<Card>().GetEffect()) // Dependiendo del efecto de la carta
        {
            case noEffect: // Si el efecto es "noEffect"
                card.transform.SetParent(fieldsParent.transform.GetChild((int)card.GetComponent<Card>().GetField())); // Establece el padre de la carta al campo correspondiente en el padre de los campos
                fields[(int)card.GetComponent<Card>().GetField()].AddScore(card.GetComponent<Card>().GetValue()); // Añade la puntuación de la carta al campo correspondiente
                break;
            case rain: // Si el efecto es "rain"
            case fog: // Si el efecto es "fog"
            case snow: // Si el efecto es "snow"
            case sun: // Si el efecto es "sun"
                boardInstance.ApplyGlobalEffect(this, card.GetComponent<Card>().GetEffect(), card); // Aplica el efecto global a este tablero de jugador
                ApplyEffect(card.GetComponent<Card>().GetEffect()); // Aplica el efecto
                break;
            case x2: // Si el efecto es "x2"
                ApplyEffect(card.GetComponent<Card>().GetField(), fields); // Aplica el efecto al campo correspondiente (es un cuerno de comandante)
                Destroy(card); // Destruye la carta
                break;
        }
        UpdateScore(); // Actualiza la puntuación
        boardInstance.UpdateBoardAndHands(); // Actualiza el tablero y las manos
    }

    /// <summary>
    /// Añade una carta a los campos de entrada de forma virtual.
    /// </summary>
    public void AddCard(GameObject card, Field[] inputFields, Field[] inputFields2) // Método para añadir una carta
    {
        switch (card.GetComponent<Card>().GetEffect()) // Dependiendo del efecto de la carta
        {
            case noEffect: // Si el efecto es "noEffect"
                inputFields[(int)card.GetComponent<Card>().GetField()].AddScore(card.GetComponent<Card>().GetValue()); // Añade la puntuación de la carta al campo correspondiente en los campos de entrada
                break;
            case rain: // Si el efecto es "rain"
            case fog: // Si el efecto es "fog"
            case snow: // Si el efecto es "snow"
            case sun: // Si el efecto es "sun"
                ApplyEffect(card.GetComponent<Card>().GetEffect(), inputFields); // Aplica el efecto a los campos de entrada
                ApplyEffect(card.GetComponent<Card>().GetEffect(), inputFields2); // Aplica el efecto a los campos de entrada 2
                break;
            case x2: // Si el efecto es "x2"
                Debug.Log("wtf"); // Imprime un mensaje de depuración
                ApplyEffect(card.GetComponent<Card>().GetField(), inputFields); // Aplica el efecto al campo correspondiente en los campos de entrada (es un cuerno de comandante)
                break;
        }
    }

    /// <summary>
    /// Devuelve cada campo a sus valores por defecto (por ejemplo, se utiliza en los cambios de ronda).
    /// </summary>
    public void ClearBoard() // Método para limpiar el tablero
    {
        for (int i = 0; i < fields.Length; i++) // Para cada campo en los campos
        {
            fields[i].DefaultAtributes(); // Restablece los atributos del campo a sus valores por defecto
            foreach (Transform child in fieldsParent.transform.GetChild(i)) // Para cada hijo del campo en el padre de los campos
            {
                Destroy(child.gameObject); // Destruye el objeto hijo
            }
        }
    }


    /// <summary>
    /// Devuelve una lista de filas (campos) no afectadas en los campos de entrada.
    /// </summary>
    public List<int> GetUnaffectedRows(Field[] inputFields) // Método para obtener las filas no afectadas de los campos de entrada
    {
        unaffectedRows.Clear(); // Limpia la lista de filas no afectadas

        foreach (Field item in inputFields) // Para cada campo en los campos de entrada
        {
            if (item.GetAtributes().condition == noEffect) // Si la condición del campo es "noEffect"
            {
                unaffectedRows.Add((int)item.GetLine()); // Añade la línea del campo a la lista de filas no afectadas
            }
        }
        return unaffectedRows; // Devuelve la lista de filas no afectadas
    }

    /// <summary>
    /// Devuelve una lista de filas (campos) no afectadas en este tablero de jugador.
    /// </summary>
    public List<int> GetUnaffectedRows() // Método para obtener las filas no afectadas
    {
        unaffectedRows.Clear(); // Limpia la lista de filas no afectadas

        foreach (Field item in fields) // Para cada campo en los campos
        {
            if (item.GetAtributes().condition == noEffect) // Si la condición del campo es "noEffect"
            {
                unaffectedRows.Add((int)item.GetLine()); // Añade la línea del campo a la lista de filas no afectadas
            }
        }
        return unaffectedRows; // Devuelve la lista de filas no afectadas
    }

    /// <summary>
    /// Devuelve una copia de los campos de este tablero de jugador.
    /// </summary>
    public Field[] GetAllBoardPlayerInfo() // Método para obtener toda la información del tablero del jugador
    {
        Field[] temp = new Field[fields.Length]; // Crea un array temporal para almacenar los campos

        fields.CopyTo(temp, 0); // Copia los campos al array temporal
        return temp; // Devuelve el array temporal
    }

    public FieldEnum.fields GetSelectedRow() // Método para obtener la fila seleccionada
    {
        return selectedRowCH; // Devuelve la fila seleccionada
    }

    /// <summary>
    /// Convierte el valor entero de entrada a un elemento de FieldEnum y establece la fila seleccionada (campo) a él. Método utilizado por los botones de las líneas en la escena.
    /// </summary>
    public void SetSelectedRow(int row) // Método para establecer la fila seleccionada
    {
        selectedRowCH = (FieldEnum.fields)row; // Establece la fila seleccionada al valor de entrada convertido a FieldEnum
    }

    #region Score Management
    [SerializeField] private GameObject scoreTextParent; // Referencia al objeto padre del texto de puntuación
    int totalScore; // Variable para almacenar la puntuación total

    /// <summary>
    /// Actualiza la puntuación total y los textos.
    /// </summary>
    void UpdateScore() // Método para actualizar la puntuación
    {
        UpdateTotalScore(); // Actualiza la puntuación total
        UpdateText(); // Actualiza el texto de la puntuación
    }

    /// <summary>
    /// Actualiza los textos de puntuación.
    /// </summary>
    void UpdateText() // Método para actualizar el texto de la puntuación
    {
        for (int i = 0; i < fields.Length; i++) // Para cada campo en los campos
        {
            scoreTextParent.transform.GetChild(i).GetComponent<Text>().text = fields[i].GetAtributes().score.ToString(); // Actualiza el texto de la puntuación del campo
        }
    }

    /// <summary>
    /// Actualiza la puntuación total de este campo.
    /// </summary>
    void UpdateTotalScore() // Método para actualizar la puntuación total
    {
        totalScore = 0; // Restablece la puntuación total a 0

        for (int i = 0; i < fields.Length; i++) // Para cada campo en los campos
        {
            totalScore += fields[i].GetAtributes().score; // Suma la puntuación del campo a la puntuación total
        }
    }

    /// <summary>
    /// Devuelve la suma de las puntuaciones de los campos de entrada.
    /// </summary>
    public int GetTotalScore(Field[] inputFields) // Método para obtener la puntuación total de los campos de entrada
    {
        int totalScoreTemp = 0; // Variable para almacenar la puntuación total temporal

        for (int i = 0; i < inputFields.Length; i++) // Para cada campo en los campos de entrada
        {
            totalScore += inputFields[i].GetAtributes().score; // Suma la puntuación del campo a la puntuación total temporal
        }

        return totalScoreTemp; // Devuelve la puntuación total temporal
    }


    /// <summary>
    /// Devuelve la suma de todas las puntuaciones de los campos (este jugador).
    /// </summary>
    public int GetTotalScore() // Método para obtener la puntuación total
    {
        return totalScore; // Devuelve la puntuación total
    }


    /// <summary>
    /// Devuelve la puntuación de una fila con un elemento de enumeración de campo específico.
    /// </summary>
    public int[] GetRowsScore(Field[] inputFields) // Método para obtener las puntuaciones de las filas de los campos de entrada
    {
        int[] temp = new int[inputFields.Length]; // Array para almacenar las puntuaciones de las filas

        for (int i = 0; i < inputFields.Length; i++) // Para cada campo en los campos de entrada
        {
            temp[i] = inputFields[i].GetAtributes().score; // Almacena la puntuación del campo en el array
        }

        return temp; // Devuelve el array de puntuaciones de las filas
    }

    #endregion

    #region Effect Management
    // Método para aplicar un efecto a los campos
    public void ApplyEffect(ScoreEffects.effect effect)
    {
        switch (effect) // Dependiendo del efecto
        {
            case rain: // Si el efecto es "rain"
                fields[(int)catapult].SetEffect(effect); // Aplica el efecto al campo "catapult"
                break;
            case fog: // Si el efecto es "fog"
                fields[(int)bow].SetEffect(effect); // Aplica el efecto al campo "bow"
                break;
            case snow: // Si el efecto es "snow"
                fields[(int)blade].SetEffect(effect); // Aplica el efecto al campo "blade"
                break;
            case sun: // Si el efecto es "sun"
                DefaultRowEffects(); // Restablece los efectos de las filas a sus valores por defecto
                break;
        }
        UpdateScore(); // Actualiza la puntuación
    }

    // Método para aplicar un efecto a los campos de entrada
    public Field[] ApplyEffect(ScoreEffects.effect effect, Field[] inputFields)
    {
        switch (effect) // Dependiendo del efecto
        {
            case rain: // Si el efecto es "rain"
                inputFields[(int)catapult].SetEffect(effect); // Aplica el efecto al campo "catapult" de los campos de entrada
                break;
            case fog: // Si el efecto es "fog"
                inputFields[(int)bow].SetEffect(effect); // Aplica el efecto al campo "bow" de los campos de entrada
                break;
            case snow: // Si el efecto es "snow"
                inputFields[(int)blade].SetEffect(effect); // Aplica el efecto al campo "blade" de los campos de entrada
                break;
            case sun: // Si el efecto es "sun"
                DefaultRowEffects(inputFields); // Restablece los efectos de las filas de entrada a sus valores por defecto
                break;
        }
        return inputFields; // Devuelve los campos de entrada
    }

    // Método para aplicar el efecto "x2" a un campo específico de los campos de entrada
    void ApplyEffect(FieldEnum.fields field, Field[] inputFields)
    {
        inputFields[(int)field].SetEffect(x2); // Aplica el efecto "x2" al campo especificado de los campos de entrada
    }

    // Método para restablecer los efectos de las filas de entrada a sus valores por defecto
    void DefaultRowEffects(Field[] inputFields)
    {
        for (int i = 0; i < inputFields.Length; i++) // Para cada campo en los campos de entrada
        {
            if (inputFields[i].GetAtributes().condition != noEffect && inputFields[i].GetAtributes().condition != x2) // Si la condición del campo no es "noEffect" ni "x2"
            {
                inputFields[i].RevertScore(); // Revierte la puntuación del campo a su valor de copia de seguridad
            }
        }
    }

    // Método para restablecer los efectos de las filas a sus valores por defecto
    void DefaultRowEffects()
    {
        for (int i = 0; i < fields.Length; i++) // Para cada campo en los campos
        {
            if (fields[i].GetAtributes().condition != noEffect && fields[i].GetAtributes().condition != x2) // Si la condición del campo no es "noEffect" ni "x2"
            {
                fields[i].RevertScore(); // Revierte la puntuación del campo a su valor de copia de seguridad
            }
        }
    }
}
#endregion
