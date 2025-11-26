using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RaceManager : MonoBehaviour
{
    public CarProgress player;
    public CarProgress[] aiCars;

    public int playerPosition;

    private void Update()
    {
        List<CarProgress> allCars = new List<CarProgress>();
        allCars.Add(player);
        allCars.AddRange(aiCars);

        allCars = allCars.OrderByDescending(c => c.totalProgress).ToList();

        playerPosition = allCars.IndexOf(player) + 1;
    }
}
