using System;
using System.Collections.Generic;

public static class ShadowCaster
{
    // Takes a circle in the form of a center point and radius, and a function that
    // can tell whether a given cell is opaque. Calls the setFoV action on
    // every cell that is both within the radius and visible from the center. 

    public static void ComputeFieldOfViewWithShadowCasting(int x, int y, int radius, Func<int, int, bool> isOpaque, Action<int, int> setFoV)
    {
        Func<int, int, bool> opaque = TranslateOrigin(isOpaque, x, y);
        Action<int, int> fov = TranslateOrigin(setFoV, x, y);

        for (int octant = 0; octant < 8; ++octant)
        {
            ComputeFieldOfViewInOctantZero(
                TranslateOctant(opaque, octant),
                TranslateOctant(fov, octant),
                radius);
        }
    }

    private static void ComputeFieldOfViewInOctantZero(Func<int, int, bool> isOpaque, Action<int, int> setFieldOfView, int radius)
    {
        var queue = new Queue<ColumnPortion>();
        queue.Enqueue(new ColumnPortion(0, new DirectionVector(1, 0), new DirectionVector(1, 1)));
        while (queue.Count != 0)
        {
            var current = queue.Dequeue();
            if (current.X > radius)
                continue;

            ComputeFoVForColumnPortion(current.X, current.TopVector, current.BottomVector, isOpaque, setFieldOfView, radius, queue);
        }
    }
    private static void ComputeFoVForColumnPortion(int x, DirectionVector topVector, DirectionVector bottomVector, Func<int, int, bool> isOpaque, Action<int, int> setFieldOfView, int radius, Queue<ColumnPortion> queue)
    {
        int topY;
        if (x == 0)
            topY = 0;
        else
        {
            int quotient = (2 * x + 1) * topVector.Y / (2 * topVector.X);
            int remainder = (2 * x + 1) * topVector.Y % (2 * topVector.X);

            if (remainder > topVector.X)
                topY = quotient + 1;
            else
                topY = quotient;
        }

        int bottomY;
        if (x == 0)
            bottomY = 0;
        else
        {
            int quotient = (2 * x - 1) * bottomVector.Y / (2 * bottomVector.X);
            int remainder = (2 * x - 1) * bottomVector.Y % (2 * bottomVector.X);

            if (remainder >= bottomVector.X)
                bottomY = quotient + 1;
            else
                bottomY = quotient;
        }

        bool? wasLastCellOpaque = null;
        for (int y = topY; y >= bottomY; --y)
        {
            bool inRadius = IsInRadius(x, y, radius);
            if (inRadius)
            {
                setFieldOfView(x, y);
            }

            bool currentIsOpaque = !inRadius || isOpaque(x, y);
            if (wasLastCellOpaque != null)
            {
                if (currentIsOpaque)
                {
                    if (!wasLastCellOpaque.Value)
                    {
                        queue.Enqueue(new ColumnPortion(x + 1, new DirectionVector(x * 2 - 1, y * 2 + 1), topVector));
                    }
                }
                else if (wasLastCellOpaque.Value)
                {
                    topVector = new DirectionVector(x * 2 + 1, y * 2 + 1);
                }
            }
            wasLastCellOpaque = currentIsOpaque;
        }
        if (wasLastCellOpaque != null && !wasLastCellOpaque.Value)
            queue.Enqueue(new ColumnPortion(x + 1, bottomVector, topVector));
    }

    private struct ColumnPortion
    {
        public int X { get; private set; }
        public DirectionVector BottomVector { get; private set; }
        public DirectionVector TopVector { get; private set; }

        public ColumnPortion(int x, DirectionVector bottom, DirectionVector top)
            : this()
        {
            this.X = x;
            this.BottomVector = bottom;
            this.TopVector = top;
        }
    }

    private static bool IsInRadius(int x, int y, int length)
    {
        return (2 * x - 1) * (2 * x - 1) + (2 * y - 1) * (2 * y - 1) <= 4 * length * length;
    }

    private struct DirectionVector
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public DirectionVector(int x, int y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }
    }

    // Octant helpers
    //
    //
    //                 \2|1/
    //                 3\|/0
    //               ----+----
    //                 4/|\7
    //                 /5|6\
    //
    // 

    private static Func<int, int, T> TranslateOrigin<T>(Func<int, int, T> f, int x, int y)
    {
        return (a, b) => f(a + x, b + y);
    }

    private static Action<int, int> TranslateOrigin(Action<int, int> f, int x, int y)
    {
        return (a, b) => f(a + x, b + y);
    }

    private static Func<int, int, T> TranslateOctant<T>(Func<int, int, T> f, int octant)
    {
        switch (octant)
        {
            default: return f;
            case 1: return (x, y) => f(y, x);
            case 2: return (x, y) => f(-y, x);
            case 3: return (x, y) => f(-x, y);
            case 4: return (x, y) => f(-x, -y);
            case 5: return (x, y) => f(-y, -x);
            case 6: return (x, y) => f(y, -x);
            case 7: return (x, y) => f(x, -y);
        }
    }

    private static Action<int, int> TranslateOctant(Action<int, int> f, int octant)
    {
        switch (octant)
        {
            default: return f;
            case 1: return (x, y) => f(y, x);
            case 2: return (x, y) => f(-y, x);
            case 3: return (x, y) => f(-x, y);
            case 4: return (x, y) => f(-x, -y);
            case 5: return (x, y) => f(-y, -x);
            case 6: return (x, y) => f(y, -x);
            case 7: return (x, y) => f(x, -y);
        }
    }
}