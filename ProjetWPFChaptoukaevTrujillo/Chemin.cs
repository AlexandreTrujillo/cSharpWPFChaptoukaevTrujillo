using System;
using System.Collections.Generic;

public class Chemin
{
    public List<Ville> listeVilles = new List<Ville>();
    public double score;
    public int generation = 1;

    public Chemin(params Ville[] ville)
    {
        listeVilles.AddRange(ville);
    }


    public Chemin(int generation, params Ville[] ville) : this(ville)
    {
        this.generation = generation;
    }


    public double CalculScore()
    {
        if (listeVilles.Count < 2)
        {
            Console.WriteLine("Un chemin est défini avec au moins deux villes !");
        }
        else
        {
            for (int i = 0; i < listeVilles.Count - 1; i++)
            {
                score = score + Math.Sqrt(Math.Pow(listeVilles[i+1].posX - listeVilles[i].posX, 2) + Math.Pow(listeVilles[i+1].posY - listeVilles[i].posY, 2));
            }
            score = score + Math.Sqrt(Math.Pow(listeVilles[0].posX - listeVilles[listeVilles.Count -1].posX, 2) + Math.Pow(listeVilles[0].posY - listeVilles[listeVilles.Count - 1].posY, 2));
        }
        return score;
    }

    public override string ToString()
    {
        string affichageVilles = "";
        score = 0;
        foreach(Ville ville in listeVilles)
        {
            affichageVilles = affichageVilles + ville + " ";
        }
        return affichageVilles;
    }
}