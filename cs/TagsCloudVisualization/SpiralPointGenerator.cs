using System.Drawing;

namespace TagsCloudVisualization;

public class SpiralPointGenerator(Point center)
{
    private const double AngleStep = 0.05;
    private const double RadiusStep = 1;
    private double angle;
    private double radius;

    public Point GenerateNextPoint()
    {
        var x = center.X + (int)(radius * Math.Cos(angle));
        var y = center.Y + (int)(radius * Math.Sin(angle));

        angle += AngleStep;
        radius += RadiusStep / (2 * Math.PI);

        return new Point(x, y);
    }
}