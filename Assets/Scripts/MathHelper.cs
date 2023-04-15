using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public static class MathHelper
{
    public static float GreatestCommonFactor(float a, float b)
    {
        while (b != 0)
        {
            float temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static float LeastCommonMultiple(float a, float b)
    {
        return (a / GreatestCommonFactor(a, b)) * b;
    }

    public static string SimplifyFractions(float n, float d)
    {
        if (n == 0) return "0";

        float auxn = n, auxd = d;
        float aux = GreatestCommonFactor(n, d);
        if (aux != 1) //they have multiples
        {
            auxn /= aux;
            auxd /= aux;

            if (auxd < 0)
            {
                auxn *= -1;
                auxd *= -1;
            }
        }

        if (auxd == 1)
        {
            return auxn.ToString();
        }
        else
        {
            return auxn.ToString() + " / " + auxd.ToString();
        }
    }

    public static float[] GenerateWrongAnswers(float correctAnswer)
    {
        float[] wrongAnswers = new float[3];
        HashSet<float> uniqueWrongAnswers = new HashSet<float>();

        int maxIterations = 1000;
        int iterations = 0;

        int limitInf = correctAnswer >= 10 ? -10 : (int)correctAnswer % 10;

        while (uniqueWrongAnswers.Count < 3 && iterations < maxIterations)
        {
            int randomValue = Random.Range(limitInf, limitInf + 20);

            float wrongAnswer = correctAnswer + randomValue;

            if (!uniqueWrongAnswers.Contains(wrongAnswer) &&
                wrongAnswer != correctAnswer)
            {
                uniqueWrongAnswers.Add(wrongAnswer);
            }

            iterations++;
        }

        if (uniqueWrongAnswers.Count < 3)
        {
            Debug.LogWarning("No se pudieron generar suficientes respuestas incorrectas únicas en " + maxIterations + " iteraciones.");
        }

        uniqueWrongAnswers.CopyTo(wrongAnswers);

        return wrongAnswers;
    }

    public static void CreateQuestion(string question)
    {
        //Initialize variables
        float xn, xd, yn, yd, zn, zd;
        int u, uE, min, max, numDec;
        int[] validChoices;
        string ca, wa1, wa2, wa3, q0, u0, u1;
        xn = xd = yn = yd = zn = zd = 1;
        ca = wa1 = wa2 = wa3 = q0 = u0 = u1 = "";
        u = uE = 0;
        numDec = 2;

        //Configurations
        //COMPETENCE 1 =======================================================================
        switch (GameSystemScript.CurrentLevelSO.currentZone)
        {
            //COMPETENCE 1 =======================================================================
            case 0:
                //L2.2
                numDec = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[3].default_value;

                //L5
                if (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[4].selected == true)
                {
                    u = Random.Range(0, 2);
                    uE = Random.Range(0, 2);
                    if (u == 0)
                    {
                        u0 = "kg";
                        u1 = "g";
                    }
                    else
                    {
                        u0 = "m";
                        u1 = "cm";
                    }
                }
                else
                {
                    u0 = "";
                    u1 = "";
                }

                break;

            //COMPETENCE 2 =======================================================================
            case 1:
                break;

            //COMPETENCE 3 =======================================================================
            case 2:
                break;

            //COMPETENCE 4 =======================================================================
            case 3:
                break;

            default:
                break;
        }

        //Compendium of all the possible conversations that an enemy can have.
        switch (question)
        {
            //COMPETENCE 1 =======================================================================
            //L1----------------------------------------------------------------------------------
            case "Naturales Suma":
                //Configurations
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                if (uE == 0) u1 = u0; //Same units

                xn = Random.Range(min, max);//kg or m
                if (uE == 0) yn = Random.Range(min, max);
                else
                {
                    xn = xn / 100f;
                }

                zn = xn + yn;

                ca = zn.ToString() + u1;
                break;

            case "Naturales Resta":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                //This way, yn will never be xn
                validChoices = new int[] { Random.Range(min, (int)xn), Random.Range((int)xn + 1, (int)max) };
                yn = validChoices[Random.Range(0, 1)];

                zn = xn - yn;

                ca = zn.ToString();
                break;

            case "Naturales Multiplicacion":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(1, min);

                zn = xn * yn;

                ca = zn.ToString();
                break;

            case "Naturales Division":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(1, min);

                zn = xn / yn;

                ca = System.Math.Round(zn, 3).ToString().Replace(",", ".");
                break;

            case "Naturales Potencia":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[3].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[4].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(2, 4);// ^2 or ^3

                zn = (int)Mathf.Pow(xn, yn);

                ca = zn.ToString();
                break;

            //L2----------------------------------------------------------------------------------
            case "Fracciones Suma":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = MathHelper.LeastCommonMultiple(xd, yd);
                zn = xn * (zd / xd) + yn * (zd / yd);

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            case "Fracciones Resta":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = MathHelper.LeastCommonMultiple(xd, yd);
                zn = xn * (zd / xd) - yn * (zd / yd);

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            case "Fracciones Multiplicacion":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = xd * yd;
                zn = xn * yn;

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            case "Fracciones Division":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = xd / yd;
                zn = xn / yn;

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            case "Decimales Suma":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;
                if (uE == 0) u0 = u1; //Same units
                else u1 = u0;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(min, max);
                xn = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);
                yn = yn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);

                zn = xn + yn;
                xn = (float)System.Math.Round((xn), numDec);
                yn = (float)System.Math.Round((yn), numDec);

                ca = System.Math.Round(zn, numDec).ToString().Replace(",", ".") + u0;
                break;

            case "Decimales Resta":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(min, max);
                xn = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);
                yn = yn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);

                if ((xn - yn) < 0)
                {
                    float auxF = xn;
                    xn = yn;
                    yn = auxF;
                }

                zn = xn - yn;
                xn = (float)System.Math.Round((xn), numDec);
                yn = (float)System.Math.Round((yn), numDec);

                ca = System.Math.Round(zn, numDec).ToString().Replace(",", ".") + u0;
                break;

            case "Decimales Multiplicacion":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(2, 11);
                xn = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);

                zn = xn * yn;
                xn = (float)System.Math.Round((xn), numDec);
                yn = (float)System.Math.Round((yn), numDec);

                ca = System.Math.Round(zn, numDec).ToString().Replace(",", ".") + u0;
                break;

            case "Decimales Division":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(2, 11);
                xn = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);

                zn = xn / yn;
                xn = (float)System.Math.Round((xn), numDec);
                yn = (float)System.Math.Round((yn), numDec);

                ca = System.Math.Round(zn, numDec).ToString().Replace(",", ".") + u0;
                break;

            //COMPETENCE 2 =======================================================================
            //L8----------------------------------------------------------------------------------
            case "Ecuaciones Simples 1":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[2].default_value;

                // xn * x + yn = 0 / xn * x + yn = xd * x + yd
                // xd != xn
                xn = Random.Range(min, max);
                validChoices = new int[] { Random.Range(-min, 0), Random.Range(1, (int)xn), Random.Range((int)xn, min) };
                xd = validChoices[Random.Range(0, 2)];

                yd = Random.Range(yn + 1, max);

                zn = (yd - yn);
                zd = (xn - xd);

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            case "Ecuaciones Simples 2":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[2].default_value;

                // xn * x + yn = 0 / xn * x + yn = xd * x + yd
                // xd != xn
                xn = Random.Range(min, max);
                validChoices = new int[] { Random.Range(-min, 0), Random.Range(1, (int)xn), Random.Range((int)xn, min) };
                xd = validChoices[Random.Range(0, 2)];

                yd = Random.Range(yn + 1, max);

                zn = (-yd - yn);
                zd = (xn - xd);

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            //L9----------------------------------------------------------------------------------
            case "Sucesiones":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(1, min);
                int aux = Random.Range(min, max);

                if (aux % 2 == 0)
                {
                    yn = 2 * xn + aux;
                    xd = 3 * xn + aux;
                    yd = 4 * xn + aux;
                    zn = 5 * xn + aux;
                    xn = 1 * xn + aux;
                }
                else
                {
                    yn = xn + aux;
                    xd = yn + xn;
                    yd = xd + yn;
                    zn = yd + xd;
                }

                ca = zn.ToString();
                break;

            //COMPETENCE 3 =======================================================================
            //L13---------------------------------------------------------------------------------
            case "Area Triangulo":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                zn = xn * yn / 2f;

                ca = System.Math.Round(zn, 3).ToString().Replace(",", ".");
                break;

            case "Perimetro Triangulo":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);
                xd = Random.Range(min, max);

                zn = xn + yn + xd;

                ca = zn.ToString();
                break;

            case "Area Rectangulo":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                zn = xn * yn;

                ca = zn.ToString();
                break;

            case "Perimetro Rectangulo":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                zn = 2 * (xn + yn);

                ca = zn.ToString();
                break;

            case "Volumen Paralelepipedo":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);
                xd = Random.Range(min, max);

                zn = xn * yn * xd;

                ca = zn.ToString();
                break;

            case "Planos":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(1, min);

                zn = xn / yn;

                ca = System.Math.Round(zn, 3).ToString().Replace(",", ".");
                break;

            //COMPETENCE 4 =======================================================================
            //L21---------------------------------------------------------------------------------
            case "Media Aritmetica":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yd = Random.Range(min, max);

                zn = (xn + yn + xd + yd) / 4;
                zn = (zn < 0 ? 2 : zn);

                ca = System.Math.Round(zn, 3).ToString().Replace(",", ".");
                break;

            case "Moda":
                xn = Random.Range(1, 10);
                yn = Random.Range(10, 20);
                xd = Random.Range(20, 30);
                yd = Random.Range(30, 40);

                float[] vals = { xn, yn, xd, yd };
                float temp;

                //Shuffle frecuency
                for (int i = 0; i < vals.Length; i++)
                {
                    int rnd = Random.Range(0, vals.Length);
                    temp = vals[rnd];
                    vals[rnd] = vals[i];
                    vals[i] = temp;
                }

                xn = Random.Range(6, 10);
                yn = Random.Range(3, 5);
                xd = Random.Range(5, 6);
                yd = Random.Range(1, 3);

                float[] frecuency = { xn, yn, xd, yd };

                List<float> pob = new List<float> { };

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < frecuency[i]; j++)
                    {
                        pob.Add(vals[i]);
                    }
                }

                ShuffleListScript.Shuffle(pob);
                for (int i = 0; i < pob.Count; i++)
                {
                    if (i == 0) q0 += pob[i].ToString();
                    else q0 += ", " + pob[i].ToString();
                }

                ca = vals[0].ToString();
                wa1 = vals[1].ToString();
                wa2 = vals[2].ToString();
                wa3 = vals[3].ToString();
                break;

            case "Probabilidad":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                zn = xn / yn;

                ca = System.Math.Round(zn, 3).ToString().Replace(",", ".");
                break;

            default:
                Debug.Log("No se pudo asignar variables a la conversación " + question);
                break;
        }

        //Set question string auxiliar
        DialogueLua.SetVariable("Q0", q0);

        DialogueLua.SetVariable("Xn", xn); //Set numerator
        DialogueLua.SetVariable("Yn", yn); //Set numerator
        DialogueLua.SetVariable("Xd", xd); //Set deno(int) minator
        DialogueLua.SetVariable("Yd", yd); //Set deno(int) minator

        //Set some units in questions
        DialogueLua.SetVariable("U0", u0);
        DialogueLua.SetVariable("U1", u1);

        //Set the correct answer
        DialogueLua.SetVariable("Ca", ca);

        //Set the wrong answers from Zn and Zd values
        //MathHelper.GenerateWrongAnswers(questionData.name, out wa1, out wa2, out wa3, zn, zd);
        float[] wrongAnswers = MathHelper.GenerateWrongAnswers(zn);
        string denom = "";

        if (question.StartsWith("Fracciones")) denom = "/" + zd.ToString();
        wa1 = System.Math.Round(wrongAnswers[0], 3).ToString().Replace(",", ".") + denom;
        wa2 = System.Math.Round(wrongAnswers[1], 3).ToString().Replace(",", ".") + denom;
        wa3 = System.Math.Round(wrongAnswers[2], 3).ToString().Replace(",", ".") + denom;

        DialogueLua.SetVariable("Wa1", wa1);
        DialogueLua.SetVariable("Wa2", wa2);
        DialogueLua.SetVariable("Wa3", wa3);

        //Check if the question is more than one
        DialogueLua.SetVariable("StartQuestion", 1); //DEPENDS
    }

    public static Vector3 RandomCircle(Vector3 center, float radius, int a)
    {
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }
}