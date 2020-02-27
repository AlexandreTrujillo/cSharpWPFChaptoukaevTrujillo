using System;
using SQLite;

[Table("Ville")]
public class Ville {

    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    [Column("Nom")]
    public string nomVille { get; set; }
    [Column("x")]
    public double posX { get; set; }
    [Column("y")]
    public double posY { get; set; }

    public Ville()
    {

    }

    public Ville(int id, string nom, double x, double y)
    {
        ID = id;
        nomVille = nom;
        posX = x;
        posY = y;
    }

    public Ville(string nom, double x, double y)
    {
        nomVille = nom;
        posX = x;
        posY = y;
    }

    public override string ToString()
    {
        return nomVille;
    }
}