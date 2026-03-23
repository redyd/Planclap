using System.Globalization;
using AsyncImageLoader.Loaders;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Planclap.Client.Views.Converters;

public class ImageConverter : IValueConverter
{
    private const string BaseUriStr = "avares://Planclap.Client.Views/";
    private const string DefaultImagePath = "Resources/Images/al.png";

    private static readonly Uri BaseUri = new(BaseUriStr, UriKind.Absolute);
    private static readonly Uri DefaultImageUri = new($"{BaseUriStr}{DefaultImagePath}");

    private static readonly RamCachedWebImageLoader ImageLoader = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            return value switch
            {
                string s when IsHttpUrl(s) =>
                    ImageLoader.ProvideImageAsync(s).GetAwaiter().GetResult(),

                string s when s.StartsWith(BaseUriStr, StringComparison.OrdinalIgnoreCase) =>
                    new Bitmap(AssetLoader.Open(new Uri(s, UriKind.Absolute))),

                string s when !string.IsNullOrWhiteSpace(s) =>
                    new Bitmap(AssetLoader.Open(new Uri(s, UriKind.Relative), BaseUri)),

                Uri u when IsHttpUrl(u.AbsoluteUri) =>
                    ImageLoader.ProvideImageAsync(u.AbsoluteUri).GetAwaiter().GetResult(),

                Uri { IsAbsoluteUri: true } u =>
                    new Bitmap(AssetLoader.Open(u)),

                Uri u =>
                    new Bitmap(AssetLoader.Open(u, BaseUri)),

                _ =>
                    new Bitmap(AssetLoader.Open(DefaultImageUri))
            };
        }
        catch
        {
            return new Bitmap(AssetLoader.Open(DefaultImageUri));
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => BindingOperations.DoNothing;

    private static bool IsHttpUrl(string s) =>
        s.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
        s.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
}
