using System;
using System.Collections.Generic;
using SQLite;

public class DBinit
{
    public static DBinit instance = null;
    private static SQLiteConnection con = null;

    private DBinit()
    {
        if (con == null)
        {
            con = new SQLiteConnection("MyDB");
            if (con.GetTableInfo("Ville").Count == 0)
            {
                con.CreateTable<Ville>();
            }
            if (con.GetTableInfo("ParamSet").Count == 0)
            {
                con.CreateTable<ParamSet>();
            }
        }
        instance = this;
    }

    public static DBinit GetInstance()
    {
        if (instance == null)
        {
            new DBinit();
        }
        return instance;
    }

    public void SaveVilles(List<Ville> v)
    {
        con.DeleteAll<Ville>();
        con.InsertAll(v);
    }

    public List<Ville> getVilles()
    {
        return con.Table<Ville>().ToList();
    }

    public void SaveParamSet(ParamSet p)
    {
        con.DeleteAll<ParamSet>();
        con.Insert(p);
    }

    public List<ParamSet> getParamSets()
    {
        return con.Table<ParamSet>().ToList();
    }
}