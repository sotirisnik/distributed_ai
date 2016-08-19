using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AgentRed : Agent {

    public int exc;
    
    public AgentRed( ) : base() {

        goods_coord = new bool[GameLogic.goods_names.Length];// new List< Pair >[GameLogic.MAX_GOODS];

        /*
        for ( int i = 0; i < goods_coord.Length; ++i ) {
            goods_coord[i] = new List<Pair>();
        }*/

        exc = 0;
        par_bfs = new state[GameLogic.HEIGHT, GameLogic.WIDTH];
        dist_bfs = new int[GameLogic.HEIGHT, GameLogic.WIDTH];

        this.bfs_path = new Stack<int>();

        this.SetAvatar((GameObject)Instantiate( (GameObject)Resources.Load("agent_red"), new Vector3( this.GetCellY() * GameLogic.tileSize, this.GetCellX() * GameLogic.tileSize, GameLogic.AGENT_LAYER), Quaternion.identity));

    }

    public AgentRed( int cur_x, int cur_y ) : base( cur_x, cur_y ) {

        goods_coord = new bool[GameLogic.goods_names.Length];// new List< Pair >[GameLogic.MAX_GOODS];

        /*
        for ( int i = 0; i < goods_coord.Length; ++i ) {
            goods_coord[i] = new List<Pair>();
        }*/

        exc = 0;
        par_bfs = new state[GameLogic.HEIGHT, GameLogic.WIDTH];
        dist_bfs = new int[GameLogic.HEIGHT, GameLogic.WIDTH];

        this.bfs_path = new Stack<int>();

        this.SetAvatar((GameObject)Instantiate((GameObject)Resources.Load("agent_red"), new Vector3( this.GetCellY() * GameLogic.tileSize, this.GetCellX() * GameLogic.tileSize, GameLogic.AGENT_LAYER), Quaternion.identity));

        //this.GetAvatar().AddComponent<TextMesh>().text = "malaka";
        
    }

    public override int what_am_i() {
        return 1;
    }

    public void ExchangeKnowledge( bool[,]neo_visited, bool []neo_goods_coord ) {

        //simeiose oti exei episkefthei kai o allos praktoras os true
        for ( int i = 0; i < GameLogic.HEIGHT; ++i ) {
            for ( int j = 0; j < GameLogic.WIDTH; ++j ) {
                if ( neo_visited[i,j] == true ) {
                    this.visited[i,j] = true;
                }
            }
        }

        //trexe dfs me riza to xorio gia na ftiaxtoun oi pateres
        for ( int i = 0; i < GameLogic.HEIGHT; ++i ) {
            for ( int j = 0; j < GameLogic.WIDTH; ++j ) {
                this.fix_par[i, j] = new state(-1, -1);
                this.fix_visited[i,j] = false;
            }
        }

        this.fix_parent(GameLogic.village_red.getX(), GameLogic.village_red.getY());// 1, 4);

        for ( int i = 0; i < GameLogic.HEIGHT; ++i ) {
            for ( int j = 0; j < GameLogic.WIDTH; ++j ) {
                this.par[i, j] = this.fix_par[i, j];
            }
        }


        /*
        for ( int i = 0; i < GameLogic.MAX_GOODS; ++i ) {
            this.goods_coord[i].AddRange(tmp.goods_coord[i]);
        }*/

        for ( int i = 0; i < neo_goods_coord.Length; ++i ) {
            if ( neo_goods_coord[i] == true ) {
                this.goods_coord[i] = true;
            }
        }

    }

    void fix_parent( int x, int y ) {
        
        if ( fix_visited[x,y] == true ) {
            return;
        }

        fix_visited[x, y] = true;
        
        for (int i = 0; i < 4; ++i) {

            int next_x = x + GameLogic.dx[i];
            int next_y = y + GameLogic.dy[i];
            
            if (this.canMove(next_x, next_y)) {

                if ( next_x == GameLogic.village_blue.getX() && next_y == GameLogic.village_blue.getY() ) {
                    continue;
                }

                if (this.visited[next_x, next_y] == true && fix_visited[next_x,next_y] == false) {
                    this.fix_par[next_x, next_y] = new state(x, y);// i;
                    this.fix_parent(next_x, next_y);
                }
            }
        }

    }

    int good_id = 0;

    public override void Logic( float delta ) {
        
        if ( canDoLogic == false ) {
            return;
        }

        if (findGood(GetCellX(), GetCellY()) ) {
            this.goods_coord[GameLogic.goods[GetCellX(), GetCellY()]] = true;
        }

        if ( findGood( GetCellX(), GetCellY() ) && isGoodOfMyInterest(GetCellX(),GetCellY())) {
            if ( HaveGood == false ) {
                HaveGood = true;
                good_id = GameLogic.goods[GetCellX(), GetCellY()];

                this.GetPossession().GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(GameLogic.goods_names[good_id]);

                 if (good_id <= 1) {
                    this.GetPossession().GetComponent<SpriteRenderer>().transform.localScale = new Vector2(0.25f, 0.25f);
                 } else if (good_id >= 5) {
                    this.GetPossession().GetComponent<SpriteRenderer>().transform.localScale = new Vector2(0.7f, 0.7f);
                 }else {
                    this.GetPossession().GetComponent<SpriteRenderer>().transform.localScale = new Vector2(0.5f, 0.5f);
                 }

                /*
                if (this.goods_coord[0].Contains(new Pair(GetCellX(), GetCellY())) == false ) {
                    this.goods_coord[0].Add(new Pair(GetCellX(), GetCellY()));
                }*/

                this.goods_coord[good_of_interest] = true;

                //GameObject.Find("Debug_move").GetComponent<Text>().text = "cur=" + GetCellX() + " " + GetCellY();

                if ( this.bfs_path.Count == 0) {
                    BreadthFirstSearch(GetCellX(), GetCellY(), GameLogic.village_red.getX(), GameLogic.village_red.getY() );//1, 4);
                }
                
            }
        }

        if ( this.GetCellX() == GameLogic.village_red.getX() && this.GetCellY() == GameLogic.village_red.getY() ) {

            if ( HaveGood ) {
                GameLogic.village_red.AddQuantity(this.good_of_interest);

                if (this.goods_coord[ this.good_of_interest ] == true ) {
                    BreadthFirstSearch( GameLogic.village_red.getX(), GameLogic.village_red.getY(), -1, -1 );
                }else {
                    //
                    bool found_pos = false;

                    int dest_x = -1;
                    int dest_y = -1;

                    for ( int k = 0; k < GameLogic.HEIGHT && !found_pos; ++k ) {
                        for ( int z = 0; z < GameLogic.WIDTH && !found_pos; ++z ) {
                            if ( visited[k,z] ) {
                                for ( int m = 0; m < 4 && !found_pos; ++m ) {
                                    int next_x = k + GameLogic.dx[m];
                                    int next_y = z + GameLogic.dy[m];

                                    if ( canMove( next_x, next_y ) ) {

                                        if ( next_x == GameLogic.village_blue.getX() && next_y == GameLogic.village_blue.getY() ) {
                                            continue;
                                        }

                                        if ( visited[next_x,next_y] == false ) {
                                            dest_x = k;
                                            dest_y = z;
                                            found_pos = true;
                                        }
                                    }

                                }
                            }
                        }
                    }

                    if ( found_pos ) {
                        BreadthFirstSearch(GameLogic.village_red.getX(), GameLogic.village_red.getY(), dest_x, dest_y);
                    }
                    //
                }

            }

            HaveGood = false;

            this.GetPossession().GetComponent<SpriteRenderer>().sprite = null;
            
        }

        //Find next move
        if ( this.isMoving() == false ) {

            ++steps;

            if ( this.bfs_path.Count > 0 ) {
                this.SetNextDir(this.bfs_path.Pop(), 1);
                //this.bfs_path.Remove(0);
                return;
            }

            int next_x = 0;
            int next_y = 0;

            bool found = false;
           
            int cnt = 0;
             
            for(int i = 0; i < 4 && !HaveGood; ++i ) {

                next_x = GetCellX() + GameLogic.dx[i];
                next_y = GetCellY() + GameLogic.dy[i];
                if (this.canMove( next_x, next_y ) ) {

                    if ( next_x == GameLogic.village_blue.getX() && next_y == GameLogic.village_blue.getY() ) {
                        continue;
                    }

                    if (this.visited[next_x, next_y] == false) {

                        if (this.findGood(next_x, next_y)) {
                            if ( this.isGoodOfMyInterest(next_x,next_y) == false ) {
                                continue;
                            }
                        }

                        ++cnt;
                        
                    }
                }
            }
            
            int z = UnityEngine.Random.Range(1, cnt + 1);//random number from 1 to cnt

            int neo_cnt = 0;

            for (int i = 0; i < 4 && !HaveGood; ++i) {

                next_x = GetCellX() + GameLogic.dx[i];
                next_y = GetCellY() + GameLogic.dy[i];

                if (this.canMove(next_x, next_y)) {

                    if ( next_x == GameLogic.village_blue.getX() && next_y == GameLogic.village_blue.getY() ) {
                        continue;
                    }

                    if (this.visited[next_x, next_y] == false) {

                        if (this.findGood(next_x, next_y)) {
                            if ( this.isGoodOfMyInterest(next_x,next_y) == false ) {
                                continue;
                            }else {
                                neo_cnt = z - 1;
                            }
                        }

                        ++neo_cnt;

                        if (neo_cnt != z ) {
                            continue;
                        }
                        
                        this.par[next_x, next_y] = new state( GetCellX(), GetCellY() );
                        //this.visited[next_x, next_y] = true;
                        this.SetNextDir(i, 1);

                        found = true;
                        break;
                    }
                }
                    //Debug.Log(found);
            }
            if (found == false || HaveGood == true ) {
                //Debug.Log("par length := " + next_x + " " + next_y + " " + GameLogic.HEIGHT + " " + GameLogic.WIDTH );

                if ( HaveGood ) {
                    if (this.bfs_path.Count > 0) {
                        this.SetNextDir( this.bfs_path.Pop(), 1 );
                        //this.bfs_path.Remove(0);// RemoveAt(0);
                    }else if ( this.bfs_path.Count == 0 ) {
                        BreadthFirstSearch(GetCellX(), GetCellY(), GameLogic.village_red.getX(), GameLogic.village_red.getY() );
                    }
                    return;
                }

                int u = next_dir_point( new state(GetCellX(), GetCellY()), par[GetCellX(),GetCellY()] );//follow_parent(par[GetCellX(), GetCellY()]);
                if (u != -1) {
                    visited[GetCellX(), GetCellY()] = true;
                    this.SetNextDir(u, 1);
                }
                
            }
        } else {
            this.Move(delta);
        }
            
    }

    public bool isGoodOfMyInterest( int x, int y ) {
        return (GameLogic.goods[x, y] == good_of_interest);
    }

    public class Pair
    {
        public int First { get; set; }
        public int Second { get; set; }

        public Pair( int first, int second ) {
            this.First = first;
            this.Second = second;
        }

        public override bool Equals(object obj) {
            Pair tmp = obj as Pair;
            return ( this.First == tmp.First && this.Second == tmp.Second );
        }

        public override int GetHashCode() {
            return this.First.GetHashCode() ^ this.Second.GetHashCode();
        }

    }
    
    private int[,] dist_bfs;
    private state[,] par_bfs;
   
    public void BreadthFirstSearch( int start_x,int start_y, int end_x, int end_y ) {
        
        for (int i = 0; i < GameLogic.HEIGHT; i++) {
            for (int j = 0; j < GameLogic.WIDTH; j++) {
                dist_bfs[i,j] = -1;
                par_bfs[i,j] = new state(-1,-1);
            }
        }
        
        BFS( start_x, start_y, end_x, end_y );
    }

    void BFS( int start_x, int start_y, int des_x, int des_y) {

        var queue = new Queue<state>();
        state s = new state(start_x, start_y);
        queue.Enqueue(s);

        dist_bfs[s.x, s.y] = 0;

        bfs_path_copy = new List<state>();
        bfs_path_copy.Clear();
        
        while (queue.Count > 0 ) {

            state top = queue.Dequeue();

            if ( top.x == des_x && top.y == des_y ) {
                break;
            }
            
            for (int k = 0; k < 4; k++) {

                int next_x = top.x + GameLogic.dx[k];
                int next_y = top.y + GameLogic.dy[k];

                if ( canMove(next_x, next_y) ) {

                    if ( next_x == GameLogic.village_blue.getX() && next_y == GameLogic.village_blue.getY() ) {
                        continue;
                    }

                    if ( this.dist_bfs[next_x, next_y] == -1 && this.visited[next_x, next_y] == true ) {
                        queue.Enqueue(new state(next_x, next_y));
                        this.dist_bfs[next_x, next_y] = this.dist_bfs[top.x, top.y] + 1;
                        this.par_bfs[next_x, next_y] = top;
                    }
                }
            }
        }

        String ttt = "";

        for ( int i = 0; i < GameLogic.HEIGHT; ++i ) {
            for ( int j = 0; j < GameLogic.WIDTH; ++j ) {
                ttt += dist_bfs[i, j] + " ";
            }
            ttt += "\n";
        }
        
        if ( des_x == -1 && des_y == -1 ) {

            int minu_val = -100;
        
            for ( int i = 0; i < GameLogic.HEIGHT; ++i ) {
                for ( int j = 0; j < GameLogic.WIDTH; ++j ) {

                    if ( dist_bfs[i,j] == -1 || this.visited[i,j] == false ) {
                        continue;
                    }

                    if (GameLogic.goods[i,j] == this.good_of_interest && ( minu_val == -100 || dist_bfs[i,j] < minu_val ) ) {
                        minu_val = dist_bfs[i,j];
                        des_x = i;
                        des_y = j;
                    }
                }
            }

        }
        
        this.bfs_path = new Stack<int>();
        this.bfs_path.Clear();
        
        if ( des_x == -1 && des_y == -1 ) {
            return;
        }

        /*
        if ( des_x == -1 && des_y == -1 ) {
            start_x = GetCellX();
            start_y = GetCellY();
            des_x = 1;
            des_y = 4;
        }*/
        
        if ( dist_bfs[des_x,des_y] == -1 ) {
            //GameObject.Find("Debug").GetComponent<Text>().text += "no path";// start_x + " " + start_y + " " + des_x + " " + des_y + "\n";

            return;
        }
        
        state tmp = new state(des_x, des_y);

        bfs_path_copy.Add( new state(des_x, des_y) );

        state next = new state(-1,-1);

        while (true) {

            int wut = -1;

            next = par_bfs[tmp.x, tmp.y];

            if ( next.x == -1 && next.y == -1 ) {
                break;
            }

            bfs_path_copy.Add(new state(next.x, next.y));

            if ( next.x == tmp.x+1 && next.y == tmp.y ) {
                wut = 0;
            }else if (next.x == tmp.x - 1 && next.y == tmp.y ) {
                wut = 1;
            }else if (next.x == tmp.x && next.y == tmp.y+1) {
                wut = 2;
            }else if (next.x == tmp.x && next.y == tmp.y-1) {
                wut = 3;
            }
            
            this.bfs_path.Push(wut);
            //Debug.Log("bfs path size " + bfs_path.Count);
            next = tmp;
            
            /*
            if ( draw_visited ) {
                GameLogic.map[tmp.x, tmp.y].GetComponent<SpriteRenderer>().color = new Color(1.0f, 0f, 0f, .1f);
            }*/

            tmp = par_bfs[tmp.x, tmp.y];

            if (tmp.x == start_x && tmp.y == start_y) {
                if ( next.x == tmp.x+1 && next.y == tmp.y ) {
                    wut = 1;
                }else if (next.x == tmp.x - 1 && next.y == tmp.y ) {
                    wut = 0;
                }else if (next.x == tmp.x && next.y == tmp.y+1) {
                    wut = 2;
                }else if (next.x == tmp.x && next.y == tmp.y-1) {
                    wut = 3;
                }
                break;
            }

        }

        /*
        var copy_path = this.bfs_path;

        while ( copy_path.Count > 0 ) {

            int top = copy_path.Pop();

            GameObject.Find("Debug").GetComponent<Text>().text += "d=" + top + " ";

        }*/

        //this.bfs_path.Reverse();

    }

}