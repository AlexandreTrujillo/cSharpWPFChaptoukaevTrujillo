using System;
using SQLite;

[Table("ParamSet")]
public class ParamSet
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    [Column("Npop")]
    public int Npop { get; set; }
    [Column("nXover")]
    public int nXover { get; set; }
    [Column("nMuntant")]
    public int nMutant { get; set; }
    [Column("nElite")]
    public int nElite { get; set; }
    [Column("stop")]
    public int stop { get; set; }

    public ParamSet()
    {

    }

    public ParamSet(int id, int N, int nX, int nM, int nE, int s)
    {
        ID = id;
        Npop = N;
        nXover = nX;
        nMutant = nM;
        nElite = nE;
        stop = s;
    }

    public ParamSet(int N, int nX, int nM, int nE, int s)
    {
        Npop = N;
        nXover = nX;
        nMutant = nM;
        nElite = nE;
        stop = s;
    }

    public override string ToString()
    {
        return Npop + " " + nXover + " " + nMutant + " " + nElite + " " + stop;
    }
}

