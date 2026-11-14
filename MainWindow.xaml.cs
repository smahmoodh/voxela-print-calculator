using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Voxela3D.PrintCalculator;

public partial class MainWindow : Window
{
    private AppSettings _settings = SettingsStore.Load();
    private int _selectedIndex = -1;
    private static readonly CultureInfo Fa = CultureInfo.GetCultureInfo("fa-IR");

    public MainWindow()
    {
        InitializeComponent();
        HourlyRateBox.Text = _settings.HourlyRate.ToString(CultureInfo.InvariantCulture);
        ProfitPercentBox.Text = _settings.ProfitPercent.ToString(CultureInfo.InvariantCulture);
        RefreshFilaments();
        if (FilamentCombo.Items.Count > 0) FilamentCombo.SelectedIndex = 0;
    }

    private static bool TryDecimal(string? input, out decimal value)
    {
        input = (input ?? "").Trim().Replace("٫", ".").Replace("٬", "").Replace(",", "");
        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    private static string Money(decimal value) => value.ToString("#,##0.##", Fa) + " تومان";

    private void RefreshFilaments()
    {
        FilamentCombo.ItemsSource = null;
        FilamentCombo.ItemsSource = _settings.Filaments.ToList();
        FilamentList.ItemsSource = null;
        FilamentList.ItemsSource = _settings.Filaments.ToList();
    }

    private void FilamentCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

    private void Calculate_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = "";
        if (FilamentCombo.SelectedItem is not Filament filament) { ErrorText.Text = "ابتدا فیلامنت را انتخاب کنید."; return; }
        if (!TryDecimal(WeightBox.Text, out var weight) || weight < 0) { ErrorText.Text = "وزن باید یک عدد معتبر و غیرمنفی باشد."; return; }
        if (!TryDecimal(HoursBox.Text, out var hours)) hours = 0;
        if (!TryDecimal(MinutesBox.Text, out var minutes)) minutes = 0;
        if (hours < 0 || minutes < 0) { ErrorText.Text = "زمان نمی‌تواند منفی باشد."; return; }
        if (!TryDecimal(HourlyRateBox.Text, out var hourly) || hourly < 0) { ErrorText.Text = "نرخ ساعتی در تنظیمات معتبر نیست."; return; }
        if (!TryDecimal(ProfitPercentBox.Text, out var percent) || percent < 0) { ErrorText.Text = "درصد سود در تنظیمات معتبر نیست."; return; }

        decimal material = filament.Unit == "kg" ? filament.Price / 1000m * weight : filament.Price * weight;
        decimal time = hours * hourly + minutes * hourly / 60m;
        decimal subtotal = material + time;
        decimal profit = subtotal * percent / 100m;
        MaterialCostText.Text = Money(material);
        TimeCostText.Text = Money(time);
        SubtotalText.Text = Money(subtotal);
        ProfitText.Text = Money(profit);
        FinalText.Text = Money(subtotal + profit);
    }

    private void SaveSettings_Click(object sender, RoutedEventArgs e)
    {
        if (!TryDecimal(HourlyRateBox.Text, out var rate) || rate < 0 ||
            !TryDecimal(ProfitPercentBox.Text, out var profit) || profit < 0)
        { SaveStatus.Text = "مقادیر نرخ و سود باید عدد معتبر و غیرمنفی باشند."; return; }
        _settings.HourlyRate = rate;
        _settings.ProfitPercent = profit;
        Persist();
        SaveStatus.Text = "تنظیمات ذخیره شد.";
    }

    private void Persist()
    {
        SettingsStore.Save(_settings);
        RefreshFilaments();
        if (_selectedIndex >= 0 && _selectedIndex < FilamentList.Items.Count) FilamentList.SelectedIndex = _selectedIndex;
    }

    private void FilamentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedIndex = FilamentList.SelectedIndex;
        if (FilamentList.SelectedItem is not Filament f) return;
        FilamentNameBox.Text = f.Name; BrandBox.Text = f.Brand; TypeBox.Text = f.Type;
        PriceBox.Text = f.Price.ToString(CultureInfo.InvariantCulture);
        UnitBox.SelectedIndex = f.Unit == "g" ? 1 : 0;
    }

    private bool ReadFilament(out Filament filament)
    {
        filament = new Filament();
        if (string.IsNullOrWhiteSpace(FilamentNameBox.Text) || !TryDecimal(PriceBox.Text, out var price) || price < 0)
        { MessageBox.Show("نام و قیمت معتبر را وارد کنید.", "Voxela 3D", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
        filament = new Filament { Name = FilamentNameBox.Text.Trim(), Brand = BrandBox.Text.Trim(),
            Type = TypeBox.Text.Trim(), Price = price, Unit = (UnitBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "kg" };
        return true;
    }

    private void AddFilament_Click(object sender, RoutedEventArgs e)
    {
        if (!ReadFilament(out var f)) return;
        _settings.Filaments.Add(f); _selectedIndex = _settings.Filaments.Count - 1; Persist();
    }

    private void UpdateFilament_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedIndex < 0 || _selectedIndex >= _settings.Filaments.Count) { MessageBox.Show("ابتدا یک فیلامنت را انتخاب کنید."); return; }
        if (!ReadFilament(out var f)) return;
        _settings.Filaments[_selectedIndex] = f; Persist();
    }

    private void DeleteFilament_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedIndex < 0 || _selectedIndex >= _settings.Filaments.Count) return;
        if (MessageBox.Show("این فیلامنت حذف شود؟", "تأیید حذف", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
        _settings.Filaments.RemoveAt(_selectedIndex); _selectedIndex = -1; Persist();
    }
}