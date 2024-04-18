using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{ // Define una clase llamada "Field"
    /// <summary>
    /// Struct que almacena los atributos de puntuación y condición de este campo.
    /// </summary>
    public struct FieldAtributes // Define una estructura llamada "FieldAtributes"
    {
        public int score; // Variable para almacenar la puntuación
        public int scoreBackup; // Variable para almacenar una copia de seguridad de la puntuación

        public ScoreEffects.effect condition; // Variable para almacenar la condición (efecto)

        public FieldAtributes(int sc, ScoreEffects.effect eff) // Constructor de la estructura FieldAtributes
        {
            score = sc; // Inicializa la puntuación con el valor proporcionado
            condition = eff; // Inicializa la condición con el valor proporcionado
            scoreBackup = -1; // Inicializa la copia de seguridad de la puntuación con -1
        }
    }

    private FieldEnum.fields line; // Variable para almacenar la línea (probablemente una posición en el tablero)
    private FieldAtributes fieldAtributes; // Variable para almacenar los atributos del campo
    private int _nCards; // Variable para almacenar el número de cartas

    /// <summary>
    /// Constructor principal de esta clase.
    /// </summary>
    public Field(FieldEnum.fields fName, int sc, ScoreEffects.effect eff) // Constructor de la clase Field
    {
        line = fName; // Inicializa la línea con el valor proporcionado
        fieldAtributes = new FieldAtributes(sc, eff); // Inicializa los atributos del campo con los valores proporcionados
    }

    /// <summary>
    /// Constructor de copia.
    /// </summary>
    public Field(Field inputField) // Constructor de copia de la clase Field
    {
        line = inputField.line; // Copia la línea del campo de entrada
        fieldAtributes = inputField.fieldAtributes; // Copia los atributos del campo de entrada
        _nCards = inputField._nCards; // Copia el número de cartas del campo de entrada
    }

    /// <summary>
    /// Añade el valor de la carta a la puntuación de este campo.
    /// </summary>
    public void AddScore(int score) // Método para añadir puntuación
    {
        ++_nCards; // Incrementa el número de cartas

        if (isAffected()) // Si el campo está afectado
        {
            AddScoreWhileAffected(score); // Añade la puntuación mientras el campo está afectado
        }
        else // Si el campo no está afectado
        {
            fieldAtributes.score += score; // Añade la puntuación al campo
        }
    }

    /// <summary>
    /// Establece la condición según el efecto de entrada.
    /// </summary>
    public void SetEffect(ScoreEffects.effect effect) // Método para establecer el efecto
    {
        fieldAtributes.condition = effect; // Establece la condición del campo según el efecto de entrada

        if (effect == ScoreEffects.effect.x2) // Si el efecto es "x2"
        {
            AffectScore(); // Afecta la puntuación duplicándola
        }
        else // Si el efecto no es "x2"
        {
            AffectScore(_nCards); // Afecta la puntuación con el número de cartas en esta línea
        }
    }

    /// <summary>
    /// Afecta la puntuación duplicando su valor.
    /// </summary>
    public void AffectScore() // Método para afectar la puntuación
    {
        fieldAtributes.scoreBackup = fieldAtributes.score; // Guarda la puntuación actual en la copia de seguridad
        fieldAtributes.score *= 2; // Duplica la puntuación
    }

    /// <summary>
    /// Afecta la puntuación con el número de cartas en esta línea. Se utiliza para el efecto climático.
    /// </summary>
    public void AffectScore(int nCardsP) // Método para afectar la puntuación con el número de cartas
    {
        fieldAtributes.scoreBackup = fieldAtributes.score; // Guarda la puntuación actual en la copia de seguridad
        fieldAtributes.score = nCardsP; // Establece la puntuación igual al número de cartas
    }

    /// <summary>
    /// Añade una puntuación mientras se está en una condición de efecto
    /// </summary>
    private void AddScoreWhileAffected(int score) // Método para añadir puntuación mientras se está en una condición de efecto
    {
        if (fieldAtributes.scoreBackup == -1) // Si la copia de seguridad de la puntuación es -1
        {
            fieldAtributes.scoreBackup = 0; // Establece la copia de seguridad de la puntuación a 0
        }
        fieldAtributes.scoreBackup += score; // Añade la puntuación a la copia de seguridad de la puntuación

        if (fieldAtributes.condition == ScoreEffects.effect.fog || fieldAtributes.condition == ScoreEffects.effect.rain || fieldAtributes.condition == ScoreEffects.effect.snow) // Si la condición es "fog", "rain" o "snow"
        {
            ++fieldAtributes.score; // Incrementa la puntuación
        }
        else // Si la condición no es "fog", "rain" ni "snow"
        {
            fieldAtributes.score = fieldAtributes.scoreBackup * 2; // Establece la puntuación igual al doble de la copia de seguridad de la puntuación
        }
    }/// <summary>
     /// Revierte los valores de la puntuación y su copia de seguridad
     /// </summary>
    public void RevertScore() // Método para revertir la puntuación
    {
        fieldAtributes.score = fieldAtributes.scoreBackup; // Restaura la puntuación a su valor de copia de seguridad
        fieldAtributes.scoreBackup = -1; // Restablece la copia de seguridad de la puntuación a -1
    }

    /// <summary>
    /// Devuelve la línea de este campo.
    /// </summary>
    public FieldEnum.fields GetLine() // Método para obtener la línea
    {
        return line; // Devuelve la línea
    }

    /// <summary>
    /// Devuelve la instancia de la estructura de atributos.
    /// </summary>
    public FieldAtributes GetAtributes() // Método para obtener los atributos
    {
        return fieldAtributes; // Devuelve los atributos del campo
    }

    /// <summary>
    /// Devuelve la condición de este campo.
    /// </summary>
    public bool isAffected() // Método para comprobar si el campo está afectado
    {
        return fieldAtributes.condition != ScoreEffects.effect.no; // Devuelve verdadero si la condición del campo no es "no"
    }

    /// <summary>
    /// Restablece los atributos a su valor inicial.
    /// </summary>
    public void DefaultAtributes() // Método para restablecer los atributos
    {
        fieldAtributes.score = 0; // Restablece la puntuación a 0
        _nCards = 0; // Restablece el número de cartas a 0

        if (isAffected()) // Si el campo está afectado
        {
            fieldAtributes.scoreBackup = -1; // Restablece la copia de seguridad de la puntuación a -1
            fieldAtributes.condition = ScoreEffects.effect.no; // Restablece la condición a "no"
        }
    }
}