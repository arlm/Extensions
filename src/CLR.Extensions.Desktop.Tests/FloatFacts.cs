// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using Xunit;
using CLR.Extensions.Portable;

/// <summary>
/// Floar testing comparisons using regular delta algorithm
/// </summary>
/// <remarks>
/// Test cases extracted from <a href="http://floating-point-gui.de/errors/comparison/">floating-point.gui.de</a> 
/// </remarks>
[Trait("Float facts", "Regular algorithm")]
public class FloatFacts
{
    /// <summary>
    ///  Regular large numbers - generally not problematic
    /// </summary>
    [Fact]
    public void BigNumbers()
    {
        Assert.True(1000000f.NearlyEquals(1000001f));
        Assert.True(1000001f.NearlyEquals(1000000f));

        Assert.False(10001f.NearlyEquals(10000f));
        Assert.False(10000f.NearlyEquals(10001f));
    }

    /// <summary>
    ///  Negative large numbers
    /// </summary>
    [Fact]
    public void BigNegativeNumbers()
    {
        Assert.True((-1000000f).NearlyEquals(-1000001f));
        Assert.True((-1000001f).NearlyEquals(-1000000f));

        Assert.False((-10000f).NearlyEquals(-10001f));
        Assert.False((-10001f).NearlyEquals(-10000f));
    }

    /// <summary>
    ///  Around 1 numbers
    /// </summary>
    [Fact]
    public void Around1Numbers()
    {
        Assert.True(1.0000001f.NearlyEquals(1.0000002f));
        Assert.True(1.0000002f.NearlyEquals(1.0000001f));

        Assert.False(1.0002f.NearlyEquals(1.0001f));
        Assert.False(1.0001f.NearlyEquals(1.0002f));
    }

    /// <summary>
    ///  Around -1 numbers
    /// </summary>
    [Fact]
    public void AroundMinus1Numbers()
    {
        Assert.True((-1.000001f).NearlyEquals(-1.000002f));
        Assert.True((-1.000002f).NearlyEquals(-1.000001f));

        Assert.False((-1.0001f).NearlyEquals(-1.0002f));
        Assert.False((-1.0002f).NearlyEquals(-1.0001f));
    }

    /// <summary>
    ///  Numbers between 1 and 0
    /// </summary>
    [Fact]
    public void SmallNumbers()
    {
        Assert.True(0.000000001000001f.NearlyEquals(0.000000001000002f));
        Assert.True(0.000000001000002f.NearlyEquals(0.000000001000001f));

        Assert.False(0.000000000001002f.NearlyEquals(0.000000000001001f));
        Assert.False(0.000000000001001f.NearlyEquals(0.000000000001002f));
    }

    /// <summary>
    ///  Numbers between -1 and 0
    /// </summary>
    [Fact]
    public void SmallNegativeNumbers()
    {
        Assert.True((-0.000000001000001f).NearlyEquals(-0.000000001000002f));
        Assert.True((-0.000000001000002f).NearlyEquals(-0.000000001000001f));

        Assert.False((-0.000000000001002f).NearlyEquals(-0.000000000001001f));
        Assert.False((-0.000000000001001f).NearlyEquals(-0.000000000001002f));
    }

    /// <summary>
    ///  Comparisons involving zero
    /// </summary>
    [Fact]
    public void Zero()
    {
        Assert.True(0.0f.NearlyEquals(0.0f));
        Assert.True(0.0f.NearlyEquals(-0.0f));
        Assert.True((-0.0f).NearlyEquals(-0.0f));
        
        Assert.False(0.00000001f.NearlyEquals(0.0f));
        Assert.False(0.0f.NearlyEquals(0.00000001f));
        Assert.False((-0.00000001f).NearlyEquals(0.0f));
        Assert.False(0.0f.NearlyEquals(-0.00000001f));

        Assert.True(0.0f.NearlyEquals(1e-40f, 0.01f));
        Assert.True(1e-40f.NearlyEquals(0.0f, 0.01f));

        Assert.False(1e-40f.NearlyEquals(0.0f, 0.000001f));
        Assert.False(0.0f.NearlyEquals(1e-40f, 0.000001f));

        Assert.True(0.0f.NearlyEquals(-1e-40f, 0.1f));
        Assert.True((-1e-40f).NearlyEquals(0.0f, 0.1f));

        Assert.False((-1e-40f).NearlyEquals(0.0f, 0.00000001f));
        Assert.False(0.0f.NearlyEquals(-1e-40f, 0.00000001f));
    }

