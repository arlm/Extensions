// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using Xunit;
using CLR.Extensions.Portable;

/// <summary>
/// Double testing comparisons using regular delta algorithm
/// </summary>
/// <remarks>
/// Test cases extracted from <a href="http://floating-point-gui.de/errors/comparison/">floating-point.gui.de</a> 
/// </remarks>
[Trait("Double facts", "Regular algorithm")]
public class DoubleFacts
{
    /// <summary>
    ///  Regular large numbers - generally not problematic
    /// </summary>
    [Fact]
    public void BigNumbers()
    {
        Assert.True(1000000d.NearlyEquals(1000001d));
        Assert.True(1000001d.NearlyEquals(1000000d));

        Assert.False(10001d.NearlyEquals(10000d));
        Assert.False(10000d.NearlyEquals(10001d));
    }

    /// <summary>
    ///  Negative large numbers
    /// </summary>
    [Fact]
    public void BigNegativeNumbers()
    {
        Assert.True((-1000000d).NearlyEquals(-1000001d));
        Assert.True((-1000001d).NearlyEquals(-1000000d));

        Assert.False((-10000d).NearlyEquals(-10001d));
        Assert.False((-10001d).NearlyEquals(-10000d));
    }

    /// <summary>
    ///  Around 1 numbers
    /// </summary>
    [Fact]
    public void Around1Numbers()
    {
        Assert.True(1.0000001d.NearlyEquals(1.0000002d));
        Assert.True(1.0000002d.NearlyEquals(1.0000001d));

        Assert.False(1.0002d.NearlyEquals(1.0001d));
        Assert.False(1.0001d.NearlyEquals(1.0002d));
    }

    /// <summary>
    ///  Around -1 numbers
    /// </summary>
    [Fact]
    public void AroundMinus1Numbers()
    {
        Assert.True((-1.000001d).NearlyEquals(-1.000002d));
        Assert.True((-1.000002d).NearlyEquals(-1.000001d));

        Assert.False((-1.0001d).NearlyEquals(-1.0002d));
        Assert.False((-1.0002d).NearlyEquals(-1.0001d));
    }

    /// <summary>
    ///  Numbers between 1 and 0
    /// </summary>
    [Fact]
    public void SmallNumbers()
    {
        Assert.True(0.000000001000001d.NearlyEquals(0.000000001000002d));
        Assert.True(0.000000001000002d.NearlyEquals(0.000000001000001d));

        Assert.False(0.000000000001002d.NearlyEquals(0.000000000001001d));
        Assert.False(0.000000000001001d.NearlyEquals(0.000000000001002d));
    }

    /// <summary>
    ///  Numbers between -1 and 0
    /// </summary>
    [Fact]
    public void SmallNegativeNumbers()
    {
        Assert.True((-0.000000001000001d).NearlyEquals(-0.000000001000002d));
        Assert.True((-0.000000001000002d).NearlyEquals(-0.000000001000001d));

        Assert.False((-0.000000000001002d).NearlyEquals(-0.000000000001001d));
        Assert.False((-0.000000000001001d).NearlyEquals(-0.000000000001002d));
    }

    /// <summary>
    ///  Comparisons involving zero
    /// </summary>
    [Fact]
    public void Zero()
    {
        Assert.True(0.0d.NearlyEquals(0.0d));
        Assert.True(0.0d.NearlyEquals(-0.0d));
        Assert.True((-0.0d).NearlyEquals(-0.0d));
        
        Assert.False(0.00000001d.NearlyEquals(0.0d));
        Assert.False(0.0d.NearlyEquals(0.00000001d));
        Assert.False((-0.00000001d).NearlyEquals(0.0d));
        Assert.False(0.0d.NearlyEquals(-0.00000001d));

        Assert.True(0.0d.NearlyEquals(1e-40d, 0.01d));
        Assert.True(1e-40d.NearlyEquals(0.0d, 0.01d));

        Assert.False(1e-40d.NearlyEquals(0.0d, 0.000001d));
        Assert.False(0.0d.NearlyEquals(1e-40d, 0.000001d));

        Assert.True(0.0d.NearlyEquals(-1e-40d, 0.1d));
        Assert.True((-1e-40d).NearlyEquals(0.0d, 0.1d));

        Assert.False((-1e-40d).NearlyEquals(0.0d, 0.00000001d));
        Assert.False(0.0d.NearlyEquals(-1e-40d, 0.00000001d));
    }

