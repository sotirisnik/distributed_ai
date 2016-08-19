using UnityEngine;
using System.Collections;

public class Village : MonoBehaviour {

    private int[] cur_quantity;
    private int[] max_capacity;

    int x, y;

    public Village() {

        cur_quantity = new int[GameLogic.goods_names.Length];
        max_capacity = new int[GameLogic.goods_names.Length];

        x = y = 0;

        for ( int i = 0; i < GameLogic.goods_names.Length; ++i ) {
            cur_quantity[i] = max_capacity[i] = 0;
        }

    }

    public void setX( int x ) {
        this.x = x;
    }

    public int getX() {
        return (x);
    }

    public void setY( int y ) {
        this.y = y;
    }

    public int getY() {
        return (y);
    }

    public void Reset( ) {
        for ( int i = 0; i < cur_quantity.Length; ++i ) {
            cur_quantity[i] = max_capacity[i] = 0;
        }
    }

    public int getQuantity( int idx ) {
        return cur_quantity[idx];
    }

    public int getCapacity(int idx ) {
        return max_capacity[idx];
    }

    public void AddQuantity( int idx ) {
        ++this.cur_quantity[idx];
    }

    public void setCapacity( int idx, int val ) {
        this.max_capacity[idx] = val;
    }

}
