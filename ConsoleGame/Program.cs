using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleGame
{
    class Program
    {
        
        static class Game
        {
            public static List<Enemy> enemy;
            private static double n = Math.Ceiling(Playfield.getFieldHeight() * 0.5);   // number of enemies is relative to the map size

            public static void Start()
            {
                Menu.StartMenu();                       // welcoming the player
                Playfield.Initiate();                   // setting up the map
                enemy = InstantiateEnemies((int) n);    // setting up the enemies
                Playfield.Draw();                       // drawing everything up
                GameLoop();                             // starting the game
            }
            
            private static void GameLoop()
            {
                int wantToPlay = 1;
                
                while (wantToPlay > 0)
                {
                    wantToPlay = Input();   // input can be any int, but it's only set to 0 if the player wants to exit or dies
                    if (wantToPlay == 0)
                    {
                        Console.WriteLine("\nDo you want to play one more? (y/n)");
                        if (Console.ReadKey().Key == ConsoleKey.Y)  // reseting the game and looping back
                        {
                            wantToPlay = 1;
                            Player.ResetStats();
                            Menu.StartMenu();
                            Playfield.Initiate();
                            enemy = InstantiateEnemies((int)n);
                            Playfield.Draw();
                        }

                    }
                }
            }
            public static int Input()
            {
                ConsoleKeyInfo input = Console.ReadKey();
                switch (input.Key)  // checking for pressed arrow keys, exit (x) key, and possible enemy selectors
                {
                    case ConsoleKey.X:  // exit the game
                        return 0;
                    case ConsoleKey.UpArrow:    // up and right arrow have a plus function to generate a new map if top-right corner is reached
                        if(Playfield.getMap()[Player.getPositionX() - 1][Player.getPositionY()] == 'O')
                        {
                            PlayerReachedEndPoint();
                        }
                        else if (Playfield.getMap()[Player.getPositionX() - 1][Player.getPositionY()] == ' ')
                        {
                            Playfield.setMapCell(Player.getPositionX(), Player.getPositionY(), ' ');    // setting the cell empty where the player stood
                            Player.setPosition(Player.getPositionX() - 1, Player.getPositionY());       // one step upwards
                            Playfield.Draw();
                            if (Player.CheckEnemiesNearby(enemy).Capacity > 0)                          // checking for enemies after every step
                            {
                                if (!Menu.EnemiesMenu()) { return 0; }                                  // EnemiesMenu handles the selection of enemies
                            }
                        }
                        return 10;
                    case ConsoleKey.RightArrow:
                        if(Playfield.getMap()[Player.getPositionX()][Player.getPositionY() + 1] == 'O')
                        {
                            PlayerReachedEndPoint();
                        }
                        else if (Playfield.getMap()[Player.getPositionX()][Player.getPositionY() + 1] == ' ')
                        {
                            Playfield.setMapCell(Player.getPositionX(), Player.getPositionY(), ' ');
                            Player.setPosition(Player.getPositionX(), Player.getPositionY() + 1);
                            Playfield.Draw();
                            if (Player.CheckEnemiesNearby(enemy).Capacity > 0)
                            {
                                if (!Menu.EnemiesMenu()) { return 0; }
                            }
                        }
                        return 10;
                    case ConsoleKey.DownArrow:
                        if (Playfield.getMap()[Player.getPositionX() + 1][Player.getPositionY()] == ' ')
                        {
                            Playfield.setMapCell(Player.getPositionX(), Player.getPositionY(), ' ');
                            Player.setPosition(Player.getPositionX() + 1, Player.getPositionY());
                            Playfield.Draw();
                            if (Player.CheckEnemiesNearby(enemy).Capacity > 0)
                            {
                                if (!Menu.EnemiesMenu()) { return 0; }
                            }
                        }
                        return 10;
                    case ConsoleKey.LeftArrow:
                        if (Playfield.getMap()[Player.getPositionX()][Player.getPositionY() - 1] == ' ')
                        {
                            Playfield.setMapCell(Player.getPositionX(), Player.getPositionY(), ' ');
                            Player.setPosition(Player.getPositionX(), Player.getPositionY() - 1);
                            Playfield.Draw();
                            if (Player.CheckEnemiesNearby(enemy).Capacity > 0)
                            {
                                if (!Menu.EnemiesMenu()) { return 0; }
                            }
                        }
                        return 10;
                        
                    case ConsoleKey.D1: return 1;   // top row of numbers on the keyboard
                    case ConsoleKey.D2: return 2;
                    case ConsoleKey.D3: return 3;
                    case ConsoleKey.D4: return 4;
                    case ConsoleKey.D5: return 5;
                    case ConsoleKey.D6: return 6;
                    case ConsoleKey.D7: return 7;
                    default: break;
                }
                return 10;  // 10 means that the player only moved the character or pressed a wrong key
            }
            private static List<Enemy> InstantiateEnemies(int n)
            {
                Random random = new Random();
                int x, y;
                List<Enemy> enemies = new List<Enemy>();
                for (int i = 0; i < n; i++)
                {
                    // searching for an empty cell to place enemy
                    do
                    {
                        x = random.Next(2, Playfield.getFieldHeight() - 2);
                        y = random.Next(2, Playfield.getFieldWidth() - 2);
                    } while (Playfield.getMap()[x][y] != ' ');

                    switch (random.Next(3))     // choosing between easy, medium or hard enemy
                    {
                        case 0:
                            Enemy e = new EasyEnemy(x, y, i);   // enemy constructors need the x, y position and a number for ID
                            enemies.Add(e);
                            Playfield.getMap()[e.getPositionX()][e.getPositionY()] = e.getCharacter();  // setting the map cell to show the enemy's icon
                            break;
                        case 1:
                            Enemy m = new MediumEnemy(x, y, i);
                            enemies.Add(m);
                            Playfield.getMap()[m.getPositionX()][m.getPositionY()] = m.getCharacter();
                            break;
                        case 2:
                            Enemy h = new HardEnemy(x, y, i);
                            enemies.Add(h);
                            Playfield.getMap()[h.getPositionX()][h.getPositionY()] = h.getCharacter();
                            break;
                        default:
                            break;
                    }
                }
                return enemies;
            }
            private static void PlayerReachedEndPoint()
            {
                // if player is at the end point, a new map is generated and the character is placed in the bottom-left corner
                Player.setPosition(Playfield.getFieldHeight() - 2, 1);
                Playfield.Initiate();
                enemy = InstantiateEnemies((int)Game.n);
                Playfield.Draw();
            }
            public static void SaveScore()
            {
                string fileName = "ScoreBoard.txt";
                try
                {
                    StreamWriter sw;
                    if (!File.Exists(fileName))
                    {
                        sw = File.CreateText(fileName);
                    }
                    else
                    {
                        sw = File.AppendText(fileName);
                    }
                    if(Player.getLevel() < 10)
                    {
                        sw.WriteLine($"Date: {DateTime.Today.ToString("yyyy.MM.dd.")}\tName: {Player.getName()}\tLevel: 0{Player.getLevel()}");
                    }
                    else
                    {
                        sw.WriteLine($"Date: {DateTime.Today.ToString("yyyy.MM.dd.")}\tName: {Player.getName()}\tLevel: {Player.getLevel()}");
                    }
                    sw.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong during score saving.");
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
            public static void ShowScore()
            {
                StreamReader sr = new StreamReader("ScoreBoard.txt");
                try
                {
                    List<string> text = new List<string>();
                    List<string> scores = new List<string>();
                    int n = 0;
                    int t = 0;

                    Console.Clear();
                    while (!sr.EndOfStream)
                    {
                        text.Add(sr.ReadLine());
                        scores.Add(text[n++].Split("Level: ")[1]);
                    }

                    if(text.Count > 1)
                    {
                        for (int i = 0; i < scores.Count; i++)
                        {
                            for (int j = 1; j < scores.Count - i; j++)
                            {
                                if (int.Parse(scores[j - 1]) < int.Parse(scores[j]))
                                {
                                    string temp = scores[j - 1];
                                    scores[j - 1] = scores[j];
                                    scores[j] = temp;
                                    temp = text[j - 1];
                                    text[j - 1] = text[j];
                                    text[j] = temp;
                                }
                            }
                        }
                    }
                    
                    while (t < text.Count && t < 10)            // showing the top 10 scores
                    {
                        Console.WriteLine(t + 1 + ".\t" + text[t]);
                        t++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("There are no high scores yet.");
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Console.WriteLine("Press Enter to continue.");
                    Console.ReadLine();
                    sr.Close();
                }
            }
        }

        static class Fight
        {
            private static Enemy attackedEnemy;
            private static bool b;

            public static bool Start()
            {
                b = true;
                ConsoleKeyInfo pressed;
                while (b)
                {

                    Menu.FightMenu();               // fight screen
                    pressed = Console.ReadKey();
                    if (pressed.Key == ConsoleKey.Enter || pressed.Key == ConsoleKey.F)
                    {
                        b = false;
                        switch (pressed.Key)
                        {
                            case ConsoleKey.Enter: return EvaluateFight();              // set to true if player won, false if enemy won
                            case ConsoleKey.F:                                          // player fleeing
                                double health = Math.Floor(Player.getHealth() * 0.5);   // halving the player health
                                Player.setHealth(-(int)health);
                                Console.WriteLine($"\nYou have {Player.getHealth()} health left.");
                                Console.WriteLine("Press Enter to continue.");
                                Console.ReadLine();
                                return false;
                            default: break;
                        }
                    }
                }

                return false;
            }

            private static bool EvaluateFight()
            {
                Random random = new Random();
                int i = 1;
                int playerTotalDamage = 0;
                int enemyTotalDamage = 0;
                while ((Player.getHealth() > 0) && (attackedEnemy.getHealth() > 0))
                {
                    double r = random.Next(8, 16) * 0.1;    // a double that can be 0.8, 0.9, 1.0, 1.1, ... , 1.5
                    double playerDamage = Math.Floor(Player.getAttack() - attackedEnemy.getDefense() * r);  // damage = attack - defense with a bit of randomness
                    r = random.Next(8, 16) * 0.1;
                    double enemyDamage = Math.Floor(attackedEnemy.getAttack() - Player.getDefense() * r);   // same as the player's attack

                    if (playerDamage < 0) { playerDamage = 0; }         // attack can't be lower than 0, because that would heal the opponent
                    if (enemyDamage < 0) { enemyDamage = 0; }
                    Console.WriteLine($"{i}. round:");
                    Console.BackgroundColor = ConsoleColor.DarkGreen;   // color so the player can easily see who attacked whom
                    Console.Write($"\t{Player.getName()} attacked for {(int)playerDamage}");    // showing attack stats

                    attackedEnemy.setHealth(attackedEnemy.getHealth() - (int)playerDamage);     // extract damage from health
                    playerTotalDamage += (int)playerDamage;             // counting the total damage dealt

                    if (attackedEnemy.getHealth() > 0)                  // enemy won't attack back in the round that it died
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"\n\t{attackedEnemy.getName()} attacked for {(int)enemyDamage}");
                        Player.setHealth(-(int)enemyDamage);
                        enemyTotalDamage += (int)enemyDamage;
                    }
                    Console.ResetColor();
                    i++;
                }
                if (Player.getHealth() > 0)     // player won the fight
                {
                    if (Player.getLevel() < 6)
                    {
                        if (attackedEnemy.getCharacter() == 'E')
                        {
                            Player.LevelUp(1);
                        }
                        if (attackedEnemy.getCharacter() == 'M')
                        {
                            Player.LevelUp(2);  // giving +2 levels for defeating a harder enemy
                        }
                        if (attackedEnemy.getCharacter() == 'H')
                        {
                            Player.LevelUp(3);  // +3 levels for defeating one of the hardest enemies as a low level Player
                        }
                    }
                    else if (Player.getLevel() >= 6 && Player.getLevel() < 11)
                    {
                        if (attackedEnemy.getCharacter() == 'M')
                        {
                            Player.LevelUp(1);
                        }
                        if (attackedEnemy.getCharacter() == 'H')
                        {
                            Player.LevelUp(2);
                        }
                    }
                    else
                    {
                        if (attackedEnemy.getCharacter() == 'H')
                        {
                            Player.LevelUp(1);
                        }
                        if (Player.getLevel() > 99)
                        {
                            Player.setLevel(99);
                        }
                    }
                    // stats after the fight
                    Console.WriteLine("\n\nYOU WON!\n\nYou have dealt a total of {0} damage\nThe enemy dealt a total of {1} damage.\n\n" +
                        "Your stats after the fight: ", playerTotalDamage, enemyTotalDamage);
                    Player.DisplayStats();
                    Console.WriteLine("\nPress Enter to continue.");
                    Console.ReadLine();
                    return true;
                }
                else
                {
                    Menu.DeathMenu();   // death screen
                    Console.ReadKey();
                    return false;
                }
            }

            public static Enemy getAttackedEnemy() { return attackedEnemy; }
            public static void setAttackedEnemy(Enemy e) { attackedEnemy = e; }
        }

        static class Menu
        {// all the possible menus
            public static void StartMenu()      // welcome screen
            {
                Console.Clear();
                Console.WriteLine("   ______            _        _______  _______  _______  _       ");
                Console.WriteLine("  (  __  \\ |\\     /|( (    /|(  ____ \\(  ____ \\(  ___  )( (    /|");
                Console.WriteLine("  | (  \\  )| )   ( ||  \\  ( || (    \\/| (    \\/| (   ) ||  \\  ( |");
                Console.WriteLine("  | |   ) || |   | ||   \\ | || |      | (__    | |   | ||   \\ | |");
                Console.WriteLine("  | |   | || |   | || (\\ \\) || | ____ |  __)   | |   | || (\\ \\) |");
                Console.WriteLine("  | |   ) || |   | || | \\   || | \\_  )| (      | |   | || | \\   |");
                Console.WriteLine("  | (__/  )| (___) || )  \\  || (___) || (____/\\| (___) || )  \\  |");
                Console.WriteLine("  (______/ (_______)|/    )_)(_______)(_______/(_______)|/    )_)");
                Console.WriteLine();
                Console.Write("Name: ");
                Player.setName(Console.ReadLine());         // setting the input as the player's name
                Console.Clear();
                Console.WriteLine("Dear {0}, thank you for playing!\n\n" +
                    "Your character is represented by '{1}'.\nUse the arrow keys to move.\n\nEnemies are represented by 'E', 'M' and 'H'.\n\t'E' means easy (level 1-5)" +
                    "\n\t'M' means medium (level 5-10)\n\t'H' means hard (level 10-20)\n\nYou can quit with the 'x' key." +
                    "\n\nPress Enter to start or 'i' to show high scores.", Player.getName(), Player.getCharacter());
                if(Console.ReadKey().Key == ConsoleKey.I)
                {
                    Game.ShowScore();                       // showing the top scores
                }
            }
            public static bool EnemiesMenu()    // telling the player which enemies can be attacked
            {
                List<Enemy> enemiesNearby = Player.CheckEnemiesNearby(Game.enemy);  // checking for close enemies
                int selectedEnemy = -1;

                Console.WriteLine("{0} enemie(s) nearby:", enemiesNearby.Count);
                for (int i = 0; i < enemiesNearby.Count; i++)    // listing out the enemies
                {
                    Console.WriteLine("\t{0}: Level {1} {2}", i + 1, enemiesNearby[i].getLevel(), enemiesNearby[i].getName());
                }
                Console.Write("If you want to fight, enter the number of the enemy: ");
                
                selectedEnemy = Game.Input();                   // Game.Input handles the number selectors as well, so the player is free to move if decides not to attack
                for (int i = 0; i < enemiesNearby.Count; i++)   // checking if a possible enemy is selected
                {
                    if (selectedEnemy == i + 1)
                    {
                        Fight.setAttackedEnemy(enemiesNearby[selectedEnemy - 1]);   // valid selection -> starting the Fight scene
                        if (Fight.Start())  // if Fight returns true, the player won
                        {
                            int deadEnemyID = enemiesNearby[selectedEnemy - 1].getID();
                            for (int j = 0; j < Game.enemy.Count; j++)
                            {
                                if (Game.enemy[j].getID() == deadEnemyID)
                                {
                                    Playfield.getMap()[Game.enemy[j].getPositionX()][Game.enemy[j].getPositionY()] = '+';   // mark the place where the enemy died
                                    Game.enemy.Remove(Game.enemy[j]);   // removing from the enemy list
                                    Playfield.Draw();
                                    return true;
                                }
                            }
                        }
                        else if (Player.getHealth() > 0) // player fleed (lost the fight, but still alive)
                        {
                            Playfield.Draw();
                            return true;
                        }
                        else                // player died
                        {
                            // save stats
                            Game.SaveScore();
                            // close the game
                            return false;
                        }
                    }
                }
                if (selectedEnemy != 10 && selectedEnemy != 0)   // if selectedEnemy == 10, then the user didn't want to attack; == 0 means 'exit the game'
                {
                    Console.WriteLine("\nThere is no enemy with a number you entered.");
                }
                return true;
            }
            public static void FightMenu()      // drawing the fight scene
            {
                Console.Clear();
                Fight.getAttackedEnemy().DisplayStats();
                Console.WriteLine("\n\n\tPress Enter to start the fight.\nPress 'F' to flee. (You will lose 50% of your health)\n\n");
                Player.DisplayStats();
            }
            public static void DeathMenu()      // after death screen
            {
                Console.Clear();
                Console.WriteLine("\t▀▄    ▄ ████▄   ▄       ██▄   ▄█ ▄███▄   ██▄   ");
                Console.WriteLine("\t  █  █  █   █    █      █  █  ██ █▀   ▀  █  █  ");
                Console.WriteLine("\t   ▀█   █   █ █   █     █   █ ██ ██▄▄    █   █ ");
                Console.WriteLine("\t   █    ▀████ █   █     █  █  ██ █▄   ▄▀ █  █  ");
                Console.WriteLine("\t ▄▀           █▄ ▄█     ███▀   █ ▀███▀   ███▀  ");
                Console.WriteLine("\t               ▀▀▀                             ");
                Console.WriteLine("\n\t\t   Press Enter to continue.");
            }
        }

        static class Player
        {
            private static string name = "John Doe";    // base name
            private static char character = '@';        // character that represents the player
            private static int level = 1;               // base stats
            private static int maxHealth = 80;
            private static int health = 80;
            private static int attack = 8;
            private static int defense = 5;
            private static int[] position = new int[] { Playfield.getFieldHeight() - 2, 1 };
            
            public static List<Enemy> CheckEnemiesNearby(List<Enemy> e)
            {
                List<Enemy> enemiesNearby = new List<Enemy>();
                
                for (int i = 0; i < e.Count; i++)   // cheking every enemy's position relative to the player
                {
                    if((e[i].getPositionX() == Player.getPositionX()) || (e[i].getPositionX() == Player.getPositionX() - 1) || (e[i].getPositionX() == Player.getPositionX() + 1))
                    {
                        if ((e[i].getPositionY() == Player.getPositionY() - 1) || (e[i].getPositionY() == Player.getPositionY()) || (e[i].getPositionY() == Player.getPositionY() + 1))
                        {
                            enemiesNearby.Add(e[i]);    // adding the enemy to the list if it's within 1 cell in any direction
                        }
                    }
                }
                
                return enemiesNearby;
            }
            // getters, setters
            public static char getCharacter()
            {
                return character;
            }
            public static int getLevel()
            {
                return level;
            }
            public static void setLevel(int _level)
            {
                level = _level;
            }
            public static int getHealth()
            {
                return health;
            }
            public static void setHealth(int _health)
            {
                health += _health;
            }
            public static int getAttack()
            {
                return attack;
            }
            public static void setAttack(int _attack)
            {
                attack += _attack;
            }
            public static int getDefense()
            {
                return defense;
            }
            public static void setDefense(int _defense)
            {
                defense += _defense;
            }
            public static int getPositionX()
            {
                return position[0];
            }
            public static int getPositionY()
            {
                return position[1];
            }
            public static void setPosition(int x, int y)
            {
                position[0] = x;
                position[1] = y;
            }
            public static string getName()
            {
                return name;
            }
            public static void setName(string _name)
            {
                name = _name;
            }
            public static void DisplayStats()       // used in the fight scene
            {
                Console.WriteLine("Name: " + name);
                Console.WriteLine("Level: " + level);
                Console.WriteLine("Health: " + health);
                Console.WriteLine("Attack: " + attack);
                Console.WriteLine("Defense: " + defense);
            }
            public static void ResetStats()         // used when the player starts a new game
            {
                name = "John Doe";
                character = '@';
                level = 1;
                maxHealth = 80;
                health = 80;
                attack = 8;
                defense = 5;
                position = new int[] { Playfield.getFieldHeight() - 2, 1 };
            }
            public static void LevelUp(int _level)  // used after the player won a fight
            {
                level += _level;
                maxHealth = (int) Math.Floor(maxHealth * 1.4);
                health = maxHealth;
                attack = (int)Math.Ceiling(attack * 1.4);
                defense = (int)Math.Ceiling(defense * 1.4);
            }
        }
        
        abstract class Enemy
        {// abstract -> all 3 types of enemies derive from this
            protected Random random = new Random();
            protected int id;
            protected string name;
            protected char character = '@';
            protected int level;
            protected int health;
            protected int attack;
            protected int defense;
            protected int[] position;
            protected Enemy(int x, int y, int ID)   // constructor
            {
                position = new int[2] { x, y };     // position stored so we can search for enemies near the player
                id = ID;
            }
            // getters, setters
            public int getID()
            {
                return id;
            }
            public char getCharacter()
            {
                return character;
            }
            public int getLevel()
            {
                return level;
            }
            public int getHealth()
            {
                return health;
            }
            public void setHealth(int Health)
            {
                health = Health;
            }
            public int getAttack()
            {
                return attack;
            }
            public int getDefense()
            {
                return defense;
            }
            public int getPositionX()
            {
                return position[0];
            }
            public int getPositionY()
            {
                return position[1];
            }
            public string getName()
            {
                return name;
            }
            public void DisplayStats()      // used in the fight scene
            {
                Console.WriteLine("\t\t\t\tName: " + name);
                Console.WriteLine("\t\t\t\tLevel: " + level);
                Console.WriteLine("\t\t\t\tHealth: " + health);
                Console.WriteLine("\t\t\t\tAttack: " + attack);
                Console.WriteLine("\t\t\t\tDefense: " + defense);
            }
        }
        
        class EasyEnemy : Enemy
        {
            // predefined name pool -> controlled, flexible and varying names
            private string[] namePool = new string[] { "Blind Crawler", "Crested Beastman", "White Faun", "Pygmy Harpy", "Tundra Howler", "Wandering Biter",
                "Mud Gnoll", "Wood Wisp", "Green Dwarf", "Den Hag", "Blonde Yeti", "Wight Lynx", "Inferior Dryad", "Ice Werewolf", "Whiskered Grendel"};

            public EasyEnemy(int x, int y, int ID) : base(x, y, ID)
            {
                // randomizing stats
                name = namePool[random.Next(namePool.Length)];
                character = 'E';
                level = random.Next(1, 6);
                health = random.Next(10, 35) * level;
                attack = random.Next(1, 5) * level;
                defense = random.Next(1, 3) * level;
            }

        }

        class MediumEnemy : Enemy
        {
            private string[] namePool = new string[] { "Cloaked Tauren", "Light Genie", "Frilled Gargoyle", "Icy Hydra",
                "Mud Terror", "Steel Mummie", "Moon Harpie", "Sunglow Werecat", "Whistling Shadow", "Nomadic Gorgon", "Arctic Nightmare", "Highland Daemon"};

            public MediumEnemy(int x, int y, int ID) : base(x, y, ID)
            {
                name = namePool[random.Next(namePool.Length)];
                character = 'M';
                level = random.Next(5, 11);
                health = random.Next(35, 70) * level;
                attack = random.Next(5, 9) * level;
                defense = random.Next(2, 8) * level;
            }
        }

        class HardEnemy : Enemy
        {
            private string[] namePool = new string[] { "Chaotic Spectre", "Timber Draugr", "Polar Giant", "Violet Valkyrie",
                "Mithril Hippogriff", "Armored Banshee", "Legendary Anubis", "Eerie Chimera", "Moon Soul", "Ash Scourge"};

            public HardEnemy(int x, int y, int ID) : base(x, y, ID)
            {
                name = namePool[random.Next(namePool.Length)];
                character = 'H';
                level = random.Next(10, 21);
                health = random.Next(75, 150) * level;
                attack = random.Next(9, 30) * level;
                defense = random.Next(8, 20) * level;
            }
        }
        
        static class Playfield
        {
            private const int fieldWidth = 50;          // the width of the map (50 characters wide)
            private const int fieldHeight = 25;         // the height of the map (25 characters tall)
            private static char[][] map = new char[fieldHeight][];      // tha map as a double array so coordinates can be used later
            private static Random random = new Random();
            
            public static void Initiate()               // adding the containers of the rows to the map
            {
                for (int i = 0; i < fieldHeight; i++)
                {
                    char[] line = new char[fieldWidth];
                    map[i] = line;
                }
                CreateMap();
            }

            // draws the play field from scratch everytime it's called
            public static void Draw()
            {
                Console.Clear();
                // placing the player character
                map[Player.getPositionX()][Player.getPositionY()] = Player.getCharacter();

                for (int i = 0; i < map.Length; i++)
                {
                    for (int j = 0; j < map[i].Length; j++)
                    {
                        switch (map[i][j])  // setting colors for enemies and the player
                        {
                            case 'E':
                                Console.BackgroundColor = ConsoleColor.DarkGreen;
                                break;
                            case 'M':
                                Console.BackgroundColor = ConsoleColor.DarkYellow;
                                break;
                            case 'H':
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                break;
                            case '@':
                                Console.BackgroundColor = ConsoleColor.DarkGray;
                                break;
                            default:
                                break;
                        }
                        Console.Write(map[i][j]);
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }
            }

            public static void CreateMap()
            {
                for (int i = 0; i < fieldHeight; i++)       // filling up the cells
                {
                    for (int j = 0; j < fieldWidth; j++)
                    {
                        if (i == 0 || i == fieldHeight - 1)
                        {
                            map[i][j] = '_';    // upper and lower walls
                        }
                        else if (j == 0 || j == fieldWidth - 1)
                        {
                            map[i][j] = '|';    // side walls
                        }
                        else
                        {
                            map[i][j] = ' ';    // empty cells that can be overwritten later
                        }
                    }
                }

                // creating the end cell
                map[1][fieldWidth - 2] = 'O';

                // creating bushes: \|/ 
                for (int i = 0; i < fieldWidth * fieldHeight * 0.01; i++)   // number of bushes is relative to the map size
                {
                    int x, y;
                    do
                    {
                        x = random.Next(4, fieldHeight - 4);
                        y = random.Next(4, fieldWidth - 4);
                    } while (!((map[x][y] == ' ') && (map[x][y-1] == ' ') && (map[x][y+1] == ' '))); // searching for 3 empty cells to place '\|/'
                    map[x][y - 1] = '\\';  // placing the bush
                    map[x][y] = '|';
                    map[x][y + 1] = '/';
                }
            }

            public static int getFieldWidth()
            {
                return fieldWidth;
            }
            public static int getFieldHeight()
            {
                return fieldHeight;
            }
            public static char[][] getMap()
            {
                return map;
            }
            public static void setMapCell(int x, int y, char value)
            {
                map[x][y] = value;
            }
        }

        static void Main(string[] args)
        {
            Game.Start();                   // initializing the game
        }
    }
}