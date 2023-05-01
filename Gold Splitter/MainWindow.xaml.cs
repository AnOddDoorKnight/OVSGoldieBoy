namespace GoldSplitter;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public static bool TryParseInt(string input, out int value)
	{
		if (string.IsNullOrEmpty(input))
		{
			value = 0;
			return true;
		}
		if (!BigInteger.TryParse(input, out var bigInt))
		{
			value = 0;
			return false;
		}
		if (bigInt < int.MinValue)
			value = int.MinValue;
		else if (bigInt > int.MaxValue)
			value = int.MaxValue;
		else
			value = int.Parse(input);
		return true;
	}


	public GoldPackage Package
	{
		get => package;
		set
		{
			package = value;
			if (IsInitialized)
			{
				Total_Platinum.Text = package.Platinum.ToString();
				Total_Gold.Text = package.Gold.ToString();
				Total_Silver.Text = package.Silver.ToString();
				Total_Copper.Text = package.Copper.ToString();
			}
		}
	}
	private GoldPackage package = new(0, 0, 0, 0);
	private readonly Grid[] subGrids;

	public MainWindow()
	{
		UpdatePanel = UpdateDivideByPanel;
		InitializeComponent();
		subGrids = new Grid[]
		{
			Grid_DivideBy,
			Grid_Add,
		};
		OnDivideByButtonClick(this, null);
		Update();
	}
	private Action UpdatePanel;
	private void Update()
	{
		if (Total is not null)
			Total.Content = $"In Total: " +
				$"\t{Package.TotalPlatinum} Platinum\n" +
				$"\t{Package.TotalGold} Gold\n" +
				$"\t{Package.TotalSilver} Silver\n" +
				$"\t{Package.TotalCopper} Copper";
		if (DispersedTotal is not null)
		{
			GoldPackage dispersedPackage = Package.Disperse();
			DispersedTotal.Content = $"Dispersed:\n" +
				$"\t{dispersedPackage.Platinum} Platinum\n" +
				$"\t{dispersedPackage.Gold} Gold\n" +
				$"\t{dispersedPackage.Silver} Silver\n" +
				$"\t{dispersedPackage.Copper} Copper";
		}

		UpdatePanel?.Invoke();
	}

	#region Changed Values
	private void TotalPlatinumChanged(object sender, TextChangedEventArgs e)
	{
		string text = (e.Source as TextBox)?.Text ?? "";
		if (TryParseInt(text, out int value))
			Package = new GoldPackage(value, Package.Gold, Package.Silver, Package.Copper);
		Update();
	}
	private void TotalGoldChanged(object sender, TextChangedEventArgs e)
	{
		string text = (e.Source as TextBox)?.Text ?? "";
		if (TryParseInt(text, out int value))
			Package = new GoldPackage(Package.Platinum, value, Package.Silver, Package.Copper);
		Update();
	}
	private void TotalSilverChanged(object sender, TextChangedEventArgs e)
	{
		string text = (e.Source as TextBox)?.Text ?? "";
		if (TryParseInt(text, out int value))
			Package = new GoldPackage(Package.Platinum, Package.Gold, value, Package.Copper);
		Update();
	}
	private void TotalCopperChanged(object sender, TextChangedEventArgs e)
	{
		string text = (e.Source as TextBox)?.Text ?? "";
		if (TryParseInt(text, out int value))
			Package = new GoldPackage(Package.Platinum, Package.Gold, Package.Silver, value);
		Update();
	}
	#endregion

	#region Divide By Panel
	private void OnDivideByButtonClick(object sender, RoutedEventArgs? e)
	{
		for (int i = 0; i < subGrids.Length; i++)
			subGrids[i].Visibility = i == 0 ? Visibility.Visible : Visibility.Hidden;
		UpdatePanel = UpdateDivideByPanel;
		Update();
	}

	private string? exampleDivideByText = null;
	private string? exampleDivideByTextRemainder = null;
	private void ChangePeople_DivideBy(object sender, TextChangedEventArgs e) => UpdateDivideByPanel();
	private void UpdateDivideByPanel()
	{
		if (TextBlock_DivideBy == null)
			return;
		if (TextBlock_DivideBy_Remainder == null)
			return;
		string text = People_DivideBy.Text ?? "";
		if (!TryParseInt(text, out int people) || people == 0)
			return;
		GoldPackage package = Package.Divide(people, out int remainder);
		exampleDivideByText ??= TextBlock_DivideBy.Text;
		exampleDivideByTextRemainder ??= TextBlock_DivideBy_Remainder.Text;
		TextBlock_DivideBy.Text = exampleDivideByText
			// Platinum
			.Replace("{0}", package.Platinum.ToString())
			// Gold
			.Replace("{1}", package.Gold.ToString())
			// Silver
			.Replace("{2}", package.Silver.ToString())
			// Copper
			.Replace("{3}", package.Copper.ToString());

		GoldPackage remainingPackage = GoldPackage.FromCopper(remainder);
		TextBlock_DivideBy_Remainder.Text = exampleDivideByTextRemainder
			// Platinum
			.Replace("{0}", remainingPackage.Platinum.ToString())
			// Gold
			.Replace("{1}", remainingPackage.Gold.ToString())
			// Silver
			.Replace("{2}", remainingPackage.Silver.ToString())
			// Copper
			.Replace("{3}", remainingPackage.Copper.ToString());
	}
	#endregion

	private GoldPackage AddPackage { get; set; } = new(0, 0, 0, 0);
	private GoldPackage Result => add ? Package + AddPackage : Package - AddPackage;
	private bool add = true;

	private void AddByButtonClick(object sender, RoutedEventArgs e)
	{
		add = true;
		for (int i = 0; i < subGrids.Length; i++)
			subGrids[i].Visibility = i == 1 ? Visibility.Visible : Visibility.Hidden;
		UpdatePanel = UpdateAddPanel;
		Update();
	}
	private void RemoveByButtonClick(object sender, RoutedEventArgs e)
	{
		add = false;
		for (int i = 0; i < subGrids.Length; i++)
			subGrids[i].Visibility = i == 1 ? Visibility.Visible : Visibility.Hidden;
		UpdatePanel = UpdateAddPanel;
		Update();
	}
	private void UpdateAddPanel()
	{
		if (AddTotal is not null)
			AddTotal.Content = $"In Add Total: " +
				$"\t{AddPackage.TotalPlatinum} Platinum\n" +
				$"\t{AddPackage.TotalGold} Gold\n" +
				$"\t{AddPackage.TotalSilver} Silver\n" +
				$"\t{AddPackage.TotalCopper} Copper";
		if (AddTotalTotal is not null)
		{
			GoldPackage addedPackage = Result;
			addedPackage = addedPackage.Disperse();
			AddTotalTotal.Content = $"Out: " +
				$"\t{addedPackage.Platinum} Platinum\n" +
				$"\t{addedPackage.Gold} Gold\n" +
				$"\t{addedPackage.Silver} Silver\n" +
				$"\t{addedPackage.Copper} Copper";
		}
	}
	private void AddPlatinumChanged(object sender, TextChangedEventArgs e)
	{
		string text = (e.Source as TextBox)?.Text ?? "";
		if (TryParseInt(text, out int value))
			AddPackage = new GoldPackage(value, AddPackage.Gold, AddPackage.Silver, AddPackage.Copper);
		Update();
	}
	private void AddGoldChanged(object sender, TextChangedEventArgs e)
	{
		string text = (e.Source as TextBox)?.Text ?? "";
		if (TryParseInt(text, out int value))
			AddPackage = new GoldPackage(AddPackage.Platinum, value, AddPackage.Silver, AddPackage.Copper);
		Update();
	}
	private void AddSilverChanged(object sender, TextChangedEventArgs e)
	{
		string text = (e.Source as TextBox)?.Text ?? "";
		if (TryParseInt(text, out int value))
			AddPackage = new GoldPackage(AddPackage.Platinum, AddPackage.Gold, value, AddPackage.Copper);
		Update();
	}
	private void AddCopperChanged(object sender, TextChangedEventArgs e)
	{
		string text = (e.Source as TextBox)?.Text ?? "";
		if (TryParseInt(text, out int value))
			AddPackage = new GoldPackage(AddPackage.Platinum, AddPackage.Gold, AddPackage.Silver, value);
		Update();
	}
	private void SendAddToInput(object sender, RoutedEventArgs e)
	{
		Package = Result;
		AddPackage = new(0, 0, 0, 0);
	}
}
