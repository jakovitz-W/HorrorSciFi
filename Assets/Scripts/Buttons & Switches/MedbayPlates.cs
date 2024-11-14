using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedbayPlates : PlateAbs
{
    public int id;

    //for mulitple plates in 1 room do an id based system
    public override void plateFunction(){
        Debug.Log("plate :)");
    } 
}
