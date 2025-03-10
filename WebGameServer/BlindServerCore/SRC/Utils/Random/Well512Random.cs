namespace BlindServerCore.Utils;

public class Well512Random
{
    private uint[] _state = new uint[16];
    private uint _index = 0;

    public Well512Random(uint seed)
    {
        uint s = seed;
        for (int i = 0; i < 16; i++)
        {
            _state[i] = s;
            s += s + 13;
        }
    }

    public uint Next(int minValue, int maxValue)
    {
        return (uint)((Next() % (maxValue - minValue)) + minValue);
    }

    public uint Next(uint maxValue)
    {
        return Next() % maxValue;
    }

    public uint Next()
    {
        uint a, b, c, d;

        a = _state[_index];
        c = _state[(_index + 13) & 15];
        b = a ^ c ^ (a << 16) ^ (c << 15);
        c = _state[(_index + 9) & 15];
        c ^= (c >> 11);
        a = _state[_index] = b ^ c;
        d = a ^ ((a << 5) & 0xda442d24U);
        _index = (_index + 15) & 15;
        a = _state[_index];
        _state[_index] = a ^ b ^ d ^ (a << 2) ^ (b << 18) ^ (c << 28);

        return _state[_index];
    }
}
