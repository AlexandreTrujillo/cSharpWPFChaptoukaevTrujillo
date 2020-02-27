using System;

public class Metrics
{
	public int id { get; set; }
	public double moyenne { get; set; }
	public double meilleur { get; set; }

	public Metrics()
	{
	}

	public Metrics(int id, double moy, double best)
	{
		this.id = id;
		this.moyenne = moy;
		this.meilleur = best;
	}
}
