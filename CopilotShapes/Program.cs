// Clear screen (black)
Console.BackgroundColor = ConsoleColor.Black;
Console.CursorVisible = false;
Console.Clear();

// // Draw a cross (X) with width 20 and height 10
// Shape[] shapes = [
//     new Line(new Point2D(0, 0), new Point2D(20, 10), new Color(ConsoleColor.White, ConsoleColor.Black, 'X')),
//     new Line(new Point2D(0, 10), new Point2D(20, 0), new Color(ConsoleColor.White, ConsoleColor.Black, 'X')),
//     new Rectangle(new Point2D(5, 5), new Point2D(15, 15), new Color(ConsoleColor.Red, ConsoleColor.Black, 'R')),
//     new Polygon(new Point2D[] { new Point2D(30, 30), new Point2D(40, 30), new Point2D(35, 40) }, false, new Color(ConsoleColor.Green, ConsoleColor.Black, 'P')),
//     new Triangle(new Point2D(50, 50), new Point2D(60, 50), new Point2D(55, 60), new Color(ConsoleColor.Blue, ConsoleColor.Black, 'T')),
//     new Ellipse(new Point2D(70, 70), 10, 5, 100, new Color(ConsoleColor.Yellow, ConsoleColor.Black, 'E'))
// ];

// // Draw lines
// foreach (var shape in shapes)
// {
//     shape.Draw();
// } 

// // Wait for key press
// Console.ReadKey();

// Clear screen (black)
Console.BackgroundColor = ConsoleColor.Black;
Console.CursorVisible = false;

// Create an ellipse
Ellipse ellipse = new Ellipse(new Point2D(0, 15), 10, 5, 100, new Color(ConsoleColor.White, ConsoleColor.Black, 'E'));

// Animation loop
while (true)
{
    // Clear the console
    Console.Clear();

    // Draw the ellipse
    ellipse.Draw();

    // Delay for a small period of time
    System.Threading.Thread.Sleep(100);

    // Move the ellipse to the right
    ellipse = new Ellipse(new Point2D(ellipse.Center.X + 1, ellipse.Center.Y), 10, 10, 100, new Color(ConsoleColor.White, ConsoleColor.Black, 'E'));

    // If the ellipse reaches the right end of the console, reset its position
    if (ellipse.Center.X >= 100)
    {
        ellipse = new Ellipse(new Point2D(0, ellipse.Center.Y), 10, 10, 100, new Color(ConsoleColor.White, ConsoleColor.Black, 'E'));
    }
}

Console.CursorVisible = true;

record struct Point2D(int X, int Y);
record struct Rect2D(Point2D TopLeft, Point2D BottomRight);
record struct Color(ConsoleColor Foreground, ConsoleColor Background, char Symbol);

// Every shape has a color (stroke). The shape is passed into the ctor. Shapes can be drawn.
abstract class Shape(Color stroke)
{
    protected Color Stroke { get; } = stroke;

    public abstract void Draw();
}

class Line(Point2D start, Point2D end, Color stroke) : Shape(stroke)
{
    public Point2D Start { get; } = start;
    public Point2D End { get; } = end;
    public List<Point2D> Points { get; } = [];

    public override void Draw()
    {
        if (Points.Count == 0)
        {
            // Bresenham's line algorithm
            int x = Start.X;
            int y = Start.Y;
            int dx = Math.Abs(End.X - Start.X);
            int dy = Math.Abs(End.Y - Start.Y);
            int sx = Start.X < End.X ? 1 : -1;
            int sy = Start.Y < End.Y ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                Points.Add(new Point2D(x, y));
                if (x == End.X && y == End.Y)
                {
                    break;
                }

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }
        }
    
        Console.BackgroundColor = Stroke.Background;
        Console.ForegroundColor = Stroke.Foreground;
        foreach (var point in Points)
        {
            if (point.X < 0) { continue; }
            if (point.Y < 0) { continue; }
            Console.SetCursorPosition(point.X, point.Y);
            Console.Write(Stroke.Symbol);
        }
    }
}

// A polygon consists of a list of points. The points are connected with lines.
// A polygon can be open or closed (closed = first and last point are connected).
// Just like with lines, polygons have a stroke (color).
class Polygon(Point2D[] points, bool isOpen, Color stroke) : Shape(stroke)
{
    private List<Line> Lines { get; } = [];
    public Point2D[] Points { get; } = points;
    public bool IsOpen { get; } = isOpen;

    public override void Draw()
    {
        if (Lines.Count == 0)
        {
            for (int i = 0; i < Points.Length - 1; i++)
            {
                Lines.Add(new Line(Points[i], Points[i + 1], Stroke));
            }

            if (!IsOpen)
            {
                Lines.Add(new Line(Points[^1], Points[0], Stroke));
            }
        }

        foreach (var line in Lines)
        {
            line.Draw();
        }
    }
}

// A rectangle is a polygon with 4 points. The points are connected with lines.
class Rectangle(Point2D topLeft, Point2D bottomRight, Color stroke) : Polygon([
    topLeft, new Point2D(bottomRight.X, topLeft.Y), 
    bottomRight, new Point2D(topLeft.X, bottomRight.Y)], false, stroke)
{
    public Rectangle(Rect2D rect, Color stroke) : this(rect.TopLeft, rect.BottomRight, stroke)
    {
    }
}

// A triangle is a polygon with 3 points. The points are connected with lines.
class Triangle(Point2D a, Point2D b, Point2D c, Color stroke) : Polygon([a, b, c], false, stroke)
{
}

class Ellipse(Point2D center, int radiusX, int radiusY, int tessalation, Color stroke) : Shape(stroke)
{
    private Polygon? approximatedEllipse;

    public Point2D Center { get; } = center;

    public override void Draw()
    {
        if (approximatedEllipse == null)
        {
            List<Point2D> points = [];
            double angle = 0;
            double angleStep = 2 * Math.PI / tessalation;
            for (int i = 0; i < tessalation; i++)
            {
                double x = Center.X + radiusX * Math.Cos(angle);
                double y = Center.Y + radiusY * Math.Sin(angle);
                points.Add(new Point2D((int)Math.Round(x), (int)Math.Round(y)));
                angle += angleStep;
            }

            approximatedEllipse = new Polygon([.. points], false, Stroke);
        }

        approximatedEllipse.Draw();
    }

}