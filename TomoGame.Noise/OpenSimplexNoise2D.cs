namespace TomoGame.Noise;

/// <summary>Fractal 2D noise: several octaves of <see cref="OpenSimplexNoise"/> summed into [0, 1].</summary>
public class OpenSimplexNoise2D
{
    // The raw field's practical extreme. Its exact value is folklore from the source this was adapted from,
    // and it only sets how far a sample can stray outside [0, 1].
    private const float SimplexNoiseMax = 0.866f;

    private readonly OpenSimplexNoise _noise;
    private readonly int _octaves;
    private readonly double _persistence;
    private readonly float _scale;

    /// <param name="octaves">How many doublings of frequency to sum.</param>
    /// <param name="persistence">How much each successive octave contributes; below 1 to fade out.</param>
    /// <param name="scale">Divides the input, so larger values give broader features.</param>
    public OpenSimplexNoise2D(int octaves, double persistence, int seed, float scale)
    {
        _noise = new OpenSimplexNoise(seed);
        _octaves = octaves;
        _persistence = persistence;
        _scale = scale;
    }

    /// <summary>Samples the field, normalised to roughly [0, 1].</summary>
    public double GetValue(double x, double y)
    {
        double value = 0.0;
        double frequency = 1.0;
        double amplitude = 1.0;
        double maxValue = 0.0;

        for (int i = 0; i < _octaves; ++i)
        {
            double noise = _noise.Evaluate(x * frequency / _scale, y * frequency / _scale);
            noise = (noise + SimplexNoiseMax) / (SimplexNoiseMax * 2);
            value += noise * amplitude;
            maxValue += amplitude;
            amplitude *= _persistence;
            frequency *= 2.0;
        }

        return value / maxValue;
    }

    public float GetValue(float x, float y) => (float)GetValue((double)x, y);
}
