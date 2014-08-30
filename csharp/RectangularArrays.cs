//----------------------------------------------------------------------------------------
//	Copyright © 2007 - 2013 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class provides the logic to simulate Java rectangular arrays, which are jagged
//	arrays with inner arrays of the same length. A size of -1 indicates unknown length.
//----------------------------------------------------------------------------------------
internal static partial class RectangularArrays
{
    internal static double[][] ReturnRectangularDoubleArray(int Size1, int Size2)
    {
        double[][] Array;
        if (Size1 > -1)
        {
            Array = new double[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new double[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static DSCompiler[][] ReturnRectangularDSCompilerArray(int Size1, int Size2)
    {
        DSCompiler[][] Array;
        if (Size1 > -1)
        {
            Array = new DSCompiler[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new DSCompiler[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static int[][] ReturnRectangularIntArray(int Size1, int Size2)
    {
        int[][] Array;
        if (Size1 > -1)
        {
            Array = new int[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new int[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static double[][][] ReturnRectangularDoubleArray(int Size1, int Size2, int Size3)
    {
        double[][][] Array;
        if (Size1 > -1)
        {
            Array = new double[Size1][][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new double[Size2][];
                    if (Size3 > -1)
                    {
                        for (int Array2 = 0; Array2 < Size2; Array2++)
                        {
                            Array[Array1][Array2] = new double[Size3];
                        }
                    }
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static DerivativeStructure[][] ReturnRectangularDerivativeStructureArray(int Size1, int Size2)
    {
        DerivativeStructure[][] Array;
        if (Size1 > -1)
        {
            Array = new DerivativeStructure[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new DerivativeStructure[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static BicubicSplineFunction[][] ReturnRectangularBicubicSplineFunctionArray(int Size1, int Size2)
    {
        BicubicSplineFunction[][] Array;
        if (Size1 > -1)
        {
            Array = new BicubicSplineFunction[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new BicubicSplineFunction[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static BivariateFunction[][][] ReturnRectangularBivariateFunctionArray(int Size1, int Size2, int Size3)
    {
        BivariateFunction[][][] Array;
        if (Size1 > -1)
        {
            Array = new BivariateFunction[Size1][][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new BivariateFunction[Size2][];
                    if (Size3 > -1)
                    {
                        for (int Array2 = 0; Array2 < Size2; Array2++)
                        {
                            Array[Array1][Array2] = new BivariateFunction[Size3];
                        }
                    }
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static TricubicSplineFunction[][][] ReturnRectangularTricubicSplineFunctionArray(int Size1, int Size2, int Size3)
    {
        TricubicSplineFunction[][][] Array;
        if (Size1 > -1)
        {
            Array = new TricubicSplineFunction[Size1][][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new TricubicSplineFunction[Size2][];
                    if (Size3 > -1)
                    {
                        for (int Array2 = 0; Array2 < Size2; Array2++)
                        {
                            Array[Array1][Array2] = new TricubicSplineFunction[Size3];
                        }
                    }
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static BigFraction[][] ReturnRectangularBigFractionArray(int Size1, int Size2)
    {
        BigFraction[][] Array;
        if (Size1 > -1)
        {
            Array = new BigFraction[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new BigFraction[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }

    internal static long[][] ReturnRectangularLongArray(int Size1, int Size2)
    {
        long[][] Array;
        if (Size1 > -1)
        {
            Array = new long[Size1][];
            if (Size2 > -1)
            {
                for (int Array1 = 0; Array1 < Size1; Array1++)
                {
                    Array[Array1] = new long[Size2];
                }
            }
        }
        else
            Array = null;

        return Array;
    }
}