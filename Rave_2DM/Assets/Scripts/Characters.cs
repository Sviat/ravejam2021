using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum PersonTypes
{
    Worker,
    Scientist,
    Other
}

enum CriminalArticles
{
    art1,
    art2,
    art3
}
public class Characters : MonoBehaviour
{
    private PersonTypes personType;
    private string personName;
    private int age;
    private int remainTimePrison;
    private int timePrison;
    private int datePrisioned;
    private CriminalArticles criminalArticle;

}
