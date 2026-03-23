using Planclap.Client.Presentations.IViewModel;

namespace Planclap.Client.Presentations.View;

public class DummyShowDetailViewModel : IShowDetailViewModel
{
    private static readonly string ImgExtension = "png";
    private static readonly string ImagePath = Path.Combine("Resources", "Images");

    public IReadOnlyList<string> Tags =>
    [
        Path.Combine(ImagePath, $"12.{ImgExtension}"),
        Path.Combine(ImagePath, $"discrimination.{ImgExtension}"),
        Path.Combine(ImagePath, $"sex.{ImgExtension}")
    ];

    public string Poster => "https://theposterdb.com/api/assets/468365/view";

    public string Title => "Dummy Show Detail";

    public TimeSpan Duration => TimeSpan.FromMinutes(120);

    public string Description =>
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer volutpat non enim eu consequat. Maecenas sed volutpat elit. Nam luctus ut sapien ut commodo. Aenean aliquam arcu eu pulvinar iaculis.";
}