    /// <summary>
    ///  Comparisons involving extreme values (overflow potential)
    /// </summary>
    [Fact]
    public void ExtremeMax()
    {
        Assert.True(float.MaxValue.NearlyEquals(float.MaxValue));

        Assert.False(float.MaxValue.NearlyEquals(-float.MaxValue));
        Assert.False((-float.MaxValue).NearlyEquals(float.MaxValue));
        Assert.False(float.MaxValue.NearlyEquals(float.MaxValue / 2));
        Assert.False(float.MaxValue.NearlyEquals(-float.MaxValue / 2));
        Assert.False((-float.MaxValue).NearlyEquals(float.MaxValue / 2));
    }

    /// <summary>
    ///  Comparisons involving infinities
    /// </summary>
    [Fact]
    public void Infitities()
    {
        Assert.True(float.PositiveInfinity.NearlyEquals(float.PositiveInfinity));
        Assert.True(float.NegativeInfinity.NearlyEquals(float.NegativeInfinity));

        Assert.False(float.NegativeInfinity.NearlyEquals(float.PositiveInfinity));
        Assert.False(float.PositiveInfinity.NearlyEquals(float.MaxValue));
        Assert.False(float.NegativeInfinity.NearlyEquals(-float.MaxValue));
    }

    /// <summary>
    ///  Comparisons involving NaN values
    /// </summary>
    [Fact]
    public void NaN()
    {
        Assert.False(float.NaN.NearlyEquals(float.NaN));
        Assert.False(float.NaN.NearlyEquals(0.0f));
        Assert.False((-0.0f).NearlyEquals(float.NaN));
        Assert.False(float.NaN.NearlyEquals(-0.0f));
        Assert.False(0.0f.NearlyEquals(float.NaN));
        Assert.False(float.NaN.NearlyEquals(float.PositiveInfinity));
        Assert.False(float.PositiveInfinity.NearlyEquals(float.NaN));
        Assert.False(float.NaN.NearlyEquals(float.NegativeInfinity));
        Assert.False(float.NegativeInfinity.NearlyEquals(float.NaN));
        Assert.False(float.NaN.NearlyEquals(float.MaxValue));
        Assert.False(float.MaxValue.NearlyEquals(float.NaN));
        Assert.False(float.NaN.NearlyEquals(-float.MaxValue));
        Assert.False((-float.MaxValue).NearlyEquals(float.NaN));
        Assert.False(float.NaN.NearlyEquals(float.MinValue));
        Assert.False(float.MinValue.NearlyEquals(float.NaN));
        Assert.False(float.NaN.NearlyEquals(-float.MinValue));
        Assert.False((-float.MinValue).NearlyEquals(float.NaN));
    }

    /// <summary>
    ///  Comparisons of numbers on opposite sides of 0
    /// </summary>
    [Fact]
    public void Oposite()
    {
        Assert.True((10 * float.MinValue).NearlyEquals(10 * -float.MinValue));

        Assert.False(1.000000001f.NearlyEquals(-1.0f));
        Assert.False((-1.0f).NearlyEquals(1.000000001f));
        Assert.False((-1.000000001f).NearlyEquals(1.0f));
        Assert.False(1.0f.NearlyEquals(-1.000000001f));
        Assert.False((10000 * float.MinValue).NearlyEquals(10000 * -float.MinValue));
    }

    /// <summary>
    ///  The really tricky part - comparisons of numbers very close to zero
    /// </summary>
    [Fact]
    public void CloseToZero()
    {
        Assert.True(float.MinValue.NearlyEquals(float.MinValue));
        Assert.True(float.MinValue.NearlyEquals(-float.MinValue));
        Assert.True((-float.MinValue).NearlyEquals(float.MinValue));
        Assert.True(float.MinValue.NearlyEquals(0));
        Assert.True(0f.NearlyEquals(float.MinValue));
        Assert.True((-float.MinValue).NearlyEquals(0));
        Assert.True(0f.NearlyEquals(-float.MinValue));

        Assert.False(0.000000001f.NearlyEquals(-float.MinValue));
        Assert.False(0.000000001f.NearlyEquals(float.MinValue));
        Assert.False(float.MinValue.NearlyEquals(0.000000001f));
        Assert.False((-float.MinValue).NearlyEquals(0.000000001f));
    }
}