    /// <summary>
    ///  Comparisons involving extreme values (overflow potential)
    /// </summary>
    [Fact]
    public void ExtremeMax()
    {
        Assert.True(double.MaxValue.NearlyEquals(double.MaxValue));

        Assert.False(double.MaxValue.NearlyEquals(-double.MaxValue));
        Assert.False((-double.MaxValue).NearlyEquals(double.MaxValue));
        Assert.False(double.MaxValue.NearlyEquals(double.MaxValue / 2));
        Assert.False(double.MaxValue.NearlyEquals(-double.MaxValue / 2));
        Assert.False((-double.MaxValue).NearlyEquals(double.MaxValue / 2));
    }

    /// <summary>
    ///  Comparisons involving infinities
    /// </summary>
    [Fact]
    public void Infitities()
    {
        Assert.True(double.PositiveInfinity.NearlyEquals(double.PositiveInfinity));
        Assert.True(double.NegativeInfinity.NearlyEquals(double.NegativeInfinity));

        Assert.False(double.NegativeInfinity.NearlyEquals(double.PositiveInfinity));
        Assert.False(double.PositiveInfinity.NearlyEquals(double.MaxValue));
        Assert.False(double.NegativeInfinity.NearlyEquals(-double.MaxValue));
    }

    /// <summary>
    ///  Comparisons involving NaN values
    /// </summary>
    [Fact]
    public void NaN()
    {
        Assert.False(double.NaN.NearlyEquals(double.NaN));
        Assert.False(double.NaN.NearlyEquals(0.0d));
        Assert.False((-0.0d).NearlyEquals(double.NaN));
        Assert.False(double.NaN.NearlyEquals(-0.0d));
        Assert.False(0.0d.NearlyEquals(double.NaN));
        Assert.False(double.NaN.NearlyEquals(double.PositiveInfinity));
        Assert.False(double.PositiveInfinity.NearlyEquals(double.NaN));
        Assert.False(double.NaN.NearlyEquals(double.NegativeInfinity));
        Assert.False(double.NegativeInfinity.NearlyEquals(double.NaN));
        Assert.False(double.NaN.NearlyEquals(double.MaxValue));
        Assert.False(double.MaxValue.NearlyEquals(double.NaN));
        Assert.False(double.NaN.NearlyEquals(-double.MaxValue));
        Assert.False((-double.MaxValue).NearlyEquals(double.NaN));
        Assert.False(double.NaN.NearlyEquals(double.MinValue));
        Assert.False(double.MinValue.NearlyEquals(double.NaN));
        Assert.False(double.NaN.NearlyEquals(-double.MinValue));
        Assert.False((-double.MinValue).NearlyEquals(double.NaN));
    }

    /// <summary>
    ///  Comparisons of numbers on opposite sides of 0
    /// </summary>
    [Fact]
    public void Oposite()
    {
        Assert.True((10 * double.MinValue).NearlyEquals(10 * -double.MinValue));

        Assert.False(1.000000001d.NearlyEquals(-1.0d));
        Assert.False((-1.0d).NearlyEquals(1.000000001d));
        Assert.False((-1.000000001d).NearlyEquals(1.0d));
        Assert.False(1.0d.NearlyEquals(-1.000000001d));
        Assert.False((10000 * double.MinValue).NearlyEquals(10000 * -double.MinValue));
    }

    /// <summary>
    ///  The really tricky part - comparisons of numbers very close to zero
    /// </summary>
    [Fact]
    public void CloseToZero()
    {
        Assert.True(double.MinValue.NearlyEquals(double.MinValue));
        Assert.True(double.MinValue.NearlyEquals(-double.MinValue));
        Assert.True((-double.MinValue).NearlyEquals(double.MinValue));
        Assert.True(double.MinValue.NearlyEquals(0));
        Assert.True(0d.NearlyEquals(double.MinValue));
        Assert.True((-double.MinValue).NearlyEquals(0));
        Assert.True(0d.NearlyEquals(-double.MinValue));

        Assert.False(0.000000001d.NearlyEquals(-double.MinValue));
        Assert.False(0.000000001d.NearlyEquals(double.MinValue));
        Assert.False(double.MinValue.NearlyEquals(0.000000001d));
        Assert.False((-double.MinValue).NearlyEquals(0.000000001d));
    }
}
