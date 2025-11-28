using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudVisualization;

public class Visualizer
{
    private readonly Font font = new("Times New Roman", 16);

    public void DrawCloud(
        IEnumerable<string> words,
        string outputPath,
        int imageWidth,
        int imageHeight)
    { 
        var bitmap = new Bitmap(imageWidth, imageHeight);
        var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.FromArgb(0, 28, 39));

        var center = new Point(imageWidth / 2, imageHeight / 2);
        var layouter = new CircularCloudLayouter(center);
        var textBrush = new SolidBrush(Color.FromArgb(255, 102, 0));

        foreach (var word in words)
        {
            var size = MeasureWord(word, graphics);
            var rectangle = layouter.PutNextRectangle(size);
            
            graphics.DrawString(word, font, textBrush, rectangle.Location);
        }

        bitmap.Save(outputPath, ImageFormat.Png);
    }

    private Size MeasureWord(string word, Graphics g)
    {
        var size = g.MeasureString(word, font);
        return new Size((int)size.Width, (int)size.Height);
    }
}