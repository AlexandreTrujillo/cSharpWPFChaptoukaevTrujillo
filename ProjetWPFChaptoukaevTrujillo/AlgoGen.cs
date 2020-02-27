using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;

public class AlgoGen
{
    public AlgoGen()
    {

    }

    //Génération 1: On crée n chemins aléatoires à partir de notre liste de
    //villes initiales.
    public List<Chemin> CheminsAleatoires(int N, List<Ville> cheminInitial)
    {
        List<Chemin> lesChemins = new List<Chemin>();

        for (int i = 0; i < N; i++)
        {
            Chemin cheminRandom = new Chemin();
            cheminRandom.listeVilles = cheminInitial.OrderBy(x => Guid.NewGuid()).ToList();
            lesChemins.Add(cheminRandom);
        }
        return lesChemins;
    }



    //Crossover: On créé n x N nouveaux chemins Xover à partir de la generation precedente, en
    //créant à chaque fois un chemin qui prend une premiere partie de la liste de villes d'un parent 1
    //et une autre partie d'un parent 2.
    public List<Chemin> Xover(int n, List<Chemin> generationPrecedente, List<Ville> listeVilles)
    {
        //Constitura les Xover candidats pour la nouvelle generation 
        List<Chemin> Xover = new List<Chemin>();

        //On crée parent1: c'est une liste de chemins crées à partir de la liste
        //de chemins de la generation precedente, de taille n x plus grande.
        List<Chemin> parent1 = new List<Chemin>();
        for (int i = 0; i < n; i++)
        {
            foreach (Chemin chemin in generationPrecedente)
            {
                parent1.Add(new Chemin(chemin.generation, chemin.listeVilles.ToArray()));
            }
        }
        parent1 = parent1.OrderBy(x => Guid.NewGuid()).ToList();

        //Pour chacun des chemins de parent1, on tire aléatoirement un parent2 dans la liste de
        //la génération précédente. 
        foreach (Chemin ch in parent1)
        {
            Random rd = new Random();
            int index = rd.Next(generationPrecedente.Count);
            Chemin parent2 = new Chemin(generationPrecedente[index].generation, generationPrecedente[index].listeVilles.ToArray());

            //On crée un nouveau chemin (liste de villes) en prenant une partie de parent1
            //et une partie de parent2. On choisit un pivot aléatoire.
            int pivot = rd.Next(1, generationPrecedente.Count - 1);
            List<Ville> list1 = ch.listeVilles.Take(pivot).ToList();
            List<Ville> list2 = parent2.listeVilles.Skip(pivot).ToList();
            list1.AddRange(list2);

            // On vérifie que notre nouvelle liste ne contient qu'une seule fois chaque ville.
            //On recupere les valeurs distinctes de notre liste, on compare ensuite avec notre liste de
            //villes initiales, et s'il manque une ville on l'ajoute à notre nouvelle liste.
            List<Ville> newListe = list1.Distinct().ToList();
            foreach (Ville v in listeVilles)
            {
                if (!newListe.Contains(v))
                {
                    newListe.Add(v);
                }
            }
            //On crée le nouveau chemin à partir de notre nouvelle liste de villes.
            Chemin newChemin = new Chemin(ch.generation, newListe.ToArray());

            //On l'ajoute à la nouvelle liste de chemins correspondant au Xover de la génération +1.
            Xover.Add(newChemin);
        }
        return Xover;
    }



    //Mutation: On permute deux villes dans un chemin (liste de villes).
    public List<Chemin> Mutate(int m, List<Chemin> generationPrecedente)
    {
        //Constituera les mutants candidats pour la nouvelle generation.
        List<Chemin> mutants = new List<Chemin>();

        //On crée longList: c'est une liste de chemins crées à partir de la liste
        //de chemins de la generation precedente, de taille m x plus grande.
        List<Chemin> longList = new List<Chemin>();
        for (int i = 0; i < m; i++)
        {
            foreach (Chemin chemin in generationPrecedente)
            {
                longList.Add(new Chemin(chemin.generation, chemin.listeVilles.ToArray()));
            }
        }

        //Pour chaque liste de villes de longList, on swap deux éléments.
        foreach (Chemin ch in longList)
        {
            Chemin newChemin = new Chemin(ch.generation, ch.listeVilles.ToArray());
            //On choisit aléatoirement deux éléments de la liste.
            Random rd = new Random();
            int idx1 = rd.Next(ch.listeVilles.Count);
            int idx2 = rd.Next(ch.listeVilles.Count);

            //On vérifie qu'on ait pas tiré deux fois la même ville de la liste.
            while (idx1 == idx2)
            {
                rd = new Random();
                idx2 = rd.Next(ch.listeVilles.Count);
            }

            //On intervertit les deux 
            Ville tmp = newChemin.listeVilles[idx1];
            newChemin.listeVilles[idx1] = newChemin.listeVilles[idx2];
            newChemin.listeVilles[idx2] = tmp;

            //On l'ajoute à la nouvelle liste de chemins correspondant aux mutants de la génération +1.
            mutants.Add(newChemin);
        }
        return mutants;
    }



