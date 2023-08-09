using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPatternLearning
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (IList list = new ListOfEnemiesProxy())
            {
                // читаем первую страницу
                Enemy enemy1 = list.GetEnemy("Baba-Yaga");
                Console.WriteLine(enemy1.Description);
                // читаем вторую страницу
                Enemy enemy2 = list.GetEnemy("Leshii");
                Console.WriteLine(enemy2.Description);
                // возвращаемся на первую страницу    
                Enemy enemy3 = list.GetEnemy("Domovoi");
                Console.WriteLine(enemy3.Description);
            }

            Console.Read();
        }
    }
    class Enemy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    class EnemyContext : DbContext
    {
        public DbSet<Enemy> Enemies { get; set; }
    }
    //subject
    interface IList : IDisposable
    {
        Enemy GetEnemy(string name);
    }
    //real subject
    class ListOfEnemies : IList
    {
        EnemyContext enemyDB;
        public ListOfEnemies()
        {
            enemyDB = new EnemyContext();
        }
        public Enemy GetEnemy(string name)
        {
            return enemyDB.Enemies.FirstOrDefault(e => e.Name == name);
        }

        public void Dispose()
        {
            enemyDB.Dispose();
        }
    }
    //proxy
    class ListOfEnemiesProxy : IList
    {
        List<Enemy> enemies;
        ListOfEnemies listOfEnemies;
        public ListOfEnemiesProxy()
        {
            enemies = new List<Enemy>();
        }
        public Enemy GetEnemy(string name)
        {
            Enemy enemy = enemies.FirstOrDefault(e => e.Name == name);
            if (enemy == null)
            {
                if (listOfEnemies == null)
                    listOfEnemies = new ListOfEnemies();
                enemy = listOfEnemies.GetEnemy(name);
                enemies.Add(enemy);
            }
            return enemy;
        }

        public void Dispose()
        {
            listOfEnemies?.Dispose();
        }
    }
}
