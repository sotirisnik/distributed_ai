using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Agent : MonoBehaviour {

    private int cell_x;//timi ston 2D pinaka
    private int cell_y;
    private float cur_x;//float timi pou vrisketai tora
    private float cur_y;
    private float des_x;//float timi ekei pou prepei na paei
    private float des_y;

    private int dir;
    private int units;
    
    public bool HaveGood = false;

    public long steps;

    //public List<AgentRed.Pair>[] goods_coord;

    public bool []goods_coord;

    public int good_of_interest;
    public Stack<int> bfs_path;

    public List<state> bfs_path_copy;

    private GameObject avatar;
    private GameObject possession;

    public state[,] fix_par;
    public bool[,] fix_visited;

    public bool[,] visited;
    public state[,] par;
    
    public bool canDoLogic;

    public Agent() {
        this.avatar = (GameObject)Resources.Load("agent_red");
        this.possession = new GameObject();
        this.possession.AddComponent<SpriteRenderer>();
        bfs_path_copy = new List<state>();
        cell_x = cell_y = 0;
        cur_x = cur_y = des_x = des_y = 0;
        dir = -1;
        visited = new bool[GameLogic.HEIGHT, GameLogic.WIDTH];
        par = new state[GameLogic.HEIGHT, GameLogic.WIDTH];
        fix_visited = new bool[GameLogic.HEIGHT, GameLogic.WIDTH];
        fix_par = new state[GameLogic.HEIGHT, GameLogic.WIDTH];
        for ( int i = 0; i < GameLogic.HEIGHT; ++i) {
            for ( int j = 0; j < GameLogic.WIDTH; ++j ) {
                par[i, j] = new state(-1,-1);
                visited[i, j] = false;
            }
        }
    }

    public Agent( int cur_x, int cur_y ) {
        this.avatar = (GameObject)Resources.Load("agent_red");
        this.possession = new GameObject();
        this.possession.AddComponent<SpriteRenderer>();
        bfs_path_copy = new List<state>();
        this.cell_x = cur_x;
        this.cell_y = cur_y;
        this.cur_x = cur_x * GameLogic.tileSize;
        this.cur_y = cur_y * GameLogic.tileSize;
        this.des_x = cur_x * GameLogic.tileSize;
        this.des_y = cur_y * GameLogic.tileSize;
        dir = -1;
        visited = new bool[GameLogic.HEIGHT, GameLogic.WIDTH];
        par = new state[GameLogic.HEIGHT, GameLogic.WIDTH];
        fix_visited = new bool[GameLogic.HEIGHT, GameLogic.WIDTH];
        fix_par = new state[GameLogic.HEIGHT, GameLogic.WIDTH];
        for ( int i = 0; i < GameLogic.HEIGHT; ++i) {
            for ( int j = 0; j < GameLogic.WIDTH; ++j ) {
                par[i, j] = new state(-1,-1);
                visited[i, j] = false;
            }
        }
    }

    public void SetAvatar( GameObject avatar ) {
        this.avatar = avatar;
    }
    
    public void SetPossesion(GameObject possession) {
        this.possession = possession;
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

    public GameObject GetPossession( ) {
        return (this.possession);
    }

    public bool isMoving( ) {
        return (dir != -1);
    }

    public void SetNextDir( int dir, int units ) {
        //0 down
        //1 up
        //2 west
        //3 east

        this.units = units;
        
        if ( this.dir == -1 ) {

            this.dir = dir;

            this.avatar.GetComponent<Animator>().SetInteger("state", dir);
            
            if ( dir == 0 ) {
                des_x = (cell_x - units) * GameLogic.tileSize;
            }else if ( dir == 1 ) {
                des_x = (cell_x + units) * GameLogic.tileSize;
            }else if ( dir == 2 ) {
                des_y = (cell_y - units) * GameLogic.tileSize;
            }else if ( dir == 3 ) {
                des_y = (cell_y + units) * GameLogic.tileSize;
            }

            //Debug.Log(des_x + " " + des_y + " " + cell_x + " " + cell_y + " " + cur_x + " " + " " + cur_y);

        }
        

    }

    public bool canMove( int next_x, int next_y ) {
        return ( next_x >= 0 && next_y >= 0 && next_x < GameLogic.HEIGHT && next_y < GameLogic.WIDTH );
    }

    public bool findGood(int next_x, int next_y) {
        if (canMove(next_x, next_y)) {
            if (GameLogic.goods[next_x, next_y] != -1) {
                return true;
            }
        }
        return false;
    }


    public virtual int what_am_i() {
        return 1;
    }

    public virtual void Logic( float delta ) {

    }
    
    public int follow_parent( int dir ) {
        
        //return -1;
        if ( dir == 0 ) {
            return 1;
        }else if ( dir == 1 ) {
            return 0;
        }else if ( dir == 2 ) {
            return 3;
        }else if ( dir == 3 ) {
            return 2;
        }
        return -1;
    }

    public bool draw_visited;

    public int next_dir_point( state a, state b ) {

        int ret = -1;
        
        if ( a.x == b.x+1 && a.y == b.y ) {
            ret = 0;
        }else if (a.x == b.x - 1 && a.y == b.y ) {
            ret = 1;
        }else if (a.x == b.x && a.y == b.y+1) {
            ret = 2;
        }else if (a.x == b.x && a.y == b.y-1) {
            ret = 3;
        }

        return (ret);

    }

    public void Move( float delta ) {
        
        //Debug.Log(des_x + " " + des_y + " " + cell_x + " " + cell_y + " " + cur_x + " " + " " + cur_y);
        
        if ( this.draw_visited ) {
            for ( int i = 0; i < GameLogic.HEIGHT; ++i ) {
                for ( int j = 0; j < GameLogic.WIDTH; ++j ) {

                    GameLogic.map[i, j].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

                    if ( visited[i,j] ) {
                        GameLogic.map[i,j].GetComponent<SpriteRenderer>().color = new Color(0.5f, 1f, 1f, .5f);
                        int u = next_dir_point(new state(i, j), par[i, j]);
                        GameLogic.arrows[i, j].SetNextDir( u );

                        //GameLogic.arrows[i, j].avatar.transform.position = new Vector3(i*GameLogic.tileSize, j*GameLogic.tileSize, GameLogic.ARROWS_LAYER);
                        //GameLogic.map[tmp.x, tmp.y].GetComponent<SpriteRenderer>().color = new Color(1.0f, 0f, 0f, .1f);
                    }

                    if ( par[i,j].x == -1 && par[i,j].y == -1 ) {
                        GameLogic.arrows[i, j].avatar.transform.position = new Vector3(j * GameLogic.tileSize, i * GameLogic.tileSize, -GameLogic.ARROWS_LAYER );
                    }else {
                        GameLogic.arrows[i, j].avatar.transform.position = new Vector3(j * GameLogic.tileSize, i * GameLogic.tileSize, +GameLogic.ARROWS_LAYER );
                    }

                }
                //Debug.Log(GameLogic.goods_names[good_of_interest]);

                if ( good_of_interest == GameLogic.UNDEFINED ) {
                    GameObject.Find("search_good").transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("question_mark");
                }else {
                    GameObject.Find("search_good").transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>( GameLogic.goods_names[good_of_interest] );
                }

            }
            
            List<state> newList = new List<state>(bfs_path_copy.Count);

            bfs_path_copy.ForEach((item) => {
                newList.Add((state)item);
            });

            while ( newList.Count > 0 ) {
                state tmp = newList[0];
                newList.RemoveAt(0);
                GameLogic.map[tmp.x, tmp.y].GetComponent<SpriteRenderer>().color = new Color(1.0f, 0f, 0f, .1f);
            }

        }
        
        
        if (dir != -1){
            this.avatar.GetComponent<Animator>().enabled = true;
        }else {
            this.visited[GetCellX(), GetCellY()] = true;
            this.avatar.GetComponent<Animator>().enabled = false;
            this.avatar.GetComponent<Animator>().Play(this.avatar.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0.0f);
            return;
        }

        if (dir == 1) {
            if (this.cur_x < des_x) {
                cur_x += delta * GameLogic.tileSize;
                this.avatar.transform.Translate(new Vector2(0,delta * GameLogic.tileSize));
                this.possession.transform.position = this.avatar.transform.position;// Translate(new Vector2(0, delta * GameLogic.tileSize));
            }else {

                int next_x = cell_x + units;
                int next_y = cell_y;

                //this.par[next_x, next_y] = new state(GetCellX(), GetCellY());
                this.visited[next_x, next_y] = true;

                cell_x += units;
                dir = -1;
                canDoLogic = false;
            }
        }else if ( dir == 0 ) {
            if (cur_x > des_x) {
                cur_x -= delta * GameLogic.tileSize;
                this.avatar.transform.Translate(new Vector2(0,-delta * GameLogic.tileSize));
                this.possession.transform.position = this.avatar.transform.position;// Translate(new Vector2(0,-delta * GameLogic.tileSize));
            }else {

                int next_x = cell_x - units;
                int next_y = cell_y;

                //this.par[next_x, next_y] = new state(GetCellX(), GetCellY());
                this.visited[next_x, next_y] = true;

                cell_x -= units;
                dir = -1;
                canDoLogic = false;
           }
        }
        
        if (dir == 3) {
            if (cur_y < des_y ) {
                cur_y += delta * GameLogic.tileSize;
                this.avatar.transform.Translate(new Vector2(delta * GameLogic.tileSize,0));
                this.possession.transform.position = this.avatar.transform.position;
            } else {

                int next_x = cell_x;
                int next_y = cell_y + units;

                //this.par[next_x, next_y] = new state(GetCellX(), GetCellY());
                this.visited[next_x, next_y] = true;

                cell_y += units;
                dir = -1;
                canDoLogic = false;
            }
        }else if ( dir == 2 ) {
             if (cur_y > des_y ) {
                cur_y -= delta * GameLogic.tileSize;
                this.avatar.transform.Translate(new Vector2(-delta * GameLogic.tileSize,0));
                this.possession.transform.position = this.avatar.transform.position;
            }else {

                int next_x = cell_x;
                int next_y = cell_y - units;

                //this.par[next_x, next_y] = new state(GetCellX(), GetCellY());
                this.visited[next_x, next_y] = true;

                cell_y -= units;
                dir = -1;
                canDoLogic = false;
            }
        }
        
    }

    public class state {
        public int x { get; set; }
        public int y { get; set; }

        public state(int x, int y ) {
            this.x = x;
            this.y = y;
        }
    }    

}