    //Elitisme: Parmis les chemins de la generation précédente on veut selectionner ceux qui ont
    //le meilleur score, pour qu'ils soient directement portés à la génération +1.
    public List<Chemin> SelectElite(int p, List<Chemin> generationPrecedente)
    {
        //Constituera les elites candidats pour la nouvelle generation.
        List<Chemin> eliteToKeep = new List<Chemin>();

        //On calcul le score pour chaque chemin de la generation entrée en parametre, et on
        //en garde p
        var elite = (from ch in generationPrecedente
                     orderby ch.CalculScore() ascending
                     select ch).Take(p);

        //On les ajoute à la liste de chemins de la nouvelle generation.
        foreach (Chemin ch in elite)
        {
            Chemin el = new Chemin(ch.generation, ch.listeVilles.ToArray());
            eliteToKeep.Add(el);
        }
        //On retroune la liste de chemins gagnants.
        return (eliteToKeep.ToList());
    }



    //Next generation: elle doit etre constituee de N chemins -taille de la population initiale-
    //et elle doit contenir les meilleurs chemins de la population precedente Xover, mutation et elite confondus.
    public List<Chemin> NextGen(int N, int nXover, int nMutant, int nElite, List<Ville> listeVilles, List<Chemin> generationPrecedente)
    {
        //Consituera la nouvelle generation.
        List<Chemin> nextGen = new List<Chemin>();

        //On réunit les chemins du Xover, de la mutation et de l'élitisme dans une unique liste.
        List<Chemin> xover = Xover(nXover, generationPrecedente, listeVilles);
        List<Chemin> mutants = Mutate(nMutant, generationPrecedente);
        List<Chemin> elite = SelectElite(nElite, generationPrecedente);
        /*
        Console.WriteLine("XOVER");
        foreach (Chemin ch in xover)
        {
            Console.WriteLine(ch);
        }
        Console.WriteLine("MUTANTS");
        foreach (Chemin ch in mutants)
        {
            Console.WriteLine(ch);
        }
        Console.WriteLine("ELITE");
        foreach (Chemin ch in elite)
        {
            Console.WriteLine(ch);
        }
        */
        //On selectionne les N chemins qui ont le meilleur score. Note: pour que le score d'un chemin soit
        //accessible il faut d'abord appeler calculScore().
        xover.AddRange(mutants);
        xover.AddRange(elite);

        var bestToKeep = (from chemin in xover
                          orderby chemin.CalculScore() ascending
                          select chemin).Take(N);

        nextGen.AddRange(bestToKeep.ToList());

        //On incrémente le numero de generation de chaque chemin.
        foreach (Chemin ch in nextGen)
        {
            ch.generation++;
        }
        /*
        Console.WriteLine("NEXTGEN");
        foreach (Chemin ch in nextGen)
        {
            Console.WriteLine(ch);
        }
        */
        return nextGen;
    }



    //Evolution: on repete NextGen jusqu'a convergence. Pour la condition d'arret on peut par exemple s'arreter
    //quand 30 generations successives ont eu le meme score.
    public Chemin Evolution(ParamSet p, List<Ville> listeVilles, List<Chemin> generationPrecedente, List<Metrics> metrics)
    {
        //Sera la sortie de l'algorithme.
        List<Chemin> CurrentGen = new List<Chemin>();
        //On initialise l'lagorithme avec la generation 1.
        List<Chemin> PreviousGen = generationPrecedente;
        List<double> bestScores = new List<double>();

        //On va l'utiliser pour afficher le numero de chaque genration.
        int i = 1;
        //On va l'utiliser comme variable d'accumulation.
        int acc = 0;

        //On stocke la moyenne des scores de la generation 1. Note: pour que le score d'un chemin soit
        //accessible il faut d'abord appeler calculScore().
        var average = (from chemin in PreviousGen
                       select chemin.CalculScore()).Average();
        average = Math.Round(average, 2);

        //On stocke le meilleur score de la generation 1.
        var bestScoreArray = (from chemin in PreviousGen
                              orderby chemin.score ascending
                              select chemin.score).Take(1).ToArray();
        double bestScore = Math.Round(bestScoreArray[0], 2);
        bestScores.Add(bestScore);
        //On remplit la liste metrics initialisée dans le main et prise en parametres.
        metrics.Add(new Metrics(i, average, bestScore));

        while (acc < p.stop)
        {
            CurrentGen = NextGen(p.Npop, p.nXover, p.nMutant, p.nElite, listeVilles, PreviousGen);
            PreviousGen = CurrentGen;
            i++;

            //On garde la moyenne des score de chaque génération. Note: ici calculScore() a deja été appelé
            //dans la fonction nextGen().
            average = (from chemin in PreviousGen
                       select chemin.score).Average();
            average = Math.Round(average, 2);

            //On garde le meilleur score de chaque génération.
            bestScoreArray = (from chemin in PreviousGen
                              orderby chemin.score ascending
                              select chemin.score).Take(1).ToArray();
            bestScore = Math.Round(bestScoreArray[0], 2);
            bestScores.Add(bestScore);

            //On calcule la difference entre le meilleur score de la generation actuelle et le meilleur score de
            //la generation precedente. S'il est inferieur à 0.01, acc devient acc+1.
            double dist = Math.Abs(bestScores.ElementAt(i - 2) - bestScores.ElementAt(i - 1));
            if (dist < 0.01)
            {
                acc++;
            }
            else
            {
                acc = 0;
            }
            //On remplit la liste metrics initialisée dans le main et prise en parametres.
            metrics.Add(new Metrics(i, average, bestScore));
        }

        //On garde un unique chemin - celui qui a le score le plus elevé dans la toute derniere generation. 
        var winnerList = (from chemin in PreviousGen
                          orderby chemin.score ascending
                          select chemin.listeVilles).Take(1).ToArray();
        Chemin winner = new Chemin(i, winnerList[0].ToArray());
        return winner;
    }
}


