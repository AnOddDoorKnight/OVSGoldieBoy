namespace GoldSplitter;

using System;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Text;

/// <summary>
/// Data specifically about some gold, sometimes split as individual pieces.
/// </summary>
/// <param name="Platinum"> Platinum coins, based in <see cref="platinumRepresentative"/> </param>
/// <param name="Gold"> Gold coins, based in <see cref="goldRepresentative"/> </param>
/// <param name="Silver"> Silver coins, based in <see cref="silverRepresentative"/> </param>
/// <param name="Copper"> Copper coins, based in <see cref="copperRepresentative"/> </param>
public readonly record struct GoldPackage(int Platinum, int Gold, int Silver, int Copper)
{
	/// <summary>
	/// Returns a <see cref="GoldPackage"/> by dispersing the copper.
	/// </summary>
	/// <param name="copper">The amount of copper applied. </param>
	public static GoldPackage FromCopper(int copper)
	{
		return new GoldPackage(0, 0, 0, copper).Disperse();
	}
	/// <summary>
	/// How much copper a copper coin is worth. All or at least most values will 
	/// be based around copper without explicitly mentioning it is based on a value of 1.
	/// </summary>
	public const int copperRepresentative = 1;
	/// <summary>
	/// How much copper a silver coin is worth.
	/// </summary>
	public const int silverRepresentative = 10;
	/// <summary>
	/// How much copper a gold coin is worth.
	/// </summary>
	public const int goldRepresentative = 100;
	/// <summary>
	/// How much copper a platinum coin is worth.
	/// </summary>
	public const int platinumRepresentative = 1000;
	public static GoldPackage operator +(GoldPackage packageLeft, GoldPackage packageRight)
		=> new()
		{
			Platinum = packageLeft.Platinum + packageRight.Platinum,
			Gold = packageLeft.Gold + packageRight.Gold,
			Silver = packageLeft.Silver + packageRight.Silver,
			Copper = packageLeft.Copper + packageRight.Copper,
		};
	/// <summary>
	/// Adds copper though <paramref name="input"/> to the <see cref="GoldPackage"/>
	/// </summary>
	/// <param name="packageLeft"> The package to add to. </param>
	/// <param name="input"> The amount of copper to add. </param>
	public static GoldPackage operator +(GoldPackage packageLeft, int input)
		=> new()
		{
			Platinum = packageLeft.Platinum,
			Gold = packageLeft.Gold,
			Silver = packageLeft.Silver,
			Copper = packageLeft.Copper + input,
		};
	public static GoldPackage operator -(GoldPackage packageLeft, GoldPackage packageRight)
		=> new()
		{
			Platinum = packageLeft.Platinum - packageRight.Platinum,
			Gold = packageLeft.Gold - packageRight.Gold,
			Silver = packageLeft.Silver - packageRight.Silver,
			Copper = packageLeft.Copper - packageRight.Copper,
		};
	public static GoldPackage operator -(GoldPackage packageLeft, int input)
		=> new()
		{
			Platinum = packageLeft.Platinum,
			Gold = packageLeft.Gold,
			Silver = packageLeft.Silver,
			Copper = packageLeft.Copper - input,
		};
	public static GoldPackage operator /(GoldPackage package, int times)
		=> package.Divide(times, out _);
	public static GoldPackage operator *(GoldPackage package, int times)
		=> new(package.Platinum * times, package.Gold * times, package.Silver * times, package.Copper * times);

	public double TotalPlatinum => (double)TotalCopper / platinumRepresentative;
	public double TotalGold => (double)TotalCopper / goldRepresentative;
	public double TotalSilver => (double)TotalCopper / silverRepresentative;
	/// <summary>
	/// All the copper that it has.
	/// </summary>
	public int TotalCopper =>
		(Copper * copperRepresentative) +
		(Silver * silverRepresentative) +
		(Gold * goldRepresentative) +
		(Platinum * platinumRepresentative);

	/// <summary>
	/// Converts all values and their respresentatives into copper, and disperses
	/// them as the highest possible values.
	/// </summary>
	public GoldPackage Disperse()
	{
		int copperTotal = TotalCopper;
		int platinumOut = copperTotal / platinumRepresentative;
		copperTotal -= platinumOut * platinumRepresentative;
		int goldOut = copperTotal / goldRepresentative;
		copperTotal -= goldOut * goldRepresentative;
		int silverOut = copperTotal / silverRepresentative;
		copperTotal -= silverOut * silverRepresentative;
		int copperOut = copperTotal;
		return new GoldPackage(platinumOut, goldOut, silverOut, copperOut);
	}

	public GoldPackage Divide(int divisor)
	{
		return new GoldPackage(0, 0, 0, TotalCopper / divisor).Disperse();
	}
	public GoldPackage Divide(int divisor, out int remainderCopper)
	{
		GoldPackage output = Divide(divisor);
		remainderCopper = TotalCopper - (output.TotalCopper * divisor);
		return output;
	}

	public override string ToString()
	{
		var builder = new StringBuilder($"Gold Package: ");
		if (Platinum != 0)
			builder.Append($"{Platinum} Plat, ");
		if (Gold != 0)
			builder.Append($"{Gold} Gold, ");
		if (Silver != 0)
			builder.Append($"{Silver} Silver, ");
		builder.Append($"{Copper} Copper");
		return builder.ToString();
	}
}