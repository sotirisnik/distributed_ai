using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arrows : MonoBehaviour {

    private int cell_x;//timi ston 2D pinaka
    private int cell_y;
    
    private int dir;
    
    public GameObject avatar;

    public Arrows() {
        this.avatar = (GameObject)Resources.Load("arrows_");
        cell_x = cell_y = 0;
        dir = -1;
        this.SetAvatar((GameObject)Instantiate(this.avatar, new Vector3(this.GetCellY() * GameLogic.tileSize, this.GetCellX() * GameLogic.tileSize, -GameLogic.ARROWS_LAYER), Quaternion.identity));
    }

    public Arrows( int cur_x, int cur_y ) {
        this.avatar = (GameObject)Resources.Load("arrows_");
        this.cell_x = cur_x;
        this.cell_y = cur_y;
        dir = -1;
        this.SetAvatar((GameObject)Instantiate( this.avatar, new Vector3(this.GetCellY() * GameLogic.tileSize, this.GetCellX() * GameLogic.tileSize, -GameLogic.ARROWS_LAYER), Quaternion.identity));
    }

    public void SetAvatar( GameObject avatar ) {
        this.avatar = avatar;
    }

    public int GetCellX( ) {
        return (this.cell_x);
    }

    public int GetCellY() {
        return (this.cell_y);
    }

    public GameObject GetAvatar( ) {
        return (this.avatar);
    }
    
    public void SetNextDir( int dir ) {
        //0 down
        //1 up
        //2 west
        //3 east
        
        this.dir = dir;

        this.avatar.GetComponent<Animator>().SetInteger("state", dir);
        
    }
    
}
