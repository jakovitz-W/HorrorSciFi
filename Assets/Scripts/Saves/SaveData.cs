using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    private GameObject player;
    private PlayerInteractions pInteractions;
    private PlayerMovement pMovement;
    private UpgradeSystem upgrades;
    private PlayerData _PlayerData = new PlayerData();


    private List<GameObject> humanObjects = new List<GameObject>();
    private List<HumanBehavior> humanBehaviors = new List<HumanBehavior>();
    private Humans _HumansData;


    private List<GameObject> monsterObjects = new List<GameObject>();
    private List<EnemyBehavior> monsterBehaviors = new List<EnemyBehavior>();
    private Monsters _MonstersData;


    private Levels _LevelData;


    public void SaveAll(){
        //get all world variables
    }

    private void SaveToJSON(){
        //set all json variables
    }

    public void LoadSaveData(){
        //get from json/set all world variables
    }

    public void ClearSaveData(){
        //reset all the variables
    }

}

[System.Serializable]
public class PlayerData{
    /*movement data*/
    public Vector2 worldPosition;
    public int currentRoom;
    public bool hasDroneKey, droneUnlocked;
    public float baseSpeed, runSpeed, regenPercent, regenStep;

    /*interaction data*/
    public bool hasTorch, hasTaser, torchActive, taserActive;
    public int keyNum;
    public float attackRadius;
    public float stunTime;

    /*Upgrade Data*/
    public int humansSaved, spent, wallet;
    public bool regenUnlocked;
}

[System.Serializable]
public class Humans{
    public List<HumanData> humans_data = new List<HumanData>();
}

[System.Serializable]
public class HumanData{

    public Vector2 worldPosition;
    public bool alive;
    public float health;
    public float currentSpeed;
    public bool isFollowing, isFrightened, idling, isClimbing, dropped, stopped;
    public List<GameObject> monsters;
    
}

public class Monsters{
    public List<MonsterData> monsters_data = new List<MonsterData>();
}

[System.Serializable]
public class MonsterData{
    public Vector2 worldPosition;
    public int direction;
}

[System.Serializable]
public class Levels{
    public List<LevelData> levels = new List<LevelData>();
}

[System.Serializable]
public class LevelData{
    public bool hasKey;
    public List<ElectricButton> buttons = new List<ElectricButton>();
    public List<Grate> grates = new List<Grate>();

}

[System.Serializable]
public class ElectricButton{
    public bool activated;
}

[System.Serializable]
public class Grate{
    public bool melted;
}

[System.Serializable]
public class PuzzleCrate{
    public Vector2 origin;
    public Vector2 worldPosition;
}



