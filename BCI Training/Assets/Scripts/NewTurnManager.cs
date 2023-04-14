using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTurnManager : MonoBehaviour 
{
    static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>(); //all units in the game
    static Queue<string> turn = new Queue<string>(); //keep track of whose turn it is
    static Queue<TacticsMove> turnTeam = new Queue<TacticsMove>(); //queue for which team turn it is

	// Update is called once per frame
	void Update () 
	{
        if (turnTeam.Count == 0)
        {
            InitializeTeamTurnQueue();
        }
	}

    static void InitializeTeamTurnQueue()
    {
        List<TacticsMove> teamList = units[turn.Peek()]; //looking at which team it is 

        foreach (TacticsMove unit in teamList)
        {
            turnTeam.Enqueue(unit);
        }

        StartTurn();
    }

    public static void StartTurn()
    {
        if (turnTeam.Count > 0) // if the teams is empty we dont do this
        {
            turnTeam.Peek().TurnBegin();
        }
    }

    public static void EndTurn() //when turn is over they call the endturn funciton themselves
    {
        TacticsMove unit = turnTeam.Dequeue(); //remove the item from the queue
        unit.TurnEnd();

        if (turnTeam.Count > 0) //is there anyone else on the team that needs to go?
        {
            StartTurn();
        }
        else
        {
            string team = turn.Dequeue();
            turn.Enqueue(team); //Add the team to the end of the queue again, so it can be their turn again
            InitializeTeamTurnQueue();
        }
    }

    public static void AddUnit(TacticsMove unit) //add a unit to the dictionary
    { //units add themselves. Call and add to appropriate team
        List<TacticsMove> list;

        if (!units.ContainsKey(unit.tag)) //make sure the tag has been added to the dictionary
        {
            list = new List<TacticsMove>(); //if not create it and add it to the list
            units[unit.tag] = list;

            if (!turn.Contains(unit.tag))
            {
                turn.Enqueue(unit.tag);
            }
        }
        else
        {
            list = units[unit.tag];
        }

        list.Add(unit);
    }

    //todo: Remove a unit from the team - and if it is last member of the team remove it from the turn(key) as well
}
