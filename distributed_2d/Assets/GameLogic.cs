using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;
//using System.Runtime.InteropServices;
//using System.Windows.Forms;

public class GameLogic : MonoBehaviour {
  
    /*  
    [DllImport("user32.dll")]
    private static extern void SaveFileDialog();

    [DllImport("user32.dll")]
    private static extern void OpenFileDialog();
    */
    public static int UNDEFINED = -1;
    public static int ARROWS_LAYER = -3;
    public static int AGENT_LAYER = -2;
    public static int MAX_GOODS = 11;

    public static int[] dx = new int[] { -1, 1, 0, 0 };
    public static int[] dy = new int[] { 0, 0, -1, 1 };

    public static GameObject[,] map;
    public static Arrows[,] arrows;

    public static int[,] goods;
    public int agent_n = 0;
    public static int WIDTH = 20;
    public static int HEIGHT = 13;

    public static float tileSize = 0.4f;

    public static AgentRed[] red_agents;
    public static AgentBlue[] blue_agents;

    public static Village village_red;
    public static Village village_blue;

    public static string[] goods_names;

    public static List<GameObject> goods_sprites;
    
    public static float GameStart;
    public static float GameEnd;
    public static int winner;

    public static string[] lines;

    //Load Map
    public static void BuildMap(string path, bool read_file) {

        if (read_file) {
            lines = System.IO.File.ReadAllLines(@path);
        }

        is_abort = is_paused = false;

        GameObject.Find("Toggle_pause").GetComponentInChildren<Text>().text = "Pause";
        GameObject.Find("Toggle_abort").GetComponentInChildren<Text>().text = "Abort";

        GameObject.Find("selected_team").GetComponent<Text>().text = "Red";

        GameObject.Find("search_good").transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("question_mark");

        GameObject.Find("game_over").GetComponentInChildren<Text>().text = "";

        int cnt = 0;

        //read height, width, total_red, total_blue

        if (map != null) {

            for (int i = 0; i < HEIGHT; ++i) {
                for (int j = 0; j < WIDTH; ++j) {
                    map[i, j].GetComponent<SpriteRenderer>().sprite = null;// enabled = false;
                    map[i, j].GetComponent<SpriteRenderer>().enabled = false;
                    Destroy(map[i, j]);//.De.GetComponent<SpriteRenderer>() = null;
                }
            }
        }

        if (goods_sprites != null) {

            while (goods_sprites.Count > 0) {
                goods_sprites[0].GetComponent<SpriteRenderer>().enabled = false;
                goods_sprites[0].GetComponent<SpriteRenderer>().sprite = null;//.enabled = false;
                goods_sprites.RemoveAt(0);
            }

            goods_sprites.Clear();
        }

        goods_sprites = new List<GameObject>();
        goods_sprites.Clear();

        if (arrows != null) {

            for (int i = 0; i < HEIGHT; ++i) {
                for (int j = 0; j < WIDTH; ++j) {
                    arrows[i, j].GetAvatar().GetComponent<SpriteRenderer>().enabled = false;
                    arrows[i, j].GetAvatar().GetComponent<SpriteRenderer>().sprite = null;
                    Destroy(arrows[i, j]);//.De.GetComponent<SpriteRenderer>() = null;
                }
            }
        }

        if (red_agents != null) {

            for (int i = 0; i < red_agents.Length; ++i) {
                red_agents[i].GetAvatar().GetComponent<SpriteRenderer>().enabled = false;
                red_agents[i].GetAvatar().GetComponent<SpriteRenderer>().sprite = null;
                red_agents[i].GetPossession().GetComponent<SpriteRenderer>().enabled = false;
                red_agents[i].GetPossession().GetComponent<SpriteRenderer>().sprite = null;
                Destroy(red_agents[i]);
            }
        }

        if (blue_agents != null) {
            for (int i = 0; i < blue_agents.Length; ++i) {
                blue_agents[i].GetAvatar().GetComponent<SpriteRenderer>().enabled = false;
                blue_agents[i].GetAvatar().GetComponent<SpriteRenderer>().sprite = null;
                blue_agents[i].GetPossession().GetComponent<SpriteRenderer>().enabled = false;
                blue_agents[i].GetPossession().GetComponent<SpriteRenderer>().sprite = null;
                Destroy(blue_agents[i]);
            }
        }

        string[] tmp_str = lines[cnt].Split(' ');

        HEIGHT = Int32.Parse(tmp_str[0]);
        WIDTH = Int32.Parse(tmp_str[1]);
        int total_red = Int32.Parse(tmp_str[2]);
        int total_blue = Int32.Parse(tmp_str[3]);

        red_agents = null;
        blue_agents = null;

        goods = new int[HEIGHT, WIDTH];

        for (int i = 0; i < HEIGHT; ++i) {
            for (int j = 0; j < WIDTH; ++j) {
                goods[i, j] = -1;
            }
        }
        
        village_red.Reset();
        village_blue.Reset();

        for (int i = 0; i < HEIGHT; i++) {
            ++cnt;

            for (int j = 0; j < WIDTH; ++j) {
                if (lines[cnt][j] == '1') {
                    //xorio 1
                    village_red.setX(i);
                    village_red.setY(j);
                }else if (lines[cnt][j] == '2') {
                    //xorio 2
                    village_blue.setX(i);
                    village_blue.setY(j);
                }else if (lines[cnt][j] == '0') {
                    //keno
                }else if (lines[cnt][j] >= 'A' && lines[cnt][j] <= 'K') {
                    //11 agatha apo to A eos to K
                    goods[i, j] = lines[cnt][j] - 'A';
                }
            }

        }

        //set sprites
        map = new GameObject[HEIGHT, WIDTH];
        
        GameObject t = new GameObject();
        t.AddComponent<SpriteRenderer>();
        t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("house_red");
        t.transform.Translate(new Vector3(village_red.getY() * tileSize, village_red.getX() * tileSize, -2));
        t.transform.localScale = new Vector2(0.5f, 0.5f);

        goods_sprites.Add(t);

        t = new GameObject();
        t.AddComponent<SpriteRenderer>();
        t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("house_blue");
        t.transform.Translate(new Vector3(village_blue.getY() * tileSize, village_blue.getX() * tileSize, -2));

        goods_sprites.Add(t);

        arrows = new Arrows[GameLogic.HEIGHT, GameLogic.WIDTH];

        for (int i = 0; i < HEIGHT; ++i) {
            for (int j = 0; j < WIDTH; ++j) {
                //Debug.Log(i + " " + j);

                arrows[i, j] = new Arrows(i, j);

                map[i, j] = new GameObject();
                map[i, j].AddComponent<SpriteRenderer>();

                if (i == 0 && j == 0) {
                    map[i, j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grass_bottom_left");
                }else if (i == HEIGHT - 1 && j == WIDTH - 1) {
                    map[i, j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grass_top_right");
                }else if (i == HEIGHT - 1 && j == 0) {
                    map[i, j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grass_top_left");
                }else if (i == 0 && j == WIDTH - 1) {
                    map[i, j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grass_bottom_right");
                }else if (i == 0) {
                    map[i, j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grass_bottom_middle");
                }else if (i == HEIGHT - 1) {
                    map[i, j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grass_top_middle");
                } else if (j == 0) {
                    map[i, j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grass_middle_left");
                }else if (j == WIDTH - 1) {
                    map[i, j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grass_middle_right");
                }else {
                    map[i, j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("grass_center");
                }

                map[i, j].transform.Translate(new Vector3(j * tileSize, i * tileSize, 0));
                map[i, j].transform.localScale = new Vector2(2.35f, 2.35f);

                if (goods[i, j] != -1) {
                    GameObject tmp = new GameObject();
                    tmp.AddComponent<SpriteRenderer>();
                    //Debug.Log(goods[i, j]);
                    tmp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(goods_names[goods[i, j]]);

                    if (goods[i, j] <= 1) {
                        tmp.transform.localScale = new Vector2(0.5f, 0.5f);
                    }else if (goods[i, j] >= 5) {
                        tmp.transform.localScale = new Vector2(1.4f, 1.4f);
                    }
                    //tmp.GetComponent<Sprite>().rect.width = 32;
                    tmp.transform.Translate(new Vector3(j * tileSize, i * tileSize, -1));
                    goods_sprites.Add(tmp);
                }
            }
        }

        //end of set sprites

        red_agents = new AgentRed[total_red];

        for (int i = 0; i < red_agents.Length; ++i) {
            red_agents[i] = new AgentRed(village_red.getX(), village_red.getY());
            red_agents[i].visited[village_red.getX(), village_red.getY()] = true;
            ++cnt;
            red_agents[i].good_of_interest = (char)lines[cnt][0] - 'A';
        }

        blue_agents = new AgentBlue[total_blue];

        for (int i = 0; i < blue_agents.Length; ++i) {
            blue_agents[i] = new AgentBlue(village_blue.getX(), village_blue.getY());
            blue_agents[i].visited[village_blue.getX(), village_blue.getY()] = true;
            blue_agents[i].good_of_interest = GameLogic.UNDEFINED;
        }

        ++cnt;
        int red_needs = Int32.Parse(lines[cnt]);

        for (int i = 0; i < red_needs; ++i) {

            ++cnt;

            tmp_str = lines[cnt].Split(' ');

            int idx = (char)tmp_str[0][0] - 'A';
            int val = Int32.Parse(tmp_str[1]);

            village_red.setCapacity(idx, val);
        }

        ++cnt;
        int blue_needs = Int32.Parse(lines[cnt]);

        for (int i = 0; i < blue_needs; ++i) {

            ++cnt;

            tmp_str = lines[cnt].Split(' ');

            int idx = (char)tmp_str[0][0] - 'A';
            int val = Int32.Parse(tmp_str[1]);

            village_blue.setCapacity(idx, val);
        }

        List<string> list_string = new List<string>();

        for (int i = 0; i < red_agents.Length; ++i) {
            list_string.Add(i.ToString());
        }

        GameObject.Find("Dropdown").GetComponent<Dropdown>().ClearOptions();

        GameObject.Find("Dropdown").GetComponent<Dropdown>().AddOptions(list_string);

        GameStart = Time.realtimeSinceStartup;
        GameEnd = GameStart - 1;

        GameObject.Find("save_stats").GetComponent<Button>().interactable = false;

    }

    public void Save_Statistics() {

        if ( IsNullOrEmpty(red_agents) || IsNullOrEmpty(blue_agents) ) {
            return;
        }

        if (GameEnd < GameStart) {
            return;
        }

        System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
        sfd.Filter = "Txt Files (*.txt*)|*.txt*";
        
        string path = "";// EditorUtility.SaveFilePanel("Save statistics txt", "", "statistics.txt", "txt");
        
        System.Windows.Forms.DialogResult result = sfd.ShowDialog();
        if (result == System.Windows.Forms.DialogResult.OK) {
            path = sfd.FileName;
        }

        if (path.Length != 0 ) {
            string tmp = "";

            tmp += "Total time: " + (GameEnd - GameStart) + "\n";

            tmp += "Winner: ";

            if (winner == -1) {
                tmp += "it's a draw!";
            }else if (winner == 1) {
                tmp += "Red team";
            }else if (winner == 2) {
                tmp += "Blue team";
            }

            tmp += "\n";

            tmp += "Red team\n";

            for (int i = 0; i < red_agents.Length; ++i) {
                tmp += "Agent" + i + ": steps=" + red_agents[i].steps + " exchanged knowledge=" + red_agents[i].exc + "\n";
            }

            tmp += "Blue team\n";
            for (int i = 0; i < blue_agents.Length; ++i) {
                tmp += "Agent" + i + ": " + blue_agents[i].steps + "\n";
            }

            File.WriteAllText(path, tmp);

        }

    }

    public void LoadMap() {
        
        System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
        ofd.Filter = "Txt Files (*.txt*)|*.txt*";

        string path = "";// EditorUtility.OpenFilePanel("Load map from txt", "", "txt");

        System.Windows.Forms.DialogResult result = ofd.ShowDialog();
        if (result == System.Windows.Forms.DialogResult.OK) {
            path = ofd.FileName;
        }
        
        if (path.Length != 0) {
            WWW www = new WWW("file:///" + path);
            BuildMap(path, true);
            // GameObject.Find("Main Camera").transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
    
    Text txt;

    public static bool is_paused;
    public static bool is_abort;

    public void toggle_paused() {

        if ( IsNullOrEmpty(red_agents) || IsNullOrEmpty(blue_agents) ) {
            return;
        }

        is_paused = !is_paused;

        if (is_paused) {
            GameObject.Find("Toggle_pause").GetComponentInChildren<Text>().text = "Play";
        }else {
            GameObject.Find("Toggle_pause").GetComponentInChildren<Text>().text = "Pause";
        }

    }

    public void toggle_abort() {

        if ( IsNullOrEmpty(red_agents) || IsNullOrEmpty(blue_agents) ) {
            return;
        }

        is_abort = !is_abort;

        if (is_abort) {
            GameObject.Find("Toggle_abort").GetComponentInChildren<Text>().text = "Restart";
        }else {
            GameObject.Find("Toggle_abort").GetComponentInChildren<Text>().text = "Abort";
            BuildMap("", false);
        }

    }

    int cur_team;

    public void change_team() {
        
        deselect_agent();

        GameObject.Find("Dropdown").GetComponent<Dropdown>().ClearOptions();
        GameObject.Find("search_good").transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("question_mark");

        if (cur_team == 1) {

            List<string> list_string = new List<string>();

            for (int i = 0; i < red_agents.Length; ++i) {
                list_string.Add(i.ToString());
            }

            GameObject.Find("Dropdown").GetComponent<Dropdown>().AddOptions(list_string);

            GameObject.Find("selected_team").GetComponent<Text>().text = "Red";
        }else {
            List<string> list_string = new List<string>();

            for (int i = 0; i < blue_agents.Length; ++i) {
                list_string.Add(i.ToString());
            }

            GameObject.Find("Dropdown").GetComponent<Dropdown>().AddOptions(list_string);

            GameObject.Find("selected_team").GetComponent<Text>().text = "Blue";
        }

        this.cur_team = 1 - this.cur_team;
        
    }

    void ClearColors() {

        for (int i = 0; i < GameLogic.HEIGHT; ++i) {
            for (int j = 0; j < GameLogic.WIDTH; ++j) {

                GameLogic.map[i, j].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                GameLogic.arrows[i, j].GetAvatar().transform.position = new Vector3(GameLogic.arrows[i, j].GetCellY() * GameLogic.tileSize, GameLogic.arrows[i, j].GetCellX() * GameLogic.tileSize, -GameLogic.ARROWS_LAYER);

            }
        }

    }

    bool IsNullOrEmpty<T>( T[] array) {
        return array == null || array.Length == 0;
    }

    public void deselect_agent( ) {

        ClearColors();

        this.agent_n = 0;

        //GameObject.Find("search_good").transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("question_mark");

        //GameObject.Find("selected_team").GetComponent<Text>().text = "Red";

        for (int i = 0; i < red_agents.Length; ++i) {
            red_agents[i].draw_visited = false;
            red_agents[i].GetAvatar().GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }

        for (int i = 0; i < blue_agents.Length; ++i) {
            blue_agents[i].draw_visited = false;
            blue_agents[i].GetAvatar().GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }

    }

    public void change_agent(int number) {

        ClearColors();

        this.agent_n = number;
        
        for (int i = 0; i < red_agents.Length; ++i) {
            red_agents[i].draw_visited = false;
            red_agents[i].GetAvatar().GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }

        for (int i = 0; i < blue_agents.Length; ++i) {
            blue_agents[i].draw_visited = false;
            blue_agents[i].GetAvatar().GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }

        if (cur_team == 0) {
            red_agents[this.agent_n].draw_visited = true;
            red_agents[this.agent_n].GetAvatar().GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.3f, 0.4f, 0.7f);
        }else {
            blue_agents[this.agent_n].draw_visited = true;
            blue_agents[this.agent_n].GetAvatar().GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.3f, 0.4f, 0.7f);
        }

    }

    void setValues() {

        GameObject.Find("wheatScore").GetComponent<Text>().text = village_red.getQuantity(0) + "/" + village_red.getCapacity(0) + "			 " + village_blue.getQuantity(0) + "/" + village_blue.getCapacity(0);
        GameObject.Find("treeScore").GetComponent<Text>().text = village_red.getQuantity(1) + "/" + village_red.getCapacity(1) + "			 " + village_blue.getQuantity(1) + "/" + village_blue.getCapacity(1);
        GameObject.Find("ironScore").GetComponent<Text>().text = village_red.getQuantity(2) + "/" + village_red.getCapacity(2) + "			 " + village_blue.getQuantity(2) + "/" + village_blue.getCapacity(2);
        GameObject.Find("goldScore").GetComponent<Text>().text = village_red.getQuantity(3) + "/" + village_red.getCapacity(3) + "			 " + village_blue.getQuantity(3) + "/" + village_blue.getCapacity(3);
        GameObject.Find("mushroomScore").GetComponent<Text>().text = village_red.getQuantity(4) + "/" + village_red.getCapacity(4) + "			 " + village_blue.getQuantity(4) + "/" + village_blue.getCapacity(4);
        GameObject.Find("bignuggetScore").GetComponent<Text>().text = village_red.getQuantity(5) + "/" + village_red.getCapacity(5) + "			 " + village_blue.getQuantity(5) + "/" + village_blue.getCapacity(5);
        GameObject.Find("bluckberryScore").GetComponent<Text>().text = village_red.getQuantity(6) + "/" + village_red.getCapacity(6) + "			 " + village_blue.getQuantity(6) + "/" + village_blue.getCapacity(6);
        GameObject.Find("honeyScore").GetComponent<Text>().text = village_red.getQuantity(7) + "/" + village_red.getCapacity(7) + "			 " + village_blue.getQuantity(7) + "/" + village_blue.getCapacity(7);
        GameObject.Find("grassgemScore").GetComponent<Text>().text = village_red.getQuantity(8) + "/" + village_red.getCapacity(8) + "			 " + village_blue.getQuantity(8) + "/" + village_blue.getCapacity(8);
        GameObject.Find("metalcoatScore").GetComponent<Text>().text = village_red.getQuantity(9) + "/" + village_red.getCapacity(9) + "			 " + village_blue.getQuantity(9) + "/" + village_blue.getCapacity(9);
    }
    
     void Start() {
        //Debug.Log(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));

        GameObject.Find("game_over").GetComponentInChildren<Text>().text = "";

        goods_names = new string[] { "wheat", "tree", "iron", "gold", "bigmushroom", "bignugget", "blukberry", "honey", "grassgem", "metalcoat" };
        village_red = new Village();
        village_blue = new Village();

        GameObject.Find("save_stats").GetComponent<Button>().interactable = false;

        //testing
        /*
        string path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
        path += "\\katanemimeni\\map1.txt";
        BuildMap(path);
        */

    }

    // Update is called once per frame
    void Update() {
        
        if ( (GameEnd > GameStart) || IsNullOrEmpty(red_agents) || IsNullOrEmpty(blue_agents) ) {
            GameObject.Find("Toggle_team").GetComponent<Button>().interactable = false;
            GameObject.Find("Toggle_pause").GetComponent<Button>().interactable = false;
            GameObject.Find("Deselect_agent").GetComponent<Button>().interactable = false;
            GameObject.Find("Toggle_abort").GetComponent<Button>().interactable = false;
            GameObject.Find("Dropdown").GetComponent<Dropdown>().interactable = false;
        }else {
            
            if ( lines.Length > 0 ) {
                GameObject.Find("Toggle_team").GetComponent<Button>().interactable = true;
                GameObject.Find("Toggle_pause").GetComponent<Button>().interactable = true;
                GameObject.Find("Deselect_agent").GetComponent<Button>().interactable = true;
                GameObject.Find("Toggle_abort").GetComponent<Button>().interactable = true;
                GameObject.Find("Dropdown").GetComponent<Dropdown>().interactable = true;
            }else {
                GameObject.Find("Toggle_pause").GetComponent<Button>().interactable = false;
            }

        }
        
        if (Input.GetKeyUp("w")) {
            //red_agents[2].SetNextDir(1,1);
            GameObject.Find("Main Camera").transform.position += new Vector3(0.0f, -1.0f, 0.0f);
        }else if (Input.GetKeyUp("s")) {
            //red_agents[2].SetNextDir(0,1);
            GameObject.Find("Main Camera").transform.position += new Vector3(0.0f, 1.0f, 0.0f);
        }else if (Input.GetKeyUp("d")) {
            //red_agents[2].SetNextDir(3,1);
            GameObject.Find("Main Camera").transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
        }else if (Input.GetKeyUp("a")) {
            //red_agents[2].SetNextDir(2,1);
            GameObject.Find("Main Camera").transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        }

        setValues();

        if (is_paused || is_abort) {
            return;
        }

        if (IsNullOrEmpty(red_agents) || IsNullOrEmpty(blue_agents)) return;

        // ean kapoio agatho den exei gemisei tote i bool einai false alliws ola einai full kai to paixnidi termatizei
        bool red_finish = true;

        for (int i = 0; i < goods_names.Length && red_finish; ++i) {
            if (village_red.getQuantity(i) < village_red.getCapacity(i)) {
                red_finish = false;
            }
        }

        if (IsNullOrEmpty(red_agents)) {
            red_finish = false;
        }

        bool blue_finish = true;

        for (int i = 0; i < goods_names.Length && blue_finish; ++i) {
            if (village_blue.getQuantity(i) < village_blue.getCapacity(i)) {
                blue_finish = false;
            }
        }

        if (IsNullOrEmpty(blue_agents)) {
            blue_finish = false;
        }
        
        if ((red_finish || blue_finish)) {
            
            if ( GameEnd < GameStart ) {
                GameEnd = Time.realtimeSinceStartup;
                //GameObject.Find("Toggle_team").GetComponent<Button>().interactable = true;

                GameObject.Find("save_stats").GetComponent<Button>().interactable = true;
            }

            if (red_finish && blue_finish) {
                GameObject.Find("game_over").GetComponentInChildren<Text>().text = "GAME OVER! WOW! IT'S A DRAW!";
                winner = -1;
            }else if (red_finish) {
                GameObject.Find("game_over").GetComponentInChildren<Text>().text = "GAME OVER! RED TEAM WINS!";
                winner = 1;
            }else {
                GameObject.Find("game_over").GetComponentInChildren<Text>().text = "GAME OVER! BLUE TEAM WINS!";
                winner = 2;
            }
            
            return;
        }

        float delta = Time.deltaTime;

        //agents[0].Move(delta);

        int total = 0;
        int total_blue = 0;

        for (int i = 0; i < red_agents.Length; ++i) {

            if (red_agents[i].canDoLogic) {
                red_agents[i].Logic(delta);
            }

        }

        for (int i = 0; i < blue_agents.Length; ++i) {

            if (blue_agents[i].canDoLogic) {
                blue_agents[i].Logic(2 * delta);
            }

        }

        for (int i = 0; i < red_agents.Length; ++i) {

            if (red_agents[i].isMoving() == false) {// anDoLogic == false) {
                ++total;
            }

        }

        for (int i = 0; i < blue_agents.Length; ++i) {

            if (blue_agents[i].isMoving() == false) {// anDoLogic == false) {
                ++total_blue;
            }

        }

        if (total_blue == blue_agents.Length) {
            for (int i = 0; i < blue_agents.Length; ++i) {
                blue_agents[i].canDoLogic = true;
            }
        }

        if (total == red_agents.Length) {

            //set logic available again in order to make move
            for (int i = 0; i < red_agents.Length; ++i) {
                red_agents[i].canDoLogic = true;
            }

            //do exchange

            for (int i = 0; i < red_agents.Length; ++i) {

                for (int j = 0; j < red_agents.Length; ++j) {

                    if (i == j) {
                        continue;
                    }

                    if (GameLogic.red_agents[i].what_am_i() == 1 && GameLogic.red_agents[j].what_am_i() == 1) {

                        bool interact = false;

                        for (int k = 0; k < 4; ++k) {
                            int next_x = GameLogic.red_agents[i].GetCellX() + GameLogic.dx[k];
                            int next_y = GameLogic.red_agents[i].GetCellY() + GameLogic.dy[k];

                            if (next_x == GameLogic.red_agents[j].GetCellX() && next_y == GameLogic.red_agents[j].GetCellY()) {
                                interact = true;
                                break;
                            }

                        }

                        if (GameLogic.red_agents[i].GetCellX() == GameLogic.red_agents[j].GetCellX() && GameLogic.red_agents[i].GetCellY() == GameLogic.red_agents[j].GetCellY()) {
                            interact = true;
                        }

                        if (interact == true) {
                            //Debug.Log("interact " + i + " " + j);
                            GameLogic.red_agents[i].ExchangeKnowledge(GameLogic.red_agents[j].visited, GameLogic.red_agents[j].goods_coord);
                            ++GameLogic.red_agents[i].exc;

                            if (GameLogic.red_agents[i].HaveGood == false) {

                                if (GameLogic.red_agents[i].goods_coord[GameLogic.red_agents[i].good_of_interest] == true) {
                                    if (GameLogic.red_agents[i].bfs_path.Count == 0) {
                                        GameLogic.red_agents[i].BreadthFirstSearch(GameLogic.red_agents[i].GetCellX(), GameLogic.red_agents[i].GetCellY(), -1, -1);
                                    }
                                }

                            }else {

                                if (GameLogic.red_agents[i].bfs_path.Count == 0) {
                                    GameLogic.red_agents[i].BreadthFirstSearch(GameLogic.red_agents[i].GetCellX(), GameLogic.red_agents[i].GetCellY(), 1, 4);
                                }
                            }

                        }

                    }

                }

            }

        }

    }
    
}

