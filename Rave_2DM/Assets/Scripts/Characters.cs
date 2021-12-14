using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Characters
{
    public string name { get; protected set; }
    private int x, y; // coord
    private int HP; // cur HP
    private int MAXHP;
}

public class Robot : Characters
{
    public Robot(string name)
    {
        this.name = name;
    }
    public void GoHunt(Prisioner prisioner)
    {
        Debug.Log("Go to hunt for " + prisioner.name);
    }
    public void WaitUntilNeedHunt()
    {
        Debug.Log("I'm waiting for hunting");                                                                                                                                               
    }
}
public class Prisioner : Characters
{
    private int age;
    private int remainTimePrison;
    private int timePrison;
    private int datePrisioned;

    public void DoWhatYouWant()
    {
        Debug.Log($"{name} {age} is doing what he want");
    }
}

public class Animal : Characters
{
    public void WalkWhileAlive()
    {
        Debug.Log("I'm living here");
    }
}
